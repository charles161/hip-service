// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using In.ProjectEKA.HipService.OpenMrs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
public class HealthCheckMiddleware {
    private readonly RequestDelegate _next;

    public HealthCheckMiddleware (RequestDelegate next) {
        _next = next;
    }

    public async Task Invoke (HttpContext httpContext,Cache cache) {
        Dictionary<string, string> healthStatus = (Dictionary<string, string>) cache.get("health");
        bool healthy = true;
        if(healthStatus!=null){
            foreach (var entry in healthStatus) {
                if (entry.Value != "Healthy") {
                    healthy = false;
                    break;
                }
            }
        }
        if (!healthy) {
            httpContext.Response.StatusCode = 500;
            await httpContext.Response.WriteAsync (JsonConvert.SerializeObject (healthStatus));
        } else {
            await _next (httpContext);
        }
    }
}

public static class HealthCheckMiddlewareExtensions {
    public static IApplicationBuilder UseHealthCheckMiddleware (this IApplicationBuilder builder) {
        return builder.UseMiddleware<HealthCheckMiddleware> ();
    }
}