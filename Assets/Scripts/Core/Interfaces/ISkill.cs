namespace CardBattle.Core
{
    public interface ISkill : ICard
    {
        SkillTiming Timing { get; }
        TargetType Target { get; }
        void Execute(BattleContext context, Character caster, Character target);
        bool CanExecute(BattleContext context, Character caster);
    }
}