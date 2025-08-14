using System.Collections.Generic;

namespace CardBattle.Core
{
    public interface ICondition : ICard
    {
        bool Evaluate(BattleContext context, Character owner);
    }
}