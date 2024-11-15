using Employee.Data;
using Employee.Data.Extensions;
using Employee.Service.Interceptors;
using Employee.Service.RpcServices;
using Employee.Service.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Employee.Service;

public class Startup
{
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<ExceptionHandlingServerInterceptor>();
        });
        services.AddEmployeeDbContext(o =>
        {
            o.ConnectionString = Configuration.GetConnectionString(nameof(EmployeeDbContext))!;
        });

        services.TryAddScoped<IEmployeeService, EmployeeService>();
    }

    public void Configure(IApplicationBuilder app)
    {
        app
            .UseRouting()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<EmployeeRpcService>();
            });
    }
}