using InfoSafeReceiver.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoSafeReceiver.Application
{
    public interface IAppService
    {
        Task<List<ContactVM>> GetContactsAsync();

        Task<List<ContactVM>> GetContactsByIdAsync(int id);

        Task AddContactAsync(ContactVM value);
    }
}
