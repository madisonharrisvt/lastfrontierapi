using LastFrontierApi.Models;

namespace LastFrontierApi.Services
{
  public interface IEventService
  {
    void AddCharacterToEvent(int characterId, int eventId);
    Event GetActiveEvent();
  }
}