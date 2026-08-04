using Kanban.Domain.Ports.Out;
using Kanban.Infrastructure.Data;
using Kanban.Infrastructure.Reports;
using Kanban.Infrastructure.Repositories;
using Kanban.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kanban.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IProyectoRepository, ProyectoRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IColumnaRepository, ColumnaRepository>();
        services.AddScoped<ITareaRepository, TareaRepository>();

        services.AddScoped<IReportGenerator, PdfReportGenerator>();
        services.AddScoped<IReportGenerator, ExcelReportGenerator>();

        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }

    public static IServiceProvider ApplyInfrastructureMigrations(this IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.Migrate();
        }
        return serviceProvider;
    }
}
