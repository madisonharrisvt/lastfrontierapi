using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LastFrontierApi.Models
{
    public class Skill
    {
        public Guid SkillId { get; set; }
        public int NameValue { get; set; }
        public int MasteryLevel { get; set; }

        public Guid CharacterId { get; set; }
        public Character Character { get; set; }
    }
}
