using System;
using System.Threading.Tasks;
using Api.Application.Interfaces;
using Api.Domain.Entities;
using Api.Domain.Repositories;
using Api.Domain.VOs;
using UUIDNext;
using DomainUuid = Api.Domain.VOs.Uuid; // Uuidのエイリアスを定義

namespace Api.Application.Services
{
  public class UserService(
      IUserRepository userRepository,
      IRefreshTokenRepository refreshTokenRepository,
      IJwtUtils jwtUtils,
      IPasswordHasher passwordHasher
        ) : IUserService
  {
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IJwtUtils _jwtUtils = jwtUtils;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

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
      try
      {
        return await _userRepository.FindByEmailAsync(email);
      }
      catch (Exception ex)
      {
        return null;
      }
    }

    /**
     * ユーザーUUIDからユーザー情報を取得する (JWTのsubクレームなど)
     * @param uuid UUID
     * @return *dto.UserDTO ユーザー情報 (現在はUserエンティティを返す想定)
     */
    public async Task<UserEntity?> GetUserBySubAsync(DomainUuid uuid)
    {
      try
      {
        return await _userRepository.FindByUuidAsync(uuid);
      }
      catch (Exception ex)
      {
        return null;
      }
    }

    /**
     * ユーザー登録
     * @param name ユーザー名
     * @param email メールアドレス
     * @param password パスワード
     * @return error エラー (成功時はTask.CompletedTask, エラー時は例外をスローする想定)
     */
    public async Task RegisterUserAsync(Name name, Email email, string password)
    {
      // 1. Emailの重複チェック
      var existingUser = await _userRepository.FindByEmailAsync(email);
      if (existingUser != null)
      {
        throw new InvalidOperationException("Email address is already in use.");
      }

      // 2. パスワードのハッシュ化
      var hashedPassword = _passwordHasher.Hash(password);

      // 3. UUID v7 (SQL Server向け) を生成
      var userUuid = new DomainUuid(UUIDNext.Uuid.NewDatabaseFriendly(UUIDNext.Database.SqlServer));

      // 4. Userエンティティの作成 (公開コンストラクタを使用し、PasswordHashは別途設定)
      // int Id はDBが自動生成する
      var newUser = new UserEntity(userUuid, name, email)
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
    public async Task<(string AccessToken, string RefreshToken, DomainUuid UserUuid)?> LoginUserAsync(Email email, string password)
    {
      // TODO: Implement actual logic
      // 1. _userRepository.FindByEmailAsync(email);
      // 2. ユーザー存在チェック
      // 3. パスワード検証 (例: if (!_passwordHasher.Verify(password, user.PasswordHash)) return null;)
      // 4. var accessToken = _jwtUtils.GenerateAccessToken(user.Uuid); // 'sub' クレームに user.Uuid を使用
      // 5. var refreshTokenString = _jwtUtils.GenerateRefreshToken(user.Uuid); // 'sub' クレームに user.Uuid を使用
      // 6. リフレッシュトークンをDBに保存 (_refreshTokenRepository.SaveAsync)
      await Task.CompletedTask; // 仮実装
      throw new NotImplementedException();
    }

    /**
     * リフレッシュトークンを使用して新しいアクセストークンを取得する
     * @param refreshTokenValue リフレッシュトークンの文字列値
     * @return string 新しいアクセストークン (失敗時はnull)
     */
    public async Task<string?> RefreshAccessTokenAsync(string refreshTokenValue)
    {
      // TODO: Implement actual logic
      // 1. var userUuid = _jwtUtils.ValidateTokenAndGetUserId(refreshTokenValue); // 'sub' クレームから User.Uuid を取得
      // 2. DBからリフレッシュトークンエンティティを取得 (_refreshTokenRepository.FindByUuidAsync など)
      // 3. トークンの有効期限や失効状態をチェック
      // 4. ユーザー存在チェック (if (userUuid.HasValue) await _userRepository.FindByUuidAsync(userUuid.Value);)
      // 5. 新しいアクセストークンを生成 (if (user != null) _jwtUtils.GenerateAccessToken(user.Uuid);)
      // 6. (オプション) 古いリフレッシュトークンを無効化し、新しいリフレッシュトークンを発行・保存
      await Task.CompletedTask; // 仮実装
      throw new NotImplementedException();
    }
  }
}