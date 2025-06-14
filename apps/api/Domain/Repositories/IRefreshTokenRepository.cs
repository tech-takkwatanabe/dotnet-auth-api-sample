using System.Threading.Tasks;
using Api.Domain.Entities;
using Api.Domain.VOs;

namespace Api.Domain.Repositories
{
  public interface IRefreshTokenRepository
  {
    Task<RefreshTokenEntity?> FindByJtiAsync(Uuid jti);
    Task SaveAsync(RefreshTokenEntity refreshToken);
    Task DeleteByJtiAsync(Uuid jti);
  }
}