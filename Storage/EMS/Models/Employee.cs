namespace EMS.Models
{
    public class Employee : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public ICollection<EmployeeDocument> Documensts { get; set; } = new List<EmployeeDocument>();
    }
}
