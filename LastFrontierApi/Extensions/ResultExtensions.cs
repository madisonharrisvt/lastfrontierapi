using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LastFrontierApi.Extensions
{
  public static class ResultExtensions
  {
    public static T ThrowIfNull<T>(this T item, string name)
    {
      if (item == null)
      {
        throw new Exception($"'{name}' cannot be null!");
      }

      return item;
    }

    public static Result<int> AsInt(this string str)
    {
      var r = new Result<int>();
      try
      {
        var asInt = int.Parse(str);

        r.AsSuccess(asInt);

        return r;
      }
      catch (Exception e)
      {
        r.AsFailure(-1, e.Message);

        return r;
      }
    }

    public class Result<T>
    {
      public bool Success { get; private set; }
      public string Error { get; private set; }
      public T Return { get; private set; }

      public void AsSuccess(T resultObj)
      {
        Success = true;
        Error = string.Empty;
        Return = resultObj;
      }

      public void AsFailure(T resultObj, string message)
      {
        Success = false;
        Error = message;
        Return = resultObj;
      }
    }
  }
}
