using AutoMapper;
using InfoSafeReceiver.Data.Models;
using InfoSafeReceiver.ViewModels;
using SharedKernel.Extensions;
using System.Data;

namespace InfoSafeReceiver.API.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<ContactVM, Contact>()
                .ForMember(dest => dest.DoB, opt => opt.MapFrom(src => src.DoB.ParseDate()));
        }
    }
}
