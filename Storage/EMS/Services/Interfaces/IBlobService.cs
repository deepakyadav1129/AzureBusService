using EMS.Models;

namespace EMS.Services.Interfaces
{
    public interface IBlobService
    {
        Task<string> UploadAsync(IFormFile file, DocumentType documentType, CancellationToken ct);
        Task DeleteAsync(string fileName, DocumentType documentType, CancellationToken ct);
        Task AppendLogAsync(string message, CancellationToken cancellationToken);
        Task<string> GenerateReadSasUrlAsync(string fileName, DocumentType document, CancellationToken cancellationToken);
    }
}
