using Microsoft.EntityFrameworkCore;
using PlataformaEducacao.GestaoFinanceira.Api.Data;

namespace PlataformaEducacao.GestaoFinanceira.Api.Configuration
{
    public static class DbMigrationHelperExtension
    {
        public static void UseDbMigrationHelper(this WebApplication app)
        {
            DbMigrationHelper.EnsureSeedData(app).Wait();
        }

        public static class DbMigrationHelper
        {
            public static async Task EnsureSeedData(WebApplication application)
            {
                var service = application.Services.CreateScope().ServiceProvider;
                await EnsureSeedData(service);
            }
            public static async Task EnsureSeedData(IServiceProvider serviceProvider)
            {
                using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
                var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

                var financeiroContext = scope.ServiceProvider.GetRequiredService<PagamentosContext>();

                if (env.EnvironmentName == "Development" || env.EnvironmentName == "Testing")
                {
                    await financeiroContext.Database.MigrateAsync();

                }
            }

        }
    }
}