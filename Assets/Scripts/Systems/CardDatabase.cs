using System;
using System.Collections.Generic;

namespace TacticalCardGame.Core
{
    public class CardDatabase
    {
        private readonly List<ISkillCard> skillCards = new List<ISkillCard>();
        private readonly List<IConditionCard> conditionCards = new List<IConditionCard>();
        private readonly List<IRelicCard> relicCards = new List<IRelicCard>();
        private readonly Random random = new Random();

        public CardDatabase()
        {
            InitializeCards();
        }

        private void InitializeCards()
        {
            // スキルカード
            skillCards.Add(new SkillCard(1, "火の矢", 2, SkillType.Attack, 3, TargetType.Opponent));
            skillCards.Add(new SkillCard(2, "氷の盾", 2, SkillType.Defense, 2, TargetType.Self));
            skillCards.Add(new SkillCard(3, "回復", 1, SkillType.Heal, 2, TargetType.Self));
            skillCards.Add(new SkillCard(4, "雷撃", 3, SkillType.Attack, 4, TargetType.Opponent));
            skillCards.Add(new SkillCard(5, "鉄壁", 3, SkillType.Defense, 3, TargetType.Self));
            skillCards.Add(new SkillCard(6, "大回復", 3, SkillType.Heal, 4, TargetType.Self));
            skillCards.Add(new SkillCard(7, "連続攻撃", 4, SkillType.Attack, 6, TargetType.Opponent));

            // 条件カード
            conditionCards.Add(new ConditionCard(101, "HP優勢", 1,
                (player, opponent) => player.HP > opponent.HP, "自分のHPが相手より多い時"));
            conditionCards.Add(new ConditionCard(102, "HP劣勢", 1,
                (player, opponent) => player.HP < opponent.HP, "自分のHPが相手より少ない時"));
            conditionCards.Add(new ConditionCard(103, "MP満タン", 2,
                (player, opponent) => player.MP >= player.MaxMP, "自分のMPが満タンの時"));
            conditionCards.Add(new ConditionCard(104, "MP半分以下", 1,
                (player, opponent) => player.MP <= player.MaxMP / 2, "自分のMPが半分以下の時"));

            // レリックカード
            relicCards.Add(new RelicCard(201, "生命の護符", RelicEffect.HPBoost, 3, "最大HP+3"));
            relicCards.Add(new RelicCard(202, "魔力の石", RelicEffect.MPBoost, 3, "最大MP+2"));
            relicCards.Add(new RelicCard(203, "戦士の指輪", RelicEffect.DamageBoost, 2, "攻撃力+1"));
            relicCards.Add(new RelicCard(204, "守護の盾", RelicEffect.DefenseBoost, 2, "防御力+1"));
            relicCards.Add(new RelicCard(205, "知恵の書", RelicEffect.DrawExtra, 1, "追加ドロー"));
        }

        public ISkillCard GetRandomSkillCard()
        {
            var original = skillCards[random.Next(skillCards.Count)];
            return new SkillCard(original.Id, original.Name, original.Cost,
                original.SkillType, original.Power, original.Target, original.Description);
        }

        public IConditionCard GetRandomConditionCard()
        {
            return conditionCards[random.Next(conditionCards.Count)];
        }

        public IRelicCard GetRandomRelicCard()
        {
            return relicCards[random.Next(relicCards.Count)];
        }
    }
}