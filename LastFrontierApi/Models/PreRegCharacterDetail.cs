namespace LastFrontierApi.Models
{
    public class PreRegCharacterDetail
    {
        public Character Character { get; set; }
        public CartItem CartItem { get; set; }
        public int BaseXp { get; set; }
        public int BaseXpCost { get; set; }
    }
}
