using LastFrontierApi.Models.Metadata;

namespace LastFrontierApi.Models
{
    public class CharacterMetadata
    {
        public Occupation[] Occupations { get; set; }
        public SideGig[] SideGigs { get; set; }
        public Skill[] Skills { get; set; }
        public Species[] Species { get; set; }
        public Status[] Statuses { get; set; }
        public StressResponse[] StressResponses { get; set; }
    }
}
