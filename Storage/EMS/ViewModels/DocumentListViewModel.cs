using EMS.Models;

namespace EMS.ViewModels
{
    public class DocumentListViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string OriginalFileName { get; set; }
        public DocumentType DocumentType { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedOn { get; set; }
        public string DownloadUrl { get; set; }

    }
}
