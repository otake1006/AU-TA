namespace TacticalCardGame.Core
{
    public interface IConditionCard : ICard
    {
        bool CheckCondition(IPlayer player, IPlayer opponent);
    }
}