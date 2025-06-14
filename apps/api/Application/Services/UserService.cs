using System;
using System.Threading.Tasks;
using Api.Application.Interfaces;
using Api.Domain.DTOs;
using Api.Domain.Entities;
using Api.Domain.Repositories;
using Api.Domain.VOs;
using Api.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using UUIDNext;
using DomainUuid = Api.Domain.VOs.Uuid;

namespace Api.Application.Services
{
  public class UserService(
      IUserRepository userRepository,
      IRefreshTokenRepository refreshTokenRepository,
      IJwtUtils jwtUtils,
      IPasswordHasher passwordHasher,
      IOptions<JwtSettings> jwtSettings
        ) : IUserService
  {
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IJwtUtils _jwtUtils = jwtUtils;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    /**
     * ユーザーIDからユーザー情報を取得する
     * @param id ユーザーID
     * @return *dto.UserDTO ユーザー情報 (現在はUserエンティティを返す想定)
     */
    public async Task<UserEntity?> GetUserByUuidAsync(DomainUuid uuid)
    {
      return await _userRepository.FindByUuidAsync(uuid);
    }

    /**
     * ユーザーIDからユーザー情報を取得する
     * @param email メールアドレス
     * @return *dto.UserDTO ユーザー情報 (現在はUserエンティティを返す想定)
     */
    public async Task<UserEntity?> GetUserByEmailAsync(Email email)
    {
      return await _userRepository.FindByEmailAsync(email);
    }

    /**
     * ユーザーUUIDからユーザー情報を取得する (JWTのsubクレームなど)
     * @param uuid UUID
     * @return *dto.UserDTO ユーザー情報 (現在はUserエンティティを返す想定)
     */
    public async Task<UserEntity?> GetUserBySubAsync(DomainUuid uuid)
    {
      return await _userRepository.FindByUuidAsync(uuid);
    }

    /**
     * ユーザー登録
     * @param name ユーザー名
     * @param email メールアドレス
     * @param password パスワード
     * @return error エラー (成功時はTask.CompletedTask, エラー時は例外をスローする想定)
     */
    public async Task RegisterUserAsync(SignUpRequest request)
    {
      // 1. Emailの重複チェック
      var existingUser = await _userRepository.FindByEmailAsync(request.Email);
      if (existingUser != null)
      {
        throw new InvalidOperationException("Email address is already in use.");
      }

      // 2. パスワードのハッシュ化
      var hashedPassword = _passwordHasher.Hash(request.Password.Value);

      // 3. UUID v7 (SQL Server向け) を生成
      var userUuid = new DomainUuid(UUIDNext.Uuid.NewDatabaseFriendly(UUIDNext.Database.SqlServer));

      // 4. Userエンティティの作成 (公開コンストラクタを使用し、PasswordHashは別途設定)
      // int Id はDBが自動生成する
      var newUser = new UserEntity(userUuid, request.Name, request.Email)
      {
        PasswordHash = hashedPassword // PasswordHashプロパティにはpublic setがあるため設定可能
      };

      // 5. ユーザーの保存
      try
      {
        await _userRepository.SaveAsync(newUser);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException("Failed to register user.", ex);
      }
    }

    /**
     * ユーザーログイン
     * @param email メールアドレス
     * @param password パスワード
     * @return (string AccessToken, string RefreshToken, Uuid UserUuid) 認証情報 (失敗時はnull)
     */
    public async Task<(string AccessToken, string RefreshToken, DomainUuid UserUuid)?> LoginUserAsync(LoginRequest request)
    {
      var email = request.Email;
      var password = request.Password.Value;

      // 1. ユーザーの取得
      var user = await _userRepository.FindByEmailAsync(email);
      if (user == null)
      {
        return null; // ユーザーが存在しない場合はnullを返す
      }

      // 2. パスワードの検証
      if (user.PasswordHash == null)
      {
        return null; // PasswordHash が null の場合は認証失敗
      }
      if (!_passwordHasher.Verify(password, user.PasswordHash!)) // null免除演算子を使用
      {
        return null; // パスワードが一致しない場合はnullを返す
      }

      // 3. アクセストークンの生成
      var accessToken = _jwtUtils.GenerateAccessToken(user.Uuid);

      // 4. リフレッシュトークンの生成と保存
      // IJwtUtils.GenerateRefreshToken が (string Token, DomainUuid Jti) を返す
      var (refreshTokenString, refreshTokenJti) = _jwtUtils.GenerateRefreshToken(user.Uuid);
      var expiresAt = DateTime.UtcNow.AddSeconds(_jwtSettings.RefreshTokenExpirationSeconds);

      // RefreshTokenEntityのIdにはトークン固有のID (JTI) を使用し、User.uuidでユーザーを紐付ける
      var refreshToken = new RefreshTokenEntity(refreshTokenJti, refreshTokenString, user.Uuid, expiresAt);
      await _refreshTokenRepository.SaveAsync(refreshToken);

      return (accessToken, refreshTokenString, user.Uuid);
    }

    /**
     * リフレッシュトークンを使用して新しいアクセストークンを取得する
     * @param refreshTokenValue リフレッシュトークンの文字列値
     * @return string 新しいアクセストークン (失敗時はnull)
     * @return (string AccessToken, string RefreshToken, DomainUuid UserUuid)? 新しい認証情報 (失敗時はnull)
     */
    public async Task<(string AccessToken, string RefreshToken, DomainUuid UserUuid)?> RefreshAccessTokenAsync(string refreshTokenValue)
    {
      // 1. リフレッシュトークンからユーザーUUIDを検証・取得
      // IJwtUtils.ValidateTokenAndGetSubAndJti が (DomainUuid? Sub, DomainUuid? Jti) を返す
      var (userUuidFromToken, jtiFromToken) = _jwtUtils.ValidateTokenAndGetSubAndJti(refreshTokenValue);

      if (userUuidFromToken == null || jtiFromToken == null)
      {
        return null; // トークンが無効または不正
      }

      // 2. リフレッシュトークンをリポジトリから取得 (IDとしてJTIを使用)
      // JTIを使用してリフレッシュトークンを検索
      var storedRefreshToken = await _refreshTokenRepository.FindByJtiAsync(jtiFromToken);

      if (storedRefreshToken == null)
      {
        return null; // トークンが存在しない (CS8602対応のため早期リターン)
      }

      // storedRefreshToken の検証:
      // - トークン文字列が一致すること (リプレイアタック対策)
      // - UserId がトークンから取得したUUIDと一致すること
      // - (オプション) IsRevoked フラグのチェック (必要であればRefreshTokenEntityに追加)
      // - 有効期限内であること
      if (storedRefreshToken.Token != refreshTokenValue ||
          storedRefreshToken.UserId != userUuidFromToken ||
          storedRefreshToken.ExpiresAt < DateTime.UtcNow)
      {
        return null; // トークンが存在しない、ユーザーが一致しない、失効済み、または期限切れ
      }

      // 3. ユーザー情報を取得
      var user = await _userRepository.FindByUuidAsync(userUuidFromToken);
      if (user == null)
      {
        return null; // ユーザーが存在しない
      }

      // 4. 新しいアクセストークンを生成
      var newAccessToken = _jwtUtils.GenerateAccessToken(user.Uuid);

      // 5. リフレッシュトークンのローテーション: 古いトークンを削除し、新しいトークンを生成・保存
      // JTIを使用して古いリフレッシュトークンを削除
      await _refreshTokenRepository.DeleteByJtiAsync(jtiFromToken);

      // IJwtUtils.GenerateRefreshToken が (string Token, DomainUuid Jti) を返す
      var (newRefreshTokenString, newRefreshTokenJti) = _jwtUtils.GenerateRefreshToken(user.Uuid);
      var newExpiresAt = DateTime.UtcNow.AddSeconds(_jwtSettings.RefreshTokenExpirationSeconds);
      var newRefreshToken = new RefreshTokenEntity(newRefreshTokenJti, newRefreshTokenString, user.Uuid, newExpiresAt);
      await _refreshTokenRepository.SaveAsync(newRefreshToken);

      return (newAccessToken, newRefreshTokenString, user.Uuid);
    }
  }
}