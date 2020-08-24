using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using In.ProjectEKA.HipService.OpenMrs;
using In.ProjectEKA.HipService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using In.ProjectEKA.HipService.OpenMrs.HealthCheck;

public class HealthCheckStatus : IHealthCheckStatus
{
    Dictionary<string, Dictionary<string, string>> statusData = new Dictionary<string, Dictionary<string, string>>();
    public void AddStatus(string key, Dictionary<string, string> value)
    {
        if (statusData.ContainsKey(key))
        {
            statusData[key] = value;
        }
        else
        {
            statusData.Add(key, value);
        }
    }

    public Dictionary<string, string> GetStatus(string key)
    {
        if (statusData.ContainsKey(key))
        {
            return statusData[key];
        }
        return null;
    }

}

