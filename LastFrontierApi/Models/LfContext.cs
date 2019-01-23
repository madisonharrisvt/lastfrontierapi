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
    public DbSet<Culture> tblCulture { get; set; }
    public DbSet<Event> tblEvent { get; set; }
    public DbSet<NpcShift> tblNpcShift { get; set; }
    public DbSet<PlayerNpcShifts> tblPlayerNpcShifts { get; set; }
    public DbSet<Cart> tblCart { get; set; }
    public DbSet<CartItem> tblCartItem { get; set; }
    public DbSet<HackingPuzzleRow> tblHackingPuzzleRow { get; set; }
    public DbSet<HackingPuzzle> tblHackingPuzzle { get; set; }
    public DbQuery<NpcShiftWithPlayerCount> Sp_GetNpcShiftsWithPlayerCount { get; set; }
    public DbQuery<PlayerId> Sp_GetPlayersWithoutNpcShiftForActiveEvent { get; set; }
  }
}