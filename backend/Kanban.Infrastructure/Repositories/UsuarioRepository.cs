using Kanban.Domain.Entities;
using Kanban.Domain.Ports.Out;
using Kanban.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Kanban.Infrastructure.Repositories;

public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Correo == email);
    }
}
