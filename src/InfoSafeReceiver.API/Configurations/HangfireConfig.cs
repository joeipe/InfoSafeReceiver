using Hangfire;
using Hangfire.SqlServer;
using Hangfire.Storage;
using InfoSafeReceiver.Application.Jobs;
using Microsoft.EntityFrameworkCore;

namespace InfoSafeReceiver.API.Configurations
{
    public static class HangfireConfig
    {
        private static readonly Type[] RecurringJobTypes = {
            typeof(AddContactJob)
        };

        public static void AddHangfireConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddHangfire(x => x
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration.GetConnectionString("DBConnectionString"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));
            services.AddHangfireServer();
        }

        public static void ApplyHangfire(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard();
        }

        public static void RegisterHangfireRecurringJobs(this IApplicationBuilder app)
        {
            var jobIds = RecurringJobTypes.Select(x => x.Name).ToHashSet();
            using (var connection = JobStorage.Current.GetConnection())
            {
                foreach (var recurringJob in connection.GetRecurringJobs().Where(recurringJob => !jobIds.Contains(recurringJob.Id)))
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }

            RegisterRecurringJob<AddContactJob>(Cron.Yearly());
        }

        private static void RegisterRecurringJob<T>(string cron) where T : JobBase
        {
            RecurringJob.AddOrUpdate<T>(x => x.ExecuteAsync(), cron);
        }
    }
}