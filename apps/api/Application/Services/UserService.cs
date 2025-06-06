using System;
using System.Threading.Tasks;
using Api.Application.Interfaces;
using Api.Domain.Entities;
using Api.Domain.Repositories;
using Api.Domain.VOs;
// using Api.Application.DTOs; // DTOsの名前空間を想定

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
    public async Task<UserEntity?> GetUserByUuidAsync(Uuid uuid)
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
    public async Task<UserEntity?> GetUserBySubAsync(Uuid uuid)
    {
      // GetUserByIdAsync と同じロジックで良いため、そちらを呼び出すか、直接リポジトリを呼び出す
      return await _userRepository.FindByUuidAsync(uuid);
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
        // TODO: より具体的なカスタム例外をスローすることを検討 (例: DuplicateEmailException)
        throw new InvalidOperationException("Email address is already in use.");
      }

      // 2. パスワードのハッシュ化
      var hashedPassword = _passwordHasher.Hash(password);

      // 3. Uuidの生成 (ここでは標準のGuidをUuidに変換)
      var userUuid = new Uuid(Guid.NewGuid()); // TODO: UUID v7 を使用する場合は適切な生成方法に変更

      // 4. Userエンティティの作成 (公開コンストラクタを使用し、PasswordHashは別途設定)
      // int Id はDBが自動生成する
      var newUser = new UserEntity(userUuid, name, email)
      {
        PasswordHash = hashedPassword // PasswordHashプロパティにはpublic setがあるため設定可能
      };
      // 5. ユーザーの保存
      await _userRepository.SaveAsync(newUser);
    }

    /**
     * ユーザーログイン
     * @param email メールアドレス
     * @param password パスワード
     * @return (string AccessToken, string RefreshToken, Uuid UserUuid) 認証情報 (失敗時はnull)
     */
    public async Task<(string AccessToken, string RefreshToken, Uuid UserUuid)?> LoginUserAsync(Email email, string password)
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