namespace Employee.Service;

public class Program
{
    public static void Main(string[] args)
    {
        Console.Title = "Employee.Service gRpc Host";
        CreateHostBuilder(args).Build().Run();
    }
    
    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host
            .CreateDefaultBuilder(args)
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureAppConfiguration((_, config) =>
            {
                config.AddEnvironmentVariables();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }

}