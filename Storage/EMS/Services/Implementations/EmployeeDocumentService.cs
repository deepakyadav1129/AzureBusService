using EMS.Data;
using EMS.Helders;
using EMS.Models;
using EMS.Services.Interfaces;
using EMS.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EMS.Services.Implementations
{
    public class EmployeeDocumentService : IEmployeeDocumentService
    {
        private readonly IBlobService _blobService;
        private readonly ApplicationDbContext _dbContext;

        public EmployeeDocumentService(IBlobService blobService, ApplicationDbContext dbContext)
        {
            _blobService = blobService;
            _dbContext = dbContext;
        }

        public async Task DeleteDocumentAsync(int documentId, CancellationToken ct)
        {
           EmployeeDocument? document = await _dbContext.EmployeeDocuments.FirstOrDefaultAsync(x => x.Id == documentId, ct);
            if(document == null)
            {
                throw new InvalidOperationException("Document not found.");
            }
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
            try
            {
                _dbContext.EmployeeDocuments.Remove(document);
                await _dbContext.SaveChangesAsync(ct);
                await _blobService.DeleteAsync(document.StoredFileName, document.DocumentType, ct);
                await transaction.CommitAsync(ct);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        public async Task<List<ViewModels.DocumentListViewModel>> GetDocumentsAsync(int employeeId, CancellationToken ct)
        {
            List<DocumentListViewModel> documents = await _dbContext.EmployeeDocuments
                .Where(x => x.EmployeeId == employeeId)
                .Select(x => new DocumentListViewModel
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId,
                    OriginalFileName = x.OriginalFileName,
                    DocumentType = x.DocumentType,
                    FileSize = x.FileSize,
                    UploadedOn = x.CreatedAtUtc,
                    DownloadUrl = string.Empty
                })
                .ToListAsync(ct);
            foreach (var doc in documents)
            {
                EmployeeDocument? entity = await _dbContext.EmployeeDocuments.FirstOrDefaultAsync(x => x.Id == doc.Id, ct);
                if (entity != null)
                {
                    doc.DownloadUrl = await _blobService.GenerateReadSasUrlAsync(entity.StoredFileName, entity.DocumentType, ct);
                }
            }
            return documents;
        }

        public async Task UploadDocumentAsync(ViewModels.UploadDocumentViewModel viewModel, CancellationToken ct)
        {
           var employeeExists = await _dbContext.Employees.AnyAsync(e => e.Id == viewModel.EmployeeId, ct);
           if (!employeeExists)
           {
               throw new InvalidOperationException("Employee does not exist.");
           }
           if (!FileValidationHelper.IsValidExtension(viewModel.File.FileName))
           {
                throw new Exception("Invalid file extension."); 
           }
           if (!FileValidationHelper.IsValidFileSize(viewModel.File.Length))
           {
                 throw new Exception("File size exceeds the limit.");
           }
           string blobUrl = await _blobService.UploadAsync(viewModel.File, viewModel.DocumentType ,ct);
            string storedFileName = Path.GetFileName(new Uri(blobUrl).AbsolutePath);
            EmployeeDocument employeeDocument = new EmployeeDocument
            {
                EmployeeId = viewModel.EmployeeId,
                DocumentType = viewModel.DocumentType,
                OriginalFileName = viewModel.File.FileName,
                StoredFileName = storedFileName,
                BlobUrl = blobUrl,
                FileSize = viewModel.File.Length,
                ContentType = viewModel.File.ContentType
            };
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
            try
            {
                await _dbContext.EmployeeDocuments.AddAsync(employeeDocument, ct);
                await _dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(ct);
                await _blobService.DeleteAsync(storedFileName, viewModel.DocumentType, ct); 
                throw;
            }
        }
    }
}
