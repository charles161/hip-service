using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using In.ProjectEKA.HipService.OpenMrs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using System.Timers;
using Microsoft.AspNetCore.Hosting;

    public class HealthCheckCache {
        private List<IHealthCheckClient> healthCheckClientList;
        private Dictionary<string,string> healthDetails;

        public HealthCheckCache (List<IHealthCheckClient> initHealthCheckClientList) {
            healthCheckClientList = initHealthCheckClientList;
            Console.WriteLine("Checking for ENV var");
            
        }

        public async Task UpdateHealthDetails () {
            Console.WriteLine("hey hey.... ho ho...");
            Dictionary<string, string> result = new Dictionary<string, string> ();
            foreach (IHealthCheckClient client in healthCheckClientList) {
                var response = await client.CheckHealth();
                result=result.Union(response).ToDictionary(pair => pair.Key,pair => pair.Value);
            }
            healthDetails = result;
        }

        public Dictionary<string,string> getHealthDetails(){
            return healthDetails;
        }

    }
