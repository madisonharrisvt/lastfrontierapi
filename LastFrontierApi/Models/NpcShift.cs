using System;

namespace LastFrontierApi.Models
{
  public class NpcShift
  {
    public int? Id { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }

    public int EventId { get; set; }
    public Event Event { get; set; }
  }
}