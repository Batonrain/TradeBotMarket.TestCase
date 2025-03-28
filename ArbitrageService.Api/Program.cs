using ArbitrageService.Core.Interfaces;
using ArbitrageService.Infrastructure.Data;
using ArbitrageService.Infrastructure.Repositories;
using ArbitrageService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/api-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPriceDifferenceRepository, PriceDifferenceRepository>();
builder.Services.AddScoped<IBinanceService, BinanceService>();
builder.Services.AddHttpClient<IBinanceService, BinanceService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

try
{
    Log.Information("Starting API service");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "API service terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
} 