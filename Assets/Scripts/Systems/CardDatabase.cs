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
            // �X�L���J�[�h
            skillCards.Add(new SkillCard(1, "�΂̖�", 2, SkillType.Attack, 3, TargetType.Opponent));
            skillCards.Add(new SkillCard(2, "�X�̏�", 2, SkillType.Defense, 2, TargetType.Self));
            skillCards.Add(new SkillCard(3, "��", 1, SkillType.Heal, 2, TargetType.Self));
            skillCards.Add(new SkillCard(4, "����", 3, SkillType.Attack, 4, TargetType.Opponent));
            skillCards.Add(new SkillCard(5, "�S��", 3, SkillType.Defense, 3, TargetType.Self));
            skillCards.Add(new SkillCard(6, "���", 3, SkillType.Heal, 4, TargetType.Self));
            skillCards.Add(new SkillCard(7, "�A���U��", 4, SkillType.Attack, 6, TargetType.Opponent));

            // �����J�[�h
            conditionCards.Add(new ConditionCard(101, "HP�D��", 1,
                (player, opponent) => player.HP > opponent.HP, "������HP�������葽����"));
            conditionCards.Add(new ConditionCard(102, "HP��", 1,
                (player, opponent) => player.HP < opponent.HP, "������HP�������菭�Ȃ���"));
            conditionCards.Add(new ConditionCard(103, "MP���^��", 2,
                (player, opponent) => player.MP >= player.MaxMP, "������MP�����^���̎�"));
            conditionCards.Add(new ConditionCard(104, "MP�����ȉ�", 1,
                (player, opponent) => player.MP <= player.MaxMP / 2, "������MP�������ȉ��̎�"));

            // �����b�N�J�[�h
            relicCards.Add(new RelicCard(201, "�����̌아", RelicEffect.HPBoost, 3, "�ő�HP+3"));
            relicCards.Add(new RelicCard(202, "���͂̐�", RelicEffect.MPBoost, 3, "�ő�MP+2"));
            relicCards.Add(new RelicCard(203, "��m�̎w��", RelicEffect.DamageBoost, 2, "�U����+1"));
            relicCards.Add(new RelicCard(204, "���̏�", RelicEffect.DefenseBoost, 2, "�h���+1"));
            relicCards.Add(new RelicCard(205, "�m�b�̏�", RelicEffect.DrawExtra, 1, "�ǉ��h���["));
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