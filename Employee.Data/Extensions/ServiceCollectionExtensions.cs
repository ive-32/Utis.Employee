using Employee.Data.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Employee.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmployeeDbContext(
        this IServiceCollection services,
        Action<EmployeeDbContextOptions> configureOptions)
    {
        services.AddOptions<EmployeeDbContextOptions>().Configure(configureOptions)
            .ValidateDataAnnotations().ValidateOnStart();

        services.AddDbContext<EmployeeDbContext>((sp, o) =>
        {
            var dbContextOptions = sp.GetRequiredService<IOptions<EmployeeDbContextOptions>>().Value;

            o.UseNpgsql(dbContextOptions.ConnectionString);
        });

        return services;
    }
}