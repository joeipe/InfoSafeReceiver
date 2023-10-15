using InfoSafeReceiver.Data.Models;
using InfoSafeReceiver.Data.Repositories.Interfaces;

namespace InfoSafeReceiver.Data.Repositories
{
    public class ContactRepository : GenericRepository<Contact>, IContactRepository
    {
        protected InfoSafeReceiverDbContext _dbContext;

        public ContactRepository(
            InfoSafeReceiverDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Contact>> GetContactsAsync()
        {
            var data = await GetAllAsync();
            return data.ToList();
        }

        public async Task<List<Contact>> GetContactsByIdAsync(int id)
        {
            var data = await SearchForAsync
                (
                    s => s.RefId == id
                );

            return data.ToList();
        }
    }
}