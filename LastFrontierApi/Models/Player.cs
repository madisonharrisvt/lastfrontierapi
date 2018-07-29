namespace LastFrontierApi.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string IdentityId { get; set; }
        public AppUser Identity { get; set; }
    }
}
