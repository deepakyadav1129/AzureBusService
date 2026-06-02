using EmployeeProgress.Data;
using EmployeeProgress.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EmployeeProgress.Functions
{
    public class ProcessEmployee
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ProcessEmployee> _logger;


        public ProcessEmployee(ApplicationDbContext dbContext, ILogger<ProcessEmployee> logger)
        {
            _dbContext = dbContext;
             _logger = logger;
        }

        [Function("ProcessEmployee")]
        public void Run([QueueTrigger("employee-request-inbound", Connection = "AzureWebJobsStorage")] string queueMessage)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                //PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                //WriteIndented = true
            };
            EmployeeRequest? employeeRequest = JsonSerializer.Deserialize<EmployeeRequest>(queueMessage, options);
            if(employeeRequest== null)
            {
                _logger.LogError($"Failed to deserialize employee request: {queueMessage}");
                return;
            }
            if (employeeRequest != null)
            {
                EmployeeRequest employee = new()
                {
                    Name = employeeRequest.Name,
                    Department = employeeRequest.Department,
                    Salary = employeeRequest.Salary,
                    Photo = employeeRequest.Photo,
                    CreatedDate = employeeRequest.CreatedDate
                };
                _dbContext.EmployeeRequests.Add(employee);
                _dbContext.SaveChanges();
                _logger.LogInformation($"Employee request processed and saved to database: {employee.Id}");
            }
            Console.WriteLine($"Processing employee request: {queueMessage}");
        }
    }
}
