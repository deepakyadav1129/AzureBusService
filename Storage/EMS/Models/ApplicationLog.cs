namespace EMS.Models
{
    public class ApplicationLog : BaseEntity
    {
        public string Message { get; set; } = string.Empty;
        public string LogLevel { get; set; } = string.Empty;
    }
}
