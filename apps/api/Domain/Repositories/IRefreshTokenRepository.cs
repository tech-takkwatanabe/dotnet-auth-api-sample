using System.Threading.Tasks;
using Api.Domain.Entities;
using Api.Domain.VOs;

namespace Api.Domain.Repositories
{
  public interface IRefreshTokenRepository
  {
    Task SaveAsync(RefreshTokenEntity refreshToken);
    Task<RefreshTokenEntity?> FindByIdAsync(Uuid id);
    Task DeleteByIdAsync(Uuid id);
  }
}