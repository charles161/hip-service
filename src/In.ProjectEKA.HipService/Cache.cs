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

public class Cache: ICache {
    Dictionary<string,object> cacheData = new Dictionary<string, object>();
    public void add(string key, object value){
        if(cacheData.ContainsKey(key)){
            cacheData[key]=value;
            Console.WriteLine("added to cache "+key);
        }else {
            cacheData.Add(key,value);
            Console.WriteLine("updated to cache "+key);
        }
    }
    public void remove(string key){
        if(cacheData.ContainsKey(key)){
            cacheData.Remove(key);
        }
    }

    public object get(string key){
        if(cacheData.ContainsKey(key)){
            return cacheData[key];
        }
        return null;
    }        
    
    public void clear(){
        cacheData.Clear();
    }
}

