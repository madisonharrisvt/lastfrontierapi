using System;
using LastFrontierApi.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LastFrontierApi
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var host = BuildWebHost(args);

      using (var scope = host.Services.CreateScope())
      {
        var services = scope.ServiceProvider;

        try
        {
          var context = services.GetRequiredService<LfContext>();
          context.Database.Migrate();
          SeedData.Initialize(services);
        }
        catch (Exception e)
        {
          Console.WriteLine(e);
          throw;
        }
      }

      host.Run();
    }

    public static IWebHost BuildWebHost(string[] args)
    {
      return WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>()
        .Build();
    }
  }
}