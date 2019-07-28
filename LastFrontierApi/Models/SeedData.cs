using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LastFrontierApi.Models
{
  public static class SeedData
  {
    public static void Initialize(IServiceProvider serviceProvider)
    {
      using (var context = new LfContext(
        serviceProvider.GetRequiredService<DbContextOptions<LfContext>>()))
      {
        //if (context.tblCharacter.Any())
        //{
        //    return;
        //}
        //else
        //{
        //    throw new Exception();
        //}
      }
    }
  }
}