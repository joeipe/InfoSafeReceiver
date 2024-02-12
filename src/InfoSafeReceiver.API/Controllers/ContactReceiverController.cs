using Hangfire;
using InfoSafeReceiver.Application;
using InfoSafeReceiver.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace InfoSafeReceiver.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ContactReceiverController : ControllerBase
    {
        private readonly ILogger<ContactReceiverController> _logger;
        private readonly IAppService _appService;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public ContactReceiverController(
            ILogger<ContactReceiverController> logger,
            IAppService appService,
            IBackgroundJobClient backgroundJobClient)
        {
            _logger = logger;
            _appService = appService;
            _backgroundJobClient = backgroundJobClient;
        }

        [HttpGet]
        public async Task<ActionResult> GetContacts()
        {
            return Ok(await _appService.GetContactsAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetContactsById(int id)
        {
            var vm = await _appService.GetContactsByIdAsync(id);

            if (vm == null)
            {
                return NotFound();
            }

            return Ok(vm);
        }

        [HttpPost]
        public async Task<ActionResult> AddContact([FromBody] ContactVM value)
        {
            await _appService.AddContactAsync(value);
            return Ok();
        }

        [HttpPost]
        public ActionResult AddContactDelayed([FromBody] ContactVM value)
        {
            var jobId = _backgroundJobClient.Enqueue<IAppService>(x => x.AddContactDelayedAsync(value));

            // Fire-and-forget
            //var jobId = BackgroundJob.Enqueue<IAppService>(x => x.AddContactDelayedAsync(value));

            //Delayed
            //jobId = BackgroundJob.Schedule<IAppService>(x => x.AddContactDelayedAsync(value), TimeSpan.FromMinutes(5));

            return Ok();
        }
    }
}