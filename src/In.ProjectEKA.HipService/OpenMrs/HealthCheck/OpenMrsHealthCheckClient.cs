using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using In.ProjectEKA.HipService.Logger;
using In.ProjectEKA.HipService.OpenMrs.HealthCheck;

namespace In.ProjectEKA.HipService.OpenMrs {
    public class OpenMrsHealthCheckClient : IHealthCheckClient {
        Dictionary<string, string> endpoints;
        IOpenMrsClient openMrsClient;

        public OpenMrsHealthCheckClient (Dictionary<string, string> initEndpoints, IOpenMrsClient client) {
            endpoints = initEndpoints;
            openMrsClient = client;
        }

        public async Task<Dictionary<string, string>> CheckHealth () {
            Dictionary<string, string> result = new Dictionary<string, string> ();
            foreach (var entry in endpoints) {
                try {
                    var response = await openMrsClient.GetAsync (entry.Value);
                    if (response.StatusCode == HttpStatusCode.OK) {
                        result.Add (entry.Key, "Healthy");
                    } else {
                        result.Add (entry.Key, "Unhealthy");
                    }
                } catch (Exception e) {
                    result.Add (entry.Key, "Unhealthy");
                    Console.WriteLine (e);
                }
            }
            return result;
        }

    }
}