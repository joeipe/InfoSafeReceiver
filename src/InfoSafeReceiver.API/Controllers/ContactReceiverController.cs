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

        public ContactReceiverController(
            ILogger<ContactReceiverController> logger,
            IAppService appService)
        {
            _logger = logger;
            _appService = appService;
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
    }
}