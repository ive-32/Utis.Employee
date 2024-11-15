using Employee.Proto;
using Quartz;
using WorkerIntegrationMoq.Configurations;
using WorkerIntegrationMoq.Jobs;

namespace WorkerIntegrationMoq;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<WorkerIntegrationConfiguration>(
            builder.Configuration.GetSection("WorkerIntegrationConfiguration"));
        builder.Services.Configure<AddWorkerActionsJobConfiguration>(
            builder.Configuration.GetSection("AddWorkerActionsJobConfiguration"));
        
        builder.Services.AddGrpc();
        builder.Services.AddGrpcClient<EmployeeService.EmployeeServiceClient>(
            options => options.Address = builder.Configuration.GetSection("EmployeeServiceAddress").Get<Uri>());
        
        builder.Services.AddQuartz(q =>
        {
            q.UseDefaultThreadPool(tp =>
            {
                tp.MaxConcurrency = 1; 
            });

            var jobKey = new JobKey("AddWorkerActionsJob"); 
            q.AddJob<AddWorkerActionsJob>(opts => opts.WithIdentity(jobKey));
            
            var addWorkersInterval = builder.Configuration.GetSection("AddWorkerActionsJobConfiguration")
                .Get<AddWorkerActionsJobConfiguration>();
            
            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("AddWorkerActionsJob-trigger")
                .WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromSeconds(addWorkersInterval!.AddWorkersInterval))
                    .RepeatForever()));
        });

        builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = false); 
        var app = builder.Build();

        app.MapGrpcService<Services.WorkerIntegrationMoq>();
        
        app.Run();
    }
}