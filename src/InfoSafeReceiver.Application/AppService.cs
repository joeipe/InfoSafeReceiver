using AutoMapper;
using InfoSafeReceiver.Data.Models;
using InfoSafeReceiver.Data.Repositories.Interfaces;
using InfoSafeReceiver.ViewModels;

namespace InfoSafeReceiver.Application
{
    public class AppService : IAppService
    {
        private readonly IMapper _mapper;
        private readonly IContactRepository _contactRepository;

        public AppService(
            IMapper mapper,
            IContactRepository contactRepository)
        {
            _mapper = mapper;
            _contactRepository = contactRepository;
        }

        public async Task<List<ContactVM>> GetContactsAsync()
        {
            var data = await _contactRepository.GetContactsAsync();
            var vm = _mapper.Map<List<ContactVM>>(data);
            return vm;
        }

        public async Task<List<ContactVM>> GetContactsByIdAsync(int id)
        {
            var data = await _contactRepository.GetContactsByIdAsync(id);
            var vm = _mapper.Map<List<ContactVM>>(data);
            return vm;
        }

        public async Task AddContactAsync(ContactVM value)
        {
            var data = _mapper.Map<Contact>(value);
            _contactRepository.Create(data);
            await _contactRepository.SaveAsync();
        }
    }
}