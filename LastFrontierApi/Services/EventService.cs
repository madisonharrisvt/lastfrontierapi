using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}
