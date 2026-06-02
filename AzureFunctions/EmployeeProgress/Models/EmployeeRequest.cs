using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeProgress.Models
{
    public class EmployeeRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public decimal Salary { get; set; }
        public string Photo { get; set; }
        public string CreatedDate { get; set; }

    }
}
