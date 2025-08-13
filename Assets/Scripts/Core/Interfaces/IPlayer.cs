using System.Collections.Generic;

namespace TacticalCardGame.Core
{
    public interface IPlayer
    {
        int Id { get; }
        int HP { get; set; }
        int MP { get; set; }
        int MaxHP { get; }
        int MaxMP { get; }
        List<ICard> Hand { get; }
        List<ICard> TacticalBoard { get; }
        List<IRelicCard> Relics { get; }
        void TakeDamage(int damage);
        void Heal(int amount);
        void GainMP(int amount);
        void SpendMP(int amount);
    }
}