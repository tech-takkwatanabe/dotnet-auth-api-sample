using System.Threading.Tasks;
using Api.Domain.DTOs;
using Api.Domain.Entities;
using Api.Domain.VOs;
using DomainUuid = Api.Domain.VOs.Uuid;

namespace Api.Application.Interfaces
{
  public interface IUserService
  {
    /**
     * ユーザーIDからユーザー情報を取得する
     * @param id ユーザーID
     * @return *dto.UserDTO ユーザー情報 (現在はUserエンティティを返す想定)
     */
    Task<UserEntity?> GetUserByUuidAsync(Uuid uuid);

    /**
     * ユーザーUUIDからユーザー情報を取得する (JWTのsubクレームなど)
     * @param uuid UUID
     * @return *dto.UserDTO ユーザー情報 (現在はUserエンティティを返す想定)
     */
    Task<UserEntity?> GetUserBySubAsync(Uuid uuid);

    /**
     * ユーザー登録
     * @param name ユーザー名
     * @param email メールアドレス
     * @param password パスワード
     * @return error エラー (成功時はTask.CompletedTask, エラー時は例外をスローする想定)
     */
    Task RegisterUserAsync(SignUpRequest request);

    /**
     * ユーザーログイン
     * @param email メールアドレス
     * @param password パスワード
     * @return (string AccessToken, string RefreshToken, Uuid UserUuid) 認証情報 (失敗時はnull)
     */
    Task<(string AccessToken, string RefreshToken, Uuid UserUuid)?> LoginUserAsync(LoginRequest request);

    /**
     * リフレッシュトークンを使用して新しいアクセストークンを取得する
     * @param refreshTokenValue リフレッシュトークンの文字列値
     * @return string 新しいアクセストークン (失敗時はnull)
     */
    Task<(string AccessToken, string RefreshToken, DomainUuid UserUuid)?> RefreshAccessTokenAsync(string refreshTokenValue);

    /**
     * ユーザーログアウト (リフレッシュトークンを無効化)
     * @param refreshTokenValue 無効化するリフレッシュトークンの文字列値
     */
    Task LogoutAsync(string refreshTokenValue);
  }
}