using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using In.ProjectEKA.HipService.OpenMrs;
using In.ProjectEKA.HipService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using System.Timers;
using Microsoft.AspNetCore.Hosting;
using In.ProjectEKA.HipService.OpenMrs.HealthCheck;

    public class HealthChecker {
        private IHealthCheckClient healthCheckClient;
        private Timer timer;

        private IHealthCheckStatus healthCheckStatus;

        public HealthChecker (IHealthCheckClient initHealthCheckClient, IHealthCheckStatus inithealthCheckStatus) {
            Console.WriteLine("Health checker is being created");
            healthCheckClient = initHealthCheckClient;
            healthCheckStatus = inithealthCheckStatus;
            timer = new Timer(Convert.ToInt32(Environment.GetEnvironmentVariable("HEALTH_CHECK_DURATION")));
            timer.Elapsed += async ( sender, e ) => await UpdateHealthStatus();
            timer.Start();
        }

        public async Task UpdateHealthStatus () {
            Dictionary<string, string> result = await healthCheckClient.CheckHealth(); 
            healthCheckStatus.AddStatus("health",result);
        }

    }
