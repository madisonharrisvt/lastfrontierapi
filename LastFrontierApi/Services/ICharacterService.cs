using LastFrontierApi.Models;

namespace LastFrontierApi.Services
{
  public interface ICharacterService
  {
    Character UpdateOrCreateCharacter(Character character);
  }
}