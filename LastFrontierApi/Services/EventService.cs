using System.Linq;
using LastFrontierApi.Models;

namespace LastFrontierApi.Services
{
  public class EventService : IEventService
  {
    private readonly LfContext _context;

    public EventService(LfContext context)
    {
      _context = context;
    }

    public void AddCharacterToEvent(int characterId, int eventId)
    {
      var newCharacterEvent = new CharacterEvent
      {
        CharacterId = characterId,
        EventId = eventId
      };

      _context.Add(newCharacterEvent);
      _context.SaveChanges();
    }

    public Event GetActiveEvent()
    {
      return _context.tblEvent.FirstOrDefault(e => e.IsActiveEvent);
    }
  }
}