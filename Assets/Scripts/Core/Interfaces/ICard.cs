namespace CardBattle.Core
{
    public interface ICard
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        int Cost { get; }
    }
}