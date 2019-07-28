namespace LastFrontierApi.Models
{
  public class CharacterEvent
  {
    public int Id { get; set; }
    public int VpToXp { get; set; }
    public int VpToItems { get; set; }
    public int XpBought { get; set; }

    public int EventId { get; set; }
    public Event Event { get; set; }

    public int CharacterId { get; set; }
    public Character Character { get; set; }
  }
}