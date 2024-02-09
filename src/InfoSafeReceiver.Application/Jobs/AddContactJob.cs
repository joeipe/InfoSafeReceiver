using Hangfire;
using InfoSafeReceiver.ViewModels;
using Microsoft.Extensions.Logging;
using SharedKernel.Extensions;

namespace InfoSafeReceiver.Application.Jobs
{
    public class AddContactJob : JobBase
    {
        private readonly ILogger<AddContactJob> _logger;
        private readonly IAppService _appService;

        public AddContactJob(
            ILogger<AddContactJob> logger,
            IAppService appService)
        {
            _logger = logger;
            _appService = appService;
        }

        [AutomaticRetry(Attempts = 0)]
        public override async Task ExecuteAsync()
        {
            var scopeInfo = new Dictionary<string, object>();
            scopeInfo.Add("Controller", nameof(AddContactJob));
            scopeInfo.Add("Action", nameof(ExecuteAsync));
            using (_logger.BeginScope(scopeInfo))
                _logger.LogInformation("{ScopeInfo} - {Param}", scopeInfo, DateTime.Now);

            var message = "{\"RefId\":1,\"FirstName\":\"Job\",\"LastName\":\"Cron\",\"DoB\":\"26/04/1981\"}";
            var value = message.OutputObject<ContactVM>();
            await _appService.AddContactDelayedAsync(value);
        }
    }
}