using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LastFrontierApi.Models
{
    public class Character
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int? AccumulatedXP { get; set; }
        public int? AvailableXP { get; set; }
        public int? Species { get; set; }
        public int? StressResponse { get; set; }
        public string HStatus { get; set; }
        public bool? CloneStatus { get; set; }
        public int? Occupation { get; set; }
        public int? SideGig { get; set; }
        public int? Status { get; set; }
        public int? TorsoHealth { get; set; }
        public int? RightArmHealth { get; set; }
        public int? LeftArmHealth { get; set; }
        public int? RightLegHealth { get; set; }
        public int? LeftLegHealth { get; set; }

        public virtual ICollection<Skill> Skills { get; set; }
    }
}
