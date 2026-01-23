using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Common.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<Stream?> DownloadAsync(string fileName, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string fileName, CancellationToken cancellationToken = default);
    string GetBlobUrl(string fileName);
}
