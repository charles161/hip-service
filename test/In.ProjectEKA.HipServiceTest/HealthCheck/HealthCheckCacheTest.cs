namespace In.ProjectEKA.HipServiceTest.OpenMrs {
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net;
    using System.Threading.Tasks;
    using System.Threading;
    using System;
    using FluentAssertions;
    using In.ProjectEKA.HipService;
    using In.ProjectEKA.HipService.OpenMrs;
    using Moq.Protected;
    using Moq;
    using Xunit;
    using Microsoft.AspNetCore.Hosting;

    [Collection ("Health Check Client Tests")]
    public class HealthCheckCacheTest {
        HealthCheckCache healthCheckCache;
        private Mock<IHealthCheckClient> healthCheckClient;
        HealthCheckInvoker healthCheckInvoker;

        [Fact]
        private void ShouldUpdateHealthWhenUpdateHealthDetailsIsInvoked() {
            Environment.SetEnvironmentVariable("HEALTH_CHECK_DURATION", "5000");

            healthCheckClient = new Mock<IHealthCheckClient> ();
            healthCheckClient.Setup (x => x.CheckHealth ())
                .Returns (Task.FromResult (new Dictionary<string, string> (){{"service","Unhealthy"}}));
            healthCheckCache = new HealthCheckCache(new List<IHealthCheckClient>(){
                healthCheckClient.Object
            });
            //healthCheckCache.UpdateHealthDetails();
            
            healthCheckClient.Setup (x => x.CheckHealth ())
                .Returns (Task.FromResult (new Dictionary<string, string> (){
                    {"service","Healthy"}
                }));

            var firstHealthCheck = healthCheckCache.getHealthDetails();
            healthCheckInvoker = new HealthCheckInvoker(healthCheckCache);
            //healthCheckCache.UpdateHealthDetails();
            var secondHealthCheck = healthCheckCache.getHealthDetails();

            Assert.True(firstHealthCheck["service"].Equals("Unhealthy"));
            Assert.True(secondHealthCheck["service"].Equals("Healthy"));
        }
    }
}