using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using LibraryManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Infrastructure.Storage
{
    public class AzureBlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly ILogger<AzureBlobStorageService> _logger;

        public AzureBlobStorageService(IConfiguration configuration, ILogger<AzureBlobStorageService> logger)
        {
            _logger = logger;

            var connectionString = configuration.GetConnectionString("BlobStorage");
            var containerName = configuration["BlobStorage:ContainerName"] ?? "book-covers";

            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogWarning("Blob Storage connection string not configured.");
                _containerClient = null!;
                return;
            }

            var blobServiceClient = new BlobServiceClient(connectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            _containerClient.CreateIfNotExists(PublicAccessType.Blob);
        }

        public async Task<string> UploadAsync(
            Stream fileStream,
            string fileName,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            if (_containerClient == null)
            {
                _logger.LogWarning("Blob Storage not configured. Skipping upload.");
                return string.Empty;
            }

            try
            {
                var blobClient = _containerClient.GetBlobClient(fileName);

                var blobHttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                };

                await blobClient.UploadAsync(
                    fileStream,
                    new BlobUploadOptions { HttpHeaders = blobHttpHeaders },
                    cancellationToken);

                _logger.LogInformation("Uploaded blob: {FileName} to {Uri}", fileName, blobClient.Uri);

                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload blob: {FileName}", fileName);
                throw;
            }
        }

        public async Task<Stream?> DownloadAsync(string fileName, CancellationToken cancellationToken = default)
        {
            if (_containerClient == null)
                return null;

            try
            {
                var blobClient = _containerClient.GetBlobClient(fileName);

                if (!await blobClient.ExistsAsync(cancellationToken))
                    return null;

                var response = await blobClient.DownloadAsync(cancellationToken);
                return response.Value.Content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download blob: {FileName}", fileName);
                return null;
            }
        }

        public async Task<bool> DeleteAsync(string fileName, CancellationToken cancellationToken = default)
        {
            if (_containerClient == null)
                return false;

            try
            {
                var blobClient = _containerClient.GetBlobClient(fileName);
                var response = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

                _logger.LogInformation("Deleted blob: {FileName}, Success: {Success}", fileName, response.Value);

                return response.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete blob: {FileName}", fileName);
                return false;
            }
        }

        public string GetBlobUrl(string fileName)
        {
            if (_containerClient == null)
                return string.Empty;

            var blobClient = _containerClient.GetBlobClient(fileName);
            return blobClient.Uri.ToString();
        }
    }
}
