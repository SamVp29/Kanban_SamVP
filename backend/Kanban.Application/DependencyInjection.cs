using Kanban.Application.Ports.In;
using Kanban.Application.Reports;
using Kanban.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kanban.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProyectoUseCase, ProyectoService>();
        services.AddScoped<IColumnaUseCase, ColumnaService>();
        services.AddScoped<ITareaUseCase, TareaService>();
        services.AddScoped<IAuthUseCase, AuthService>();
        services.AddScoped<IReportUseCase, ReportService>();

        return services;
    }
}
