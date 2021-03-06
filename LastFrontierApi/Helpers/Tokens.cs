﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LastFrontierApi.Auth;
using LastFrontierApi.Models;
using Newtonsoft.Json;

namespace LastFrontierApi.Helpers
{
  public class Tokens
  {
    public static async Task<string> GenerateJwt(ClaimsIdentity identity, IList<string> roles, IJwtFactory jwtFactory,
      string userName, JwtIssuerOptions jwtOptions, JsonSerializerSettings serializerSettings)
    {
      var response = new
      {
        id = identity.Claims.Single(c => c.Type == "id").Value,
        auth_token = await jwtFactory.GenerateEncodedToken(userName, identity, roles),
        expires_in = (int) jwtOptions.ValidFor.TotalSeconds
      };

      return JsonConvert.SerializeObject(response, serializerSettings);
    }
  }
}