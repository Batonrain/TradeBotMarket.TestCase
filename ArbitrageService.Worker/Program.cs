using ArbitrageService.Core.Interfaces;
using ArbitrageService.Infrastructure.Data;
using ArbitrageService.Infrastructure.Repositories;
using ArbitrageService.Infrastructure.Services;
using ArbitrageService.Worker.Jobs;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/worker-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IPriceDifferenceRepository, PriceDifferenceRepository>();
            services.AddScoped<IBinanceService, BinanceService>();
            services.AddHttpClient<IBinanceService, BinanceService>();

            services.AddQuartz(q =>
            {
                var jobKey = new JobKey("PriceDifferenceJob");
                q.AddJob<PriceDifferenceJob>(opts => opts.WithIdentity(jobKey));
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("PriceDifferenceJob-trigger")
                    .WithCronSchedule("0 * * * * ?"));
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        })
        .UseSerilog()
        .Build();

    Log.Information("Starting Worker service");
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Worker service terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
} 