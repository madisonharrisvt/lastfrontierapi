using LastFrontierApi.Models.Metadata;
using Microsoft.EntityFrameworkCore;

namespace LastFrontierApi.Models
{
    public class LfContext : DbContext
    {
        public LfContext(DbContextOptions<LfContext> options)
            : base(options)
        {
        }

        public DbSet<Character> tblCharacter { get; set; }

        public DbSet<CharacterSkill> tblCharacterSkills { get; set; }

        public DbSet<Occupation> tblOccupation { get; set; }
        public DbSet<SideGig> tblSideGig { get; set; }
        public DbSet<Skill> tblSkill { get; set; }
        public DbSet<CharacterEvent> tblCharacterEvents { get; set; }
        public DbSet<Species> tblSpecies { get; set; }
        public DbSet<Status> tblStatus { get; set; }
        public DbSet<StressResponse> tblStressResponse { get; set; }

        public DbSet<Event> tblEvent { get; set; }
    }
}
