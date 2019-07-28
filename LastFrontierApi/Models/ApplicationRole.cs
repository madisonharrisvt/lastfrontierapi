using Microsoft.AspNetCore.Identity;

namespace LastFrontierApi.Models
{
  public class ApplicationRole : IdentityRole
  {
    public string RoleName { get; set; }
    public string Description { get; set; }
  }
}