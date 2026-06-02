using Azure.Monitor.OpenTelemetry.Exporter;
using EmployeeProgress.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using Microsoft.EntityFrameworkCore;

var builder = FunctionsApplication.CreateBuilder(args);
var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});
builder.ConfigureFunctionsWebApplication();

//builder.Services.AddOpenTelemetry()
//    .UseFunctionsWorkerDefaults()
//    .UseAzureMonitorExporter();

builder.Build().Run();
