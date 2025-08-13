namespace TacticalCardGame.Core
{
    public interface ISkillCard : ICard
    {
        SkillType SkillType { get; }
        int Power { get; }
        TargetType Target { get; }
    }
}