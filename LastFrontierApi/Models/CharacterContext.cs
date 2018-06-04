using Microsoft.EntityFrameworkCore;

namespace LastFrontierApi.Models
{
    public class CharacterContext : DbContext
    {
        public CharacterContext(DbContextOptions<CharacterContext> options)
            : base(options)
        {
        }

        public DbSet<Character> tblCharacter { get; set; }
    }
}
