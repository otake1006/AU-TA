namespace TacticalCardGame.Core
{
    public interface IRelicCard : ICard
    {
        RelicEffect Effect { get; }
        int Duration { get; }
    }
}