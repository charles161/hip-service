using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using In.ProjectEKA.HipService.OpenMrs;
using Moq;
using Moq.Protected;
using Xunit;

namespace In.ProjectEKA.HipServiceTest.OpenMrs
{
    [Collection("Fhir Gateway Client Tests")]
    public class FhirClientTest
    {
        [Fact(Skip="it is a real call and needs local setup")]
        [Trait("Category", "Infrastructure")]
        public async Task ShouldGetPatientDataRealCallAsync()
        {
            //Given
            // Disable SSL verification in test only
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            var httpClient = new HttpClient(handler);
            var openmrsConfiguration = new OpenMrsConfiguration {
                Url = "https://someurl/openmrs/",
                Username = "someusername",
                Password = "somepassword"
            };
            var openmrsClient = new FhirClient(httpClient, openmrsConfiguration);
            //When
            var response = await openmrsClient.GetAsync("ws/fhir2/Patient");
            //Then
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task ShouldPropagateStatusWhenCredentialsFailed()
        {
            //Given
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            var openmrsConfiguration = new OpenMrsConfiguration
            {
                Url = "https://someurl/openmrs/",
                Username = "someusername",
                Password = "somepassword"
            };
            var openmrsClient = new FhirClient(httpClient, openmrsConfiguration);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                })
                .Verifiable();

            //When
            var response = await openmrsClient.GetAsync("path/to/resource");
            //Then
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ShouldThrowExceptionAndLogIfAnyExceptionIsThrown()
        {
            //Given
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            var openmrsConfiguration = new OpenMrsConfiguration
            {
                Url = "https://someurl/openmrs/",
                Username = "someusername",
                Password = "somepassword"
            };
            var openmrsClient = new FhirClient(httpClient, openmrsConfiguration);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new Exception("Some message here"))
                .Verifiable();

            //When
            Func<Task> getAsyncMethod = async () => { await openmrsClient.GetAsync("path/to/resource"); };
            //Then
            getAsyncMethod.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData("path/to/resource")]
        [InlineData("/path/to/resource")]
        public async Task ShouldInterrogateTheRightDataSource(string patientDiscoveryPath)
        {
            //Given
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            var httpClient = new HttpClient(handlerMock.Object);
            var openmrsConfiguration = new OpenMrsConfiguration
            {
                Url = "https://someurl/openmrs/",
                Username = "someusername",
                Password = "somepassword"
            };
            var openmrsClient = new FhirClient(httpClient, openmrsConfiguration);

            var wasCalledWithTheRightUri = false;
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((response, token) =>
                {
                    if (response.RequestUri.AbsoluteUri == "https://someurl/openmrs/path/to/resource" ||
                        response.RequestUri.AbsoluteUri == "https://someurl/openmrs//path/to/resource")
                    {
                        wasCalledWithTheRightUri = true;
                    }
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                })
                .Verifiable();

            //When
            await openmrsClient.GetAsync(patientDiscoveryPath);
            //Then
            wasCalledWithTheRightUri.Should().BeTrue();
        }
    }
}