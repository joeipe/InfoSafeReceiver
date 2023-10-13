using AutoMapper;
using InfoSafeReceiver.Data.Models;
using InfoSafeReceiver.ViewModels;
using SharedKernel.Extensions;
using System.Data;

namespace InfoSafeReceiver.API.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Contact, ContactVM>()
                .ForMember(dest => dest.DoB, opt => opt.MapFrom(src => src.DoB.ParseDate()));
        }
    }
}
