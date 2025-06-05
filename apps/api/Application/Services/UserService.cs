using System;
using System.Threading.Tasks;
using Api.Application.Interfaces;
using Api.Domain.Entities;
using Api.Domain.Repositories;
using Api.Domain.VOs;
// using Api.Application.DTOs; // DTOsの名前空間を想定

namespace Api.Application.Services
{
  public class UserService : IUserService
  {
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtUtils _jwtUtils;
    // private readonly IPasswordHasher _passwordHasher; // パスワードハッシュ化サービスのインターフェースを想定

    public UserService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtUtils jwtUtils
        // IPasswordHasher passwordHasher
        )
    {
      _userRepository = userRepository;
      _refreshTokenRepository = refreshTokenRepository;
      _jwtUtils = jwtUtils;
      // _passwordHasher = passwordHasher;
    }

    /**
     * ユーザーIDからユーザー情報を取得する
     * @param id ユーザーID
     * @return *dto.UserDTO ユーザー情報 (現在はUserエンティティを返す想定)
     */
    public async Task<User?> GetUserByIdAsync(Uuid id)
    {
      // TODO: Implement actual logic
      // 例: return await _userRepository.FindByIdAsync(id);
      await Task.CompletedTask; // 仮実装
      throw new NotImplementedException();
    }

    /**
     * ユーザーUUIDからユーザー情報を取得する (JWTのsubクレームなど)
     * @param uuid UUID
     * @return *dto.UserDTO ユーザー情報 (現在はUserエンティティを返す想定)
     */
    public async Task<User?> GetUserBySubAsync(Uuid uuid)
    {
      // TODO: Implement actual logic
      // 例: return await _userRepository.FindByIdAsync(uuid);
      await Task.CompletedTask; // 仮実装
      throw new NotImplementedException();
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
      // TODO: Implement actual logic
      // 1. Emailの重複チェック
      // 2. パスワードのハッシュ化 (例: var hashedPassword = _passwordHasher.Hash(password);)
      // 3. Userエンティティの作成
      // 4. _userRepository.SaveAsync(user);
      await Task.CompletedTask; // 仮実装
      throw new NotImplementedException();
    }

    /**
     * ユーザーログイン
     * @param email メールアドレス
     * @param password パスワード
     * @return (string AccessToken, string RefreshToken, Uuid UserId) 認証情報 (失敗時はnull)
     */
    public async Task<(string AccessToken, string RefreshToken, Uuid UserId)?> LoginUserAsync(Email email, string password)
    {
      // TODO: Implement actual logic
      // 1. _userRepository.FindByEmailAsync(email);
      // 2. ユーザー存在チェック
      // 3. パスワード検証 (例: if (!_passwordHasher.Verify(password, user.PasswordHash)) return null;)
      // 4. _jwtUtils.GenerateAccessToken(user.Id);
      // 5. _jwtUtils.GenerateRefreshToken(user.Id);
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
      // 1. _jwtUtils.ValidateTokenAndGetUserId(refreshTokenValue) でリフレッシュトークンを検証し、UserIdを取得 (あるいはリフレッシュトークンIDでDBから検索)
      // 2. DBからリフレッシュトークンエンティティを取得 (_refreshTokenRepository.FindByIdAsync など)
      // 3. トークンの有効期限や失効状態をチェック
      // 4. ユーザー存在チェック (_userRepository.FindByIdAsync)
      // 5. 新しいアクセストークンを生成 (_jwtUtils.GenerateAccessToken)
      // 6. (オプション) 古いリフレッシュトークンを無効化し、新しいリフレッシュトークンを発行・保存
      await Task.CompletedTask; // 仮実装
      throw new NotImplementedException();
    }
  }
}