using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DattingApp.api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DattingApp.api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // changer CreateHostBuilder(args).Build().run :
            var host = CreateHostBuilder(args).Build();
            // using pour liberer après création
                                // CreateScope pour l'injection de dépendance  
            using(var scope = host.Services.CreateScope())
            {
                //-------- le but est d'injecter un service pour entrer les données
                //-------- de userSeedData en passant par la methode statique
                //-------- SeedUsers de Seed
                //ServiceProvider permet l'injection de service depuis le main
                var services = scope.ServiceProvider;
                try
                {
                    //appel du service pour injection des données dans la base
                    var context = services.GetRequiredService<DataContext>();
                    // appel de Migrate pour modif database
                    context.Database.Migrate();
                    // appel de la méthode statique pour insertion
                    Seed.SeedUsers(context);
                }
                catch(Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occured during the migration process");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
