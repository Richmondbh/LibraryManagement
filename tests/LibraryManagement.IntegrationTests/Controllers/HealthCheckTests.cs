using FluentAssertions;
using LibraryManagement.IntegrationTests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LibraryManagement.IntegrationTests.Controllers
{
    public class HealthCheckTests : BaseIntegrationTest
    {
        public HealthCheckTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Swagger_ShouldBeAccessible()
        {
            // Act
            var response = await Client.GetAsync("/swagger/index.html");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ApiRoot_ShouldReturnNotFound()
        {
            // Act
            var response = await Client.GetAsync("/");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
