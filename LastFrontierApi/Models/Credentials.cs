using FluentValidation.Attributes;
using LastFrontierApi.Models.Validations;

namespace LastFrontierApi.Models
{
  [Validator(typeof(CredentialsValidator))]
  public class Credentials
  {
    public string UserName { get; set; }
    public string Password { get; set; }
  }
}