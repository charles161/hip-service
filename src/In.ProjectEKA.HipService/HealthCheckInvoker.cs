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

    public class HealthCheckInvoker:IDisposable{
        HealthCheckCache healthCheckCache;
        Timer timer;

        public HealthCheckInvoker (HealthCheckCache cache) {
            healthCheckCache=cache;
            timer = new Timer(Convert.ToInt64(Environment.GetEnvironmentVariable("HEALTH_CHECK_DURATION")));
            this.Start();
        }

        public void Start() {
            Console.WriteLine("Hello nan inga iruken");
            //timer.Interval = Convert.ToInt64(Environment.GetEnvironmentVariable("HEALTH_CHECK_DURATION"));
            //timer.Elapsed += new ElapsedEventHandler(Invoke); 
            timer.Elapsed += async ( sender, e ) => await Invoke();
            this.Invoke();
            timer.Start();
            Console.WriteLine("Hello kela vantan");
        }

        public void Dispose(){
            timer.Dispose();
        }

        public async Task Invoke(){
            Console.WriteLine("Nan inga vandha nala irukum");
            healthCheckCache.UpdateHealthDetails();
        }

    }
