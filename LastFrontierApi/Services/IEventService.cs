namespace LastFrontierApi.Services
{
    public interface IEventService
    {
        void AddCharacterToEvent(int characterId, int eventId);
    }
}
