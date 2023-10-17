using InfoSafeReceiver.API.Messages;
using InfoSafeReceiver.Application;
using InfoSafeReceiver.ViewModels;

namespace InfoSafeReceiver.API.Services
{
    public class MessagingService
    {
        private readonly IAppService _appService;

        public MessagingService(
            IAppService appService)
        {
            _appService = appService;
        }

        public async Task AddContactAsync(ContactMessage value)
        {
            var vm = new ContactVM
            {
                Id = 0,
                RefId = value.Id,
                FirstName = value.FirstName,
                LastName = value.LastName,
                DoB = value.DoB
            };
            await _appService.AddContactAsync(vm);
        }
    }
}