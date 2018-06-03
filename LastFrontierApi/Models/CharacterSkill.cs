namespace LastFrontierApi.Models
{
    public class CharacterSkill
    {
        public int Id { get; set; }
        public int SkillId { get; set; }
        public int MasteryLevel { get; set; }

        public int CharacterId { get; set; }
        public Character Character { get; set; }
    }
}
