using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LastFrontierApi.Extensions
{
  public static class PuzzleExtensions
  {
    public static void Shuffle<T>(this IList<T> list)
    {
      var n = list.Count;
      while (n > 1)
      {
        n--;
        var k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
        var value = list[k];
        list[k] = list[n];
        list[n] = value;
      }
    }

    public static class ThreadSafeRandom
    {
      [ThreadStatic] private static Random _local;

      public static Random ThisThreadsRandom
      {
        get { return _local ?? (_local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
      }
    }
  }
}
