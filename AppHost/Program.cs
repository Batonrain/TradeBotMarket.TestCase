using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var apiApp = builder.AddProject<ArbitrageService_Api>("ArbitrageApi");
var workerApp = builder.AddProject<ArbitrageService_Worker>("ArbitrageWorker");

builder.Build().Run();
