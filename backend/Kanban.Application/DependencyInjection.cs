using Kanban.Application.Services;
using Kanban.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Kanban.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProyectoService, ProyectoService>();
        services.AddScoped<IColumnaService, ColumnaService>();
        services.AddScoped<ITareaService, TareaService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<Kanban.Application.Reports.IReportService, Kanban.Application.Reports.ReportService>();

        return services;
    }
}
