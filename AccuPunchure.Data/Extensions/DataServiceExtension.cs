using AccuPunchure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AccuPunchure.Data.Extensions;

public static class DataServiceExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        services.AddScoped<OrganizationRepository>();
        services.AddScoped<UserRepository>();
        services.AddScoped<EmployeeRepository>();
        services.AddScoped<PunchRepository>();

        return services;
    }
}