namespace InfoSafeReceiver.Application.Jobs
{
    public abstract class JobBase
    {
        public abstract Task ExecuteAsync();
    }
}