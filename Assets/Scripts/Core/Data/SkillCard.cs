namespace TacticalCardGame.Core
{
    public class SkillCard : ISkillCard
    {
        public int Id { get; }
        public string Name { get; }
        public CardType Type => CardType.Skill;
        public int Cost { get; }
        public string Description { get; }
        public SkillType SkillType { get; }
        public int Power { get; }
        public TargetType Target { get; }

        public SkillCard(int id, string name, int cost, SkillType skillType, int power, TargetType target, string description = "")
        {
            Id = id;
            Name = name;
            Cost = cost;
            SkillType = skillType;
            Power = power;
            Target = target;
            Description = description;
        }

        public override string ToString() => $"{Name} [{SkillType}] (Cost:{Cost}, Power:{Power})";
    }
}