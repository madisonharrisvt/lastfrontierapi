using System.Collections.Generic;

namespace LastFrontierApi.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string IdentityId { get; set; }
        public AppUser Identity { get; set; }
        public int? VolunteerPoints { get; set; }
    }
}
