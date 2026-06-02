using EMS.Models;

namespace EMS.ViewModels
{
    public class UploadDocumentViewModel
    {
        public int EmployeeId { get; set; }
        public DocumentType DocumentType { get; set; }
        public IFormFile File { get; set; }
    }
}
