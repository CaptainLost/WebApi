using Microsoft.EntityFrameworkCore;
using Users.Domain.Users;
using Users.Persistence.Database;

namespace Users.Persistence.Users;

internal sealed class RoleRepository : IRoleRepository
{
    private readonly UsersDbContext _dbContext;

    public RoleRepository(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Role?> GetByName(string name, CancellationToken cancellationToken = default)
    {
        string normalizedName = name.ToUpperInvariant();

        return await _dbContext.Roles
            .FirstOrDefaultAsync(r => r.Name.ToUpper() == normalizedName, cancellationToken);
    }
}