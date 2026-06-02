using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using EMS.Models;
using EMS.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Xml.Linq;

namespace EMS.Services.Implementations
{
    public class BlobService : IBlobService
    {
        private readonly AzureBlobSettings _settings;
        private readonly BlobServiceClient _blobServiceClient;

        public BlobService(IOptions<AzureBlobSettings> options, BlobServiceClient blobServiceClient)
        {
            _settings = options.Value;
            _blobServiceClient = blobServiceClient;
        }
        public async Task AppendLogAsync(string message, CancellationToken cancellationToken)
        {
            BlobContainerClient containerClient = await GetContainerAsync(_settings.LogContainer);
            AppendBlobClient appendBlobClient = containerClient.GetAppendBlobClient($"logs-{DateTime.UtcNow:yyyy-MM-dd}.txt");
            if (!await appendBlobClient.ExistsAsync(cancellationToken))
            {
                await appendBlobClient.CreateAsync(cancellationToken: cancellationToken);
            }
            string logMessage = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(logMessage);
            await using MemoryStream stream = new MemoryStream(bytes);
            await appendBlobClient.AppendBlockAsync(stream, cancellationToken: cancellationToken);
        }

        public async Task DeleteAsync(string fileName, DocumentType documentType, CancellationToken ct)
        {
           string containerName = GetContainerName(documentType);
            BlobContainerClient containerClient = await GetContainerAsync(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
            //Log append

        }

        public async Task<string> GenerateReadSasUrlAsync(string fileName, DocumentType documentType, CancellationToken cancellationToken)
        {
            string containerName = GetContainerName(documentType);
            BlobContainerClient containerClient = await GetContainerAsync(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            BlobSasBuilder sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = fileName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(10)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);
            Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
            return sasUri.ToString();
        }

        public async Task<string> UploadAsync(IFormFile file, DocumentType documentType, CancellationToken ct)
        {
            string containerName = GetContainerName(documentType);
            BlobContainerClient containerClient = await GetContainerAsync(containerName);
            string extension = Path.GetExtension(file.FileName);
            string storedFileName = $"{Guid.NewGuid()}{extension}";
            BlobClient blobClient = containerClient.GetBlobClient(storedFileName);

            BlobHttpHeaders headers = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            };
            await using Stream stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = headers }, cancellationToken: ct);

            return blobClient.Uri.ToString();
        }

        private async Task<BlobContainerClient> GetContainerAsync(string containerName)
        {
           BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
            return containerClient;
        }

        private string GetContainerName(DocumentType documentType)
        {
            return documentType switch
            {
                DocumentType.Resume => _settings.ResumesContainer,
                DocumentType.Photo => _settings.PhotoContainer,
                DocumentType.Aadhar => _settings.AadharContainer,
                DocumentType.PAN => _settings.PanContainer,
                DocumentType.Payslip => _settings.PayslipContainer,
                DocumentType.Certificate => _settings.CertificateContainer,
                _ => throw new ArgumentOutOfRangeException(nameof(documentType), "Invalid document type")
            };
        }
    }
}
