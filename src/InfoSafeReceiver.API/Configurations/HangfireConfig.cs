using Hangfire;
using Hangfire.SqlServer;
using Hangfire.Storage;
using InfoSafeReceiver.Application.Jobs;
using Microsoft.AspNetCore.Http;
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
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 3, DelaysInSeconds = new int[] { 1, 5, 100 } });
            services.AddHangfireServer();
        }

        public static void ApplyHangfire(this IApplicationBuilder app)
        {
            var options = new DashboardOptions
            {
                AppPath = "/swagger"
            };
            app.UseHangfireDashboard("/hangfire", options);

            RegisterHangfireRecurringJobs();
        }

        private static void RegisterHangfireRecurringJobs()
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
            var type = RecurringJobTypes.SingleOrDefault(x => x == typeof(T));
            if (type == null)
            {
                throw new InvalidOperationException("new recurring job must be added to the RecurringJobTypes collection before it can be registered");
            }
            RecurringJob.AddOrUpdate<T>(type.Name, x => x.ExecuteAsync(JobCancellationToken.Null), cron);
        }
    }
}