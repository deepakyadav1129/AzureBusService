using EMS.ViewModels;

namespace EMS.Services.Interfaces
{
    public interface IEmployeeDocumentService
    {
        Task UploadDocumentAsync(UploadDocumentViewModel viewModel, CancellationToken ct);
        Task<List<DocumentListViewModel>> GetDocumentsAsync(int employeeId, CancellationToken ct);
        Task DeleteDocumentAsync(int documentId, CancellationToken ct);
    }
}
