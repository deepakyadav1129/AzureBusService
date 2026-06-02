using EmployeeProgress.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeProgress.Functions
{
    public class EmployeeReport
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<EmployeeReport> _logger;

        public EmployeeReport(ApplicationDbContext dbContext, ILogger<EmployeeReport> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        [Function("EmployeeReportTimer")]
        [BlobOutput("reports/employee-report-{DateTime:yyyy-MM-dd-HH-mm-ss}.txt", Connection = "AzureWebJobsStorage")]
        public string Run([TimerTrigger("0 */1 * * * *")] TimerInfo timer)
        {
            _logger.LogInformation($"Employee report generation triggered at: {DateTime.Now}");
            var totalEmployee = _dbContext.EmployeeRequests.Count();
           decimal totalSalary = _dbContext.EmployeeRequests.Sum(e => e.Salary);
            StringBuilder report = new();
            report.AppendLine($"========== Employee Report ==========");
            report.AppendLine($"Employee Report - Generated on {DateTime.Now}");
            report.AppendLine($"Total Employees: {totalEmployee}");
            report.AppendLine($"Total Salary: {totalSalary:C}");
            return report.ToString();
        }
    }
}
