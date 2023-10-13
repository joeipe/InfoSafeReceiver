using InfoSafeReceiver.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoSafeReceiver.Data.Repositories.Interfaces
{
    public interface IContactRepository : IGenericRepository<Contact>
    {
        Task<List<Contact>> GetContactsAsync();

        Task<List<Contact>> GetContactsByIdAsync(int id);
    }
}
