using System;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace TacticalCardGame.Core
{
    public class ConditionCard : IConditionCard
    {
        public int Id { get; }
        public string Name { get; }
        public CardType Type => CardType.Condition;
        public int Cost { get; }
        public string Description { get; }

        private readonly Func<IPlayer, IPlayer, bool> conditionCheck;

        public ConditionCard(int id, string name, int cost, Func<IPlayer, IPlayer, bool> condition, string description = "")
        {
            Id = id;
            Name = name;
            Cost = cost;
            conditionCheck = condition;
            Description = description;
        }

        public bool CheckCondition(IPlayer player, IPlayer opponent)
        {
            return conditionCheck?.Invoke(player, opponent) ?? false;
        }

        public override string ToString() => $"{Name} [Condition] (Cost:{Cost})";
    }
}