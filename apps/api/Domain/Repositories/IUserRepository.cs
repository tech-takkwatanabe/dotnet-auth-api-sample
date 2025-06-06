using System.Threading.Tasks;
using Api.Domain.Entities;
using Api.Domain.VOs;

namespace Api.Domain.Repositories
{
  public interface IUserRepository
  {
    Task SaveAsync(UserEntity user);
    Task<UserEntity?> FindByEmailAsync(Email email);
    Task<UserEntity?> FindByIdAsync(Uuid id);
  }
}