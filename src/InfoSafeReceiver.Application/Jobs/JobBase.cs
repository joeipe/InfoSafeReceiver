using Hangfire;

namespace InfoSafeReceiver.Application.Jobs
{
    public abstract class JobBase
    {
        public abstract Task ExecuteAsync(IJobCancellationToken cancellationToken);
    }
}