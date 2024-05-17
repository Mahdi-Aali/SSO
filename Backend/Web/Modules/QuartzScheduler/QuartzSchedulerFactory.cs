using Quartz;
using Web.Modules.QuartzJobs;

namespace Web.Modules.QuartzScheduler;

public class QuartzSchedulerFactory
{
    public static IReadOnlyDictionary<IJobDetail, IReadOnlyCollection<ITrigger>> GetJobsAndTriggers()
    {
        return new Dictionary<IJobDetail, IReadOnlyCollection<ITrigger>>()
        {
            {
                JobBuilder
                .Create<DatabaseSeedDataJob>()
                .WithIdentity("data-base-seed-data-job", "data-base")
                .Build(),

                new List<ITrigger>()
                {
                    TriggerBuilder.Create()
                    .WithIdentity("data-base-seed-data-job-trigger", "data-base")
                    .StartNow()
                    .WithSimpleSchedule(cfg =>
                    {
                        cfg.WithRepeatCount(0);
                    })
                    .Build()
                }
            }
        };
    }
}
