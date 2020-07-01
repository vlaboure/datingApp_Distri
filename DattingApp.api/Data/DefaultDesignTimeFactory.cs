using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DattingApp.api.Data   
{
    // public class DefaultDesignTimeFactory : IDesignTimeDbContextFactory<DataContext>
    // {
    //     public DataContext CreateDbContext(string[] args)
    //     {
    //         string path = Directory.GetCurrentDirectory();

    //         IConfigurationBuilder builder = new ConfigurationBuilder()
    //                            .SetBasePath(path)
    //                            .AddJsonFile("appsettings.json");

    //         IConfigurationRoot config = builder.Build();
    //         string connectionString = config.GetConnectionString("DefaultConnection");
    //         DbContextOptionsBuilder<DataContext> optionBuilder = new DbContextOptionsBuilder<DataContext>();
    //         optionBuilder.UseSqlServer(connectionString);

    //         return new DataContext(optionBuilder.Options);
    //     }
    // }
}