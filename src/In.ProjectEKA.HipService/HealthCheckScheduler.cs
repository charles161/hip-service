using System;
public static class HealthCheckScheduler
{
    public static void IntervalInSeconds(int hour, int sec, double interval, Action task)
    {
        interval = interval/3600;
        SchedulerService.Instance.ScheduleTask(hour, sec, interval, task);
    }

            Timer timer = new Timer(Convert.ToInt64(Environment.GetEnvironmentVariable("HEALTH_CHECK_DURATION")));
            healthCheckScheduler.IntervalInSeconds(11, 10, 15,
            () => {
                Console.WriteLine("//here write the code that you want to schedule");
            });
            timer.Elapsed += async ( sender, e ) => await UpdateHealthDetails();
            timer.Start();
}