using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.IntegrationTests.Common;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove ALL Entity Framework related services
            var descriptorsToRemove = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<LibraryDbContext>) ||
                d.ServiceType == typeof(DbContextOptions) ||
                d.ServiceType.FullName?.Contains("EntityFrameworkCore") == true ||
                d.ServiceType.FullName?.Contains("Npgsql") == true ||
                d.ImplementationType?.FullName?.Contains("EntityFrameworkCore") == true ||
                d.ImplementationType?.FullName?.Contains("Npgsql") == true
            ).ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

            var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(LibraryDbContext));
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            // Use SAME database name for all requests in this factory instance
            services.AddDbContext<LibraryDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            // Mock message publisher
            var messagePublisherDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IMessagePublisher));
            if (messagePublisherDescriptor != null)
            {
                services.Remove(messagePublisherDescriptor);
            }

            var mockMessagePublisher = new Mock<IMessagePublisher>();
            mockMessagePublisher
                .Setup(m => m.PublishAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            mockMessagePublisher
                .Setup(m => m.PublishAsync(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            services.AddSingleton(mockMessagePublisher.Object);

            // Mock blob storage
            var blobStorageDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IBlobStorageService));
            if (blobStorageDescriptor != null)
            {
                services.Remove(blobStorageDescriptor);
            }

            var mockBlobStorage = new Mock<IBlobStorageService>();
            mockBlobStorage
                .Setup(b => b.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("https://test.blob.core.windows.net/test/image.jpg");
            services.AddSingleton(mockBlobStorage.Object);

            // Build and create database
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
            db.Database.EnsureCreated();
        });

        builder.UseEnvironment("Testing");
    }
}