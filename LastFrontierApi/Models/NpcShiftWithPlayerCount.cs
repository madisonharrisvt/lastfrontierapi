using System;

namespace LastFrontierApi.Models
{
    public class NpcShiftWithPlayerCount
    {
        public int EventId { get; set; }
        public int Id { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int NpcCount { get; set; }
    }
}
