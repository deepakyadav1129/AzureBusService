
namespace EMS.Models
{
    public class EmployeeDocument : BaseEntity
    {
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
        public DocumentType DocumentType { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public string BlobUrl { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;

        //public static implicit operator EmployeeDocument(EmployeeDocument v)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
