using System.Threading.Tasks;
using Api.Domain.Entities;
using Api.Domain.VOs;
// using Api.Application.DTOs; // DTOsの名前空間を想定

namespace Api.Application.Interfaces
{
  public interface IUserService
  {
    /**
     * ユーザーIDからユーザー情報を取得する
     * @param id ユーザーID
     * @return *dto.UserDTO ユーザー情報 (現在はUserエンティティを返す想定)
     */
    Task<User?> GetUserByIdAsync(Uuid id);

    /**
     * ユーザーUUIDからユーザー情報を取得する (JWTのsubクレームなど)
     * @param uuid UUID
     * @return *dto.UserDTO ユーザー情報 (現在はUserエンティティを返す想定)
     */
    Task<User?> GetUserBySubAsync(Uuid uuid);

    /**
     * ユーザー登録
     * @param name ユーザー名
     * @param email メールアドレス
     * @param password パスワード
     * @return error エラー (成功時はTask.CompletedTask, エラー時は例外をスローする想定)
     */
    Task RegisterUserAsync(Name name, Email email, string password);

    /**
     * ユーザーログイン
     * @param email メールアドレス
     * @param password パスワード
     * @return (string AccessToken, string RefreshToken, Uuid UserId) 認証情報 (失敗時はnull)
     */
    Task<(string AccessToken, string RefreshToken, Uuid UserId)?> LoginUserAsync(Email email, string password);

    /**
     * リフレッシュトークンを使用して新しいアクセストークンを取得する
     * @param refreshTokenValue リフレッシュトークンの文字列値
     * @return string 新しいアクセストークン (失敗時はnull)
     */
    Task<string?> RefreshAccessTokenAsync(string refreshTokenValue);
  }
}