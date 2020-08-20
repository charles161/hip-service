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

    public class HealthChecker {
        private List<IHealthCheckClient> healthCheckClients;

        private Cache cache;

        public HealthChecker (List<IHealthCheckClient> initHealthCheckClients, Cache initCache, Timer timer) {
            healthCheckClients = initHealthCheckClients;
            cache = initCache;
            timer.Elapsed += async ( sender, e ) => await UpdateHealthInCache();
            timer.Start();
        }

        public async Task UpdateHealthInCache () {
            Console.WriteLine("updating health cache");
            Dictionary<string, string> result = new Dictionary<string, string> ();
            foreach (IHealthCheckClient client in healthCheckClients) {
                var response = await client.CheckHealth();
                result=result.Union(response).ToDictionary(pair => pair.Key,pair => pair.Value);
            }
            cache.add("health",result);
        }
    }
