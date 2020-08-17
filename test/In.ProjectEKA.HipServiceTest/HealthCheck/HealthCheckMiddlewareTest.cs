namespace In.ProjectEKA.HipServiceTest.OpenMrs {
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Net;
    using System.Threading.Tasks;
    using System.Threading;
    using System;
    using FluentAssertions;
    using In.ProjectEKA.HipService.OpenMrs;
    using In.ProjectEKA.HipService;
    using Microsoft.AspNetCore.Http;
    using Moq.Protected;
    using Moq;
    using Newtonsoft.Json;
    using Xunit;

    [Collection ("Health Check Middleware Tests")]
    public class HealthCheckMiddlewareTest {

        private Mock<IHealthCheckClient> healthCheckClient;
        private HealthCheckMiddleware healthCheckMiddleWare;

        public HealthCheckMiddlewareTest () {
            healthCheckMiddleWare = new HealthCheckMiddleware (async (innerHttpContext) => {
                innerHttpContext.Response.StatusCode = 200;
                await innerHttpContext.Response.WriteAsync ("Success");
            });
        }

        [Fact]
        private async void ShouldReturnStatus500WithDetailsEvenIfOneServiceIsUnhealthy () {
            var sampleServiceData = new Dictionary<string, string> () { { "Service1", "Unhealthy" }, { "Service2", "Healthy" } };
            healthCheckClient = new Mock<IHealthCheckClient> ();
            healthCheckClient.Setup (x => x.CheckHealth ())
                .Returns (Task.FromResult (sampleServiceData));
            var context = new DefaultHttpContext ();
            context.Response.Body = new MemoryStream ();
            var expectedResult = JsonConvert.SerializeObject (sampleServiceData);

            await healthCheckMiddleWare.Invoke (context, healthCheckClient.Object);
            context.Response.Body.Seek (0, SeekOrigin.Begin);
            var responseBody = new StreamReader (context.Response.Body).ReadToEnd ();

            responseBody
                .Should ()
                .BeEquivalentTo (expectedResult);
            context.Response.StatusCode
                .Should ()
                .Be (500);
        }

        [Fact]
        private async void ShouldReturnStatus200WithDetailsIfAllTheServicesAreHealthy () {
            var sampleServiceData = new Dictionary<string, string> () { { "Service1", "Healthy" }, { "Service2", "Healthy" } };
            healthCheckClient = new Mock<IHealthCheckClient> ();
            healthCheckClient.Setup (x => x.CheckHealth ())
                .Returns (Task.FromResult (sampleServiceData));
            var context = new DefaultHttpContext ();
            context.Response.Body = new MemoryStream ();
            var expectedResult = "Success";

            await healthCheckMiddleWare.Invoke (context, healthCheckClient.Object);
            context.Response.Body.Seek (0, SeekOrigin.Begin);
            var responseBody = new StreamReader (context.Response.Body).ReadToEnd ();

            responseBody
                .Should ()
                .BeEquivalentTo (expectedResult);
            context.Response.StatusCode
                .Should ()
                .Be (200);
        }
    }
}