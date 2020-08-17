namespace In.ProjectEKA.HipServiceTest.OpenMrs {
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net;
    using System.Threading.Tasks;
    using System.Threading;
    using System;
    using FluentAssertions;
    using In.ProjectEKA.HipService.OpenMrs;
    using Moq;
    using Xunit;

    [Collection ("Health Check Client Tests")]
    public class HealthCareClientTest {
        private Mock<IHealthCheckClient> healthCheckClient;

        [Fact]
        private async void ShouldReturnDictionaryWhenCheckingForHealth () {
            healthCheckClient = new Mock<IHealthCheckClient> ();
            healthCheckClient.Setup (x => x.CheckHealth ())
                .Returns (Task.FromResult (new Dictionary<string, string> ()));

            var result = await healthCheckClient.Object.CheckHealth ();

            Assert.True (result.GetType ().Equals (new Dictionary<string, string> ().GetType ()));
        }

    }
}