using System.Threading.Tasks;
using Api.Domain.Entities;
using Api.Domain.Repositories;
using Api.Domain.VOs;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Persistence
{
  public class RefreshTokenRepository(ApplicationDbContext context) : IRefreshTokenRepository
  {
    private readonly ApplicationDbContext _context = context;

    public async Task<RefreshTokenEntity?> FindByUuidAsync(Uuid uuid)
    {
      return await _context.RefreshTokens
                           .FirstOrDefaultAsync(rt => rt.Id.Value == uuid.Value); // Now correctly refers to RefreshTokenEntity.Id
    }

    public async Task SaveAsync(RefreshTokenEntity refreshToken)
    {
      var existingToken = await _context.RefreshTokens
                                        .AsTracking()
                                        .FirstOrDefaultAsync(rt => rt.Id.Value == refreshToken.Id.Value); // Now correctly refers to RefreshTokenEntity.Id

      if (existingToken == null)
      {
        _context.RefreshTokens.Add(refreshToken);
      }
      else
      {
        _context.Entry(existingToken).CurrentValues.SetValues(refreshToken);
      }
      await _context.SaveChangesAsync();
    }

    public async Task DeleteByUuidAsync(Uuid uuid)
    {
      var tokenToDelete = await _context.RefreshTokens
                                       .FirstOrDefaultAsync(rt => rt.Id.Value == uuid.Value); // Now correctly refers to RefreshTokenEntity.Id

      if (tokenToDelete != null)
      {
        _context.RefreshTokens.Remove(tokenToDelete);
        await _context.SaveChangesAsync();
      }
    }
  }
}