using System;
using System.Collections.Generic;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace TacticalCardGame.Core
{
    public class Player : IPlayer
    {
        public int Id { get; }
        public int HP { get; set; }
        public int MP { get; set; }
        public int MaxHP { get; }
        public int MaxMP { get; }
        public List<ICard> Hand { get; }
        public List<ICard> TacticalBoard { get; }
        public List<IRelicCard> Relics { get; }

        private int defense = 0;

        public Player(int id, int maxHP = 20, int maxMP = 10)
        {
            Id = id;
            MaxHP = maxHP;
            MaxMP = maxMP;
            HP = maxHP;
            MP = maxMP;
            Hand = new List<ICard>();
            TacticalBoard = new List<ICard>();
            Relics = new List<IRelicCard>();
        }

        public void TakeDamage(int damage)
        {
            int actualDamage = Math.Max(0, damage - defense);
            HP = Math.Max(0, HP - actualDamage);
            defense = 0; // –hŒä‚Í1ƒ^[ƒ“‚Ì‚Ý
        }

        public void Heal(int amount)
        {
            HP = Math.Min(MaxHP, HP + amount);
        }

        public void GainMP(int amount)
        {
            MP = Math.Min(MaxMP, MP + amount);
        }

        public void SpendMP(int amount)
        {
            MP = Math.Max(0, MP - amount);
        }

        public void AddDefense(int amount)
        {
            defense += amount;
        }

        public int GetDefense() => defense;

        public void ResetDefense()
        {
            defense = 0;
        }
    }
}