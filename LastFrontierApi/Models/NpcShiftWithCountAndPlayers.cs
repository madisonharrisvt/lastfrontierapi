using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LastFrontierApi.Models
{
    public class NpcShiftWithCountAndPlayers
    {
        public int EventId { get; set; }
        public int Id { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int NpcCount { get; set; }
        public List<Player> Players { get; set; }
    }
}
