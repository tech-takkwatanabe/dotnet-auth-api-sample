using System.Threading.Tasks;
using Api.Domain.Entities;
using Api.Domain.Repositories;
using Api.Domain.VOs;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Persistence
{
  public class UserRepository(ApplicationDbContext context) : IUserRepository
  {
    private readonly ApplicationDbContext _context = context;

    public async Task<UserEntity?> FindByEmailAsync(Email email)
    {
      return await _context.Users
                          .FirstOrDefaultAsync(u => u.Email.Value == email.Value);
    }

    public async Task<UserEntity?> FindByUuidAsync(Uuid uuid)
    {
      return await _context.Users.FirstOrDefaultAsync(u => u.Uuid == uuid);
    }

    public async Task SaveAsync(UserEntity user)
    {
      var existingUser = await _context.Users
                          .AsTracking()
                          .FirstOrDefaultAsync(u => u.Id == user.Id && user.Id != 0); // int型のIdで検索 (0は新規扱い)

      if (existingUser == null)
      {
        _context.Users.Add(user);
      }
      else
      {
        _context.Entry(existingUser).CurrentValues.SetValues(user);
      }
      await _context.SaveChangesAsync();
    }
  }
}