using InfoSafeReceiver.Data.Models;

namespace InfoSafeReceiver.Data.Repositories.Interfaces
{
    public interface IContactRepository : IGenericRepository<Contact>
    {
        Task<List<Contact>> GetContactsAsync();

        Task<List<Contact>> GetContactsByIdAsync(int id);
    }
}