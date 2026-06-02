using EmployeeProgress.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EmployeeProgress.Functions;

public class OnEmployeeUploadWriteToQueue
{
    private readonly ILogger<OnEmployeeUploadWriteToQueue> _logger;

    public OnEmployeeUploadWriteToQueue(ILogger<OnEmployeeUploadWriteToQueue> logger)
    {
        _logger = logger;
    }

    [Function("OnEmployeeUploadWriteToQueue")]
    [QueueOutput("employee-request-inbound", Connection="AzureWebJobsStorage")] // in which queue i want to write the message, and which connection string to use
    public async Task<EmployeeRequest> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            //PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            //WriteIndented = true
        };
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        EmployeeRequest employeeRequest = JsonSerializer.Deserialize<EmployeeRequest>(requestBody,options);
        return employeeRequest ?? new EmployeeRequest();

        //_logger.LogInformation("C# HTTP trigger function processed a request.");
        //return new OkObjectResult("Welcome to Azure Functions!");
    }
}