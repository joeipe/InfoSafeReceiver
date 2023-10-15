using InfoSafeReceiver.ViewModels;

namespace InfoSafeReceiver.Application
{
    public interface IAppService
    {
        Task<List<ContactVM>> GetContactsAsync();

        Task<List<ContactVM>> GetContactsByIdAsync(int id);

        Task AddContactAsync(ContactVM value);
    }
}