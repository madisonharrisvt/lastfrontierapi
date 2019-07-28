using System;
using System.Linq;

namespace LastFrontierApi.Helpers
{
  public class RandomString
  {
    private static readonly Random Random = new Random();

    public static string GetRandomString(int length)
    {
      const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#$%^&*()_+?=-0123456789";
      return new string(Enumerable.Repeat(chars, length)
        .Select(s => s[Random.Next(s.Length)]).ToArray());
    }
  }
}