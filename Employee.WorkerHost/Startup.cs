using Employee.WorkerHost.Clients;
using Employee.WorkerHost.Configurations;
using Employee.WorkerHost.Jobs;
using Quartz;

namespace Employee.WorkerHost;

public class Startup
{
    private readonly IConfiguration _configuration;
    
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 1; 
                });
            
            var jobKey = new JobKey("WorkerIntegrationRequestJob");
            
            var options = _configuration.GetSection("WorkerIntegrationService").Get<WorkerIntegrationConfiguration>();
            
            q.AddJob<WorkerJob>(opts => opts.WithIdentity(jobKey));
            
            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("WorkerIntegrationRequestJob-trigger")
                .WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromSeconds(options!.RequestInterval))
                    .RepeatForever()));
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        services.AddSingleton<WorkerIntegrationClient>(serviceProvider =>
        {
            var workerIntegrationConfiguration =
                _configuration.GetSection("WorkerIntegrationService").Get<WorkerIntegrationConfiguration>();
            var logger = serviceProvider.GetRequiredService<ILogger<WorkerIntegrationClient>>();

            return new WorkerIntegrationClient(workerIntegrationConfiguration!, logger);
        });
        
        var employeeServiceConfiguration = 
            _configuration.GetSection("EmployeeService").Get<EmployeeClientConfiguration>();
        services.AddSingleton<EmployeeServiceClient>(
            _ => new EmployeeServiceClient(employeeServiceConfiguration!));
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
    }
}