using System.Threading.Tasks;
using Api.Domain.Entities;
using Api.Domain.VOs;

namespace Api.Domain.Repositories
{
  public interface IRefreshTokenRepository
  {
    Task SaveAsync(RefreshTokenEntity refreshToken);
    Task<RefreshTokenEntity?> FindByUuidAsync(Uuid uuid);
    Task DeleteByUuidAsync(Uuid uuid);
  }
}