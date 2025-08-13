namespace TacticalCardGame.Core
{
    public interface ICard
    {
        int Id { get; }
        string Name { get; }
        CardType Type { get; }
        int Cost { get; }
        string Description { get; }
    }
}