namespace LastFrontierApi.Models
{
  public class PlayerNpcShifts
  {
    public int? Id { get; set; }

    public int NpcShiftId { get; set; }
    public NpcShift NpcShift { get; set; }

    public int PlayerId { get; set; }
  }
}