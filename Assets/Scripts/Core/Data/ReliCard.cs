namespace TacticalCardGame.Core
{
    public class RelicCard : IRelicCard
    {
        public int Id { get; }
        public string Name { get; }
        public CardType Type => CardType.Relic;
        public int Cost { get; }
        public string Description { get; }
        public RelicEffect Effect { get; }
        public int Duration { get; }

        public RelicCard(int id, string name, RelicEffect effect, int duration, string description = "")
        {
            Id = id;
            Name = name;
            Cost = 0; // レリックはコストなし
            Effect = effect;
            Duration = duration;
            Description = description;
        }

        public override string ToString() => $"{Name} [Relic] (Duration:{Duration})";
    }
}