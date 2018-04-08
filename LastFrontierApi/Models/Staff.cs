namespace LastFrontierApi.Models
{
    public class Staff
    {
        public int Id { get; set; }
        public string IdentityId { get; set; }
        public AppUser Identity { get; set; }
        public string Location { get; set; }
        public string Locale { get; set; }
    }
}
