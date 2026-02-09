using PlataformaEducacao.WebApi.Core.Usuario;

namespace PlataformaEducacao.GestaoIdentidade.Api.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddScoped<IAspNetUser, AspNetUser>();

           
            return services;
        }
    }
}
