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

        public DbSet<Skill> tblCharacterSkills { get; set; }
    }
}
