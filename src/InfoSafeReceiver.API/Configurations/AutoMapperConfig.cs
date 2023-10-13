using InfoSafeReceiver.API.AutoMapper;

namespace InfoSafeReceiver.API.Configurations
{
    public static class AutoMapperConfig
    {
        public static void AddAutoMapperConfiguration(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddAutoMapper(typeof(AutoMapperProfileSetup));

            AutoMapperProfileSetup.RegisterMappings();
        }
    }
}
