using System;
using System.Collections.Generic;
using System.Linq;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace TacticalCardGame.Core
{
    public class GameController
    {
        private readonly IPlayer player1;
        private readonly IPlayer player2;
        private readonly CardDatabase cardDatabase;

        public int CurrentRound { get; private set; } = 1;
        public int MaxRounds { get; } = 5;
        public GameResult GameResult { get; private set; } = GameResult.Ongoing;

        public GameController()
        {
            player1 = new Player(1);
            player2 = new Player(2);
            cardDatabase = new CardDatabase();

            InitializeGame();
        }

        private void InitializeGame()
        {
            // 初期手札配布
            for (int i = 0; i < 5; i++)
            {
                player1.Hand.Add(cardDatabase.GetRandomSkillCard());
                player2.Hand.Add(cardDatabase.GetRandomSkillCard());
            }

            // 初期条件カード配布
            player1.Hand.Add(cardDatabase.GetRandomConditionCard());
            player2.Hand.Add(cardDatabase.GetRandomConditionCard());

            Console.WriteLine("=== ゲーム開始 ===");
            Console.WriteLine($"プレイヤー1: HP {player1.HP}/{player1.MaxHP}, MP {player1.MP}/{player1.MaxMP}");
            Console.WriteLine($"プレイヤー2: HP {player2.HP}/{player2.MaxHP}, MP {player2.MP}/{player2.MaxMP}");
        }

        public void PlaceCardOnBoard(int playerId, int cardIndex)
        {
            var player = GetPlayer(playerId);
            if (cardIndex < 0 || cardIndex >= player.Hand.Count) return;

            var card = player.Hand[cardIndex];
            if (player.MP < card.Cost)
            {
                Console.WriteLine($"MP不足: 必要{card.Cost}, 所持{player.MP}");
                return;
            }

            player.SpendMP(card.Cost);
            player.TacticalBoard.Add(card);
            player.Hand.RemoveAt(cardIndex);

            Console.WriteLine($"プレイヤー{playerId} が戦術ボードに配置: {card.Name}");
        }

        public void ExecuteRound()
        {
            if (GameResult != GameResult.Ongoing) return;

            Console.WriteLine($"\n=== ラウンド {CurrentRound} 実行 ===");

            // 1. 条件チェックとスキル実行
            ExecutePlayerActions(player1, player2);
            ExecutePlayerActions(player2, player1);

            // 2. ラウンド終了処理
            EndRoundProcessing();

            // 3. 勝利判定
            CheckGameEnd();

            CurrentRound++;

            // 4. 次ラウンド準備
            if (GameResult == GameResult.Ongoing && CurrentRound <= MaxRounds)
            {
                PrepareNextRound();
            }
        }

        private void ExecutePlayerActions(IPlayer activePlayer, IPlayer opponent)
        {
            // 戦術ボードのカードを上から順に実行
            var validCards = new List<ICard>();

            foreach (var card in activePlayer.TacticalBoard)
            {
                bool canExecute = true;

                // 条件カードがある場合はチェック
                if (card is IConditionCard condition)
                {
                    canExecute = condition.CheckCondition(activePlayer, opponent);
                    Console.WriteLine($"条件チェック [{condition.Name}]: {(canExecute ? "成功" : "失敗")}");
                }

                if (canExecute && card is ISkillCard skill)
                {
                    ExecuteSkill(skill, activePlayer, opponent);
                    validCards.Add(card);
                }
            }

            // 実行されたカードを戦術ボードから削除
            foreach (var card in validCards)
            {
                activePlayer.TacticalBoard.Remove(card);
            }
        }

        private void ExecuteSkill(ISkillCard skill, IPlayer caster, IPlayer target)
        {
            Console.WriteLine($"プレイヤー{caster.Id} が {skill.Name} を実行");

            switch (skill.SkillType)
            {
                case SkillType.Attack:
                    if (skill.Target == TargetType.Opponent)
                    {
                        target.TakeDamage(skill.Power);
                        Console.WriteLine($"  → プレイヤー{target.Id} に {skill.Power} ダメージ (HP: {target.HP})");
                    }
                    break;

                case SkillType.Defense:
                    if (skill.Target == TargetType.Self && caster is Player player)
                    {
                        player.AddDefense(skill.Power);
                        Console.WriteLine($"  → プレイヤー{caster.Id} が {skill.Power} 防御力獲得");
                    }
                    break;

                case SkillType.Heal:
                    if (skill.Target == TargetType.Self)
                    {
                        caster.Heal(skill.Power);
                        Console.WriteLine($"  → プレイヤー{caster.Id} が {skill.Power} 回復 (HP: {caster.HP})");
                    }
                    break;

                case SkillType.Buff:
                    Console.WriteLine($"  → バフ効果適用");
                    break;

                case SkillType.Debuff:
                    Console.WriteLine($"  → デバフ効果適用");
                    break;
            }
        }

        private void EndRoundProcessing()
        {
            // MP回復
            player1.GainMP(2);
            player2.GainMP(2);

            // 新しいスキルカード配布
            player1.Hand.Add(cardDatabase.GetRandomSkillCard());
            player2.Hand.Add(cardDatabase.GetRandomSkillCard());

            // 負けた方にレリック付与
            if (player1.HP < player2.HP)
            {
                var relic = cardDatabase.GetRandomRelicCard();
                player1.Relics.Add(relic);
                Console.WriteLine($"プレイヤー1 がレリックを獲得: {relic.Name}");
            }
            else if (player2.HP < player1.HP)
            {
                var relic = cardDatabase.GetRandomRelicCard();
                player2.Relics.Add(relic);
                Console.WriteLine($"プレイヤー2 がレリックを獲得: {relic.Name}");
            }

            // 防御リセット
            if (player1 is Player p1) p1.ResetDefense();
            if (player2 is Player p2) p2.ResetDefense();
        }

        private void CheckGameEnd()
        {
            bool p1Dead = player1.HP <= 0;
            bool p2Dead = player2.HP <= 0;
            bool maxRounds = CurrentRound >= MaxRounds;

            if (p1Dead && p2Dead)
            {
                GameResult = GameResult.Draw;
                Console.WriteLine("=== 引き分け！（同時HP0） ===");
            }
            else if (p1Dead)
            {
                GameResult = GameResult.Player2Win;
                Console.WriteLine("=== プレイヤー2 勝利！ ===");
            }
            else if (p2Dead)
            {
                GameResult = GameResult.Player1Win;
                Console.WriteLine("=== プレイヤー1 勝利！ ===");
            }
            else if (maxRounds)
            {
                GameResult = GameResult.Draw;
                Console.WriteLine("=== 引き分け！（最大ラウンド到達） ===");
            }
        }

        private void PrepareNextRound()
        {
            Console.WriteLine($"\n--- ラウンド {CurrentRound} 終了 ---");
            Console.WriteLine($"プレイヤー1: HP {player1.HP}/{player1.MaxHP}, MP {player1.MP}/{player1.MaxMP}");
            Console.WriteLine($"プレイヤー2: HP {player2.HP}/{player2.MaxHP}, MP {player2.MP}/{player2.MaxMP}");
        }

        public IPlayer GetPlayer(int playerId) => playerId == 1 ? player1 : player2;

        public void ShowGameState()
        {
            Console.WriteLine($"\n=== 現在の状況 (ラウンド {CurrentRound}) ===");
            ShowPlayerState(player1);
            ShowPlayerState(player2);
        }

        private void ShowPlayerState(IPlayer player)
        {
            Console.WriteLine($"\nプレイヤー{player.Id}:");
            Console.WriteLine($"  HP: {player.HP}/{player.MaxHP}, MP: {player.MP}/{player.MaxMP}");

            Console.WriteLine($"  手札 ({player.Hand.Count}枚):");
            for (int i = 0; i < player.Hand.Count; i++)
            {
                var canPlay = player.MP >= player.Hand[i].Cost;
                var status = canPlay ? "✓" : "✗";
                Console.WriteLine($"    {i + 1}. [{status}] {player.Hand[i]}");
            }

            Console.WriteLine($"  戦術ボード ({player.TacticalBoard.Count}枚):");
            foreach (var card in player.TacticalBoard)
            {
                Console.WriteLine($"    - {card}");
            }

            if (player.Relics.Count > 0)
            {
                Console.WriteLine($"  レリック ({player.Relics.Count}個):");
                foreach (var relic in player.Relics)
                {
                    Console.WriteLine($"    - {relic}");
                }
            }
        }
    }
}