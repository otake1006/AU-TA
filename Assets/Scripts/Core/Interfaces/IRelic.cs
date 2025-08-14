namespace CardBattle.Core
{
    public interface IRelic : ICard
    {
        RelicTrigger Trigger { get; }
        void OnTrigger(BattleContext context, Character owner, TriggerEvent evt);
        bool IsActive { get; set; }
        int RemainingUses { get; set; }
        void Initialize();
    }
}