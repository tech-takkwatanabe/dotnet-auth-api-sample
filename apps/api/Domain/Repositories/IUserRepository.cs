using System.Threading.Tasks;
using Api.Domain.Entities;
using Api.Domain.VOs;

namespace Api.Domain.Repositories
{
  public interface IUserRepository
  {
    Task SaveAsync(User user);
    Task<User?> FindByEmailAsync(Email email);
    Task<User?> FindByIdAsync(Uuid id);
  }
}