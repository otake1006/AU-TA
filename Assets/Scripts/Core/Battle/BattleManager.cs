using CardBattle.Core;
using System;
using System.Linq;
using UnityEngine;

public class BattleManager
{
    private BattleContext _context;
    private GameSettings _settings;
    private bool _battleEnded;

    public event Action<Character> OnBattleEnd;
    public event Action<int> OnRoundStart;
    public event Action<int> OnRoundEnd;

    public BattleManager(GameSettings settings)
    {
        _settings = settings;
    }

    public void StartBattle(Character player, Character enemy)
    {
        _context = new BattleContext
        {
            Settings = _settings,
            Player = player,
            Enemy = enemy,
            CurrentRound = 1,
            CurrentTurn = 1
        };

        // �L�����N�^�[������
        player.Initialize(_settings);
        enemy.Initialize(_settings);

        Debug.Log("=== BATTLE START ===");
        Debug.Log($"{player.Name} VS {enemy.Name}");

        RunBattle();
    }

    private void RunBattle()
    {
        _battleEnded = false;

        for (int round = 1; round <= _settings.maxRounds && !_battleEnded; round++)
        {
            _context.CurrentRound = round;
            RunRound();

            if (_battleEnded) break;

            if (round < _settings.maxRounds)
            {
                ProcessRoundEnd();
            }
        }

        if (!_battleEnded)
        {
            DetermineFinalWinner();
        }
    }

    private void RunRound()
    {
        Debug.Log($"\n=== ROUND {_context.CurrentRound} START ===");
        OnRoundStart?.Invoke(_context.CurrentRound);

        ProcessRelics(RelicTrigger.RoundStart);

        for (int turn = 1; turn <= _settings.turnsPerRound && !_battleEnded; turn++)
        {
            _context.CurrentTurn = turn;
            RunTurn();
        }

        if (!_battleEnded && _context.CurrentTurn >= _settings.turnsPerRound)
        {
            Debug.Log($"���E���h {_context.CurrentRound} �͈�������");
        }

        OnRoundEnd?.Invoke(_context.CurrentRound);
    }

    private void RunTurn()
    {
        Debug.Log($"\n--- Turn {_context.CurrentTurn} ---");

        // MP��
        _context.Player.RestoreMP(_settings.mpRecoveryPerTurn, _context);
        _context.Enemy.RestoreMP(_settings.mpRecoveryPerTurn, _context);

        // �o�t���ԍX�V
        _context.Player.Stats.UpdateBuffDurations();
        _context.Enemy.Stats.UpdateBuffDurations();

        ProcessRelics(RelicTrigger.TurnStart);

        // ��p�{�[�h�]��
        var playerSkill = _context.Player.EvaluateTacticalBoard(_context);
        var enemySkill = _context.Enemy.EvaluateTacticalBoard(_context);

        // �X�L���������s
        if (playerSkill != null)
        {
            ExecuteSkill(_context.Player, playerSkill);
        }
        else
        {
            ProcessSkillNotUsed(_context.Player);
        }

        if (enemySkill != null)
        {
            ExecuteSkill(_context.Enemy, enemySkill);
        }
        else
        {
            ProcessSkillNotUsed(_context.Enemy);
        }

        ProcessRelics(RelicTrigger.TurnEnd);

        CheckBattleEnd();
        DisplayStatus();
    }

    private void ExecuteSkill(Character caster, SkillBase skill)
    {
        Character target = skill.targetType == TargetType.Enemy ?
            (caster == _context.Player ? _context.Enemy : _context.Player) : caster;

        skill.Execute(_context, caster, target);
    }

    private void ProcessSkillNotUsed(Character character)
    {
        foreach (var relic in character.relics.Where(r => r.trigger == RelicTrigger.OnSkillNotUsed && r.isActive))
        {
            relic.OnTrigger(_context, character, new TriggerEvent
            {
                EventType = "SkillNotUsed",
                Source = character
            });
        }
    }

    private void ProcessRelics(RelicTrigger trigger)
    {
        foreach (var character in new[] { _context.Player, _context.Enemy })
        {
            foreach (var relic in character.relics.Where(r => r.trigger == trigger && r.isActive))
            {
                relic.OnTrigger(_context, character, new TriggerEvent
                {
                    EventType = trigger.ToString(),
                    Source = character
                });
            }
        }
    }

    private void CheckBattleEnd()
    {
        bool playerAlive = _context.Player.IsAlive();
        bool enemyAlive = _context.Enemy.IsAlive();

        if (!playerAlive && !enemyAlive)
        {
            Debug.Log("=== �������� (�������j) ===");
            _battleEnded = true;
            OnBattleEnd?.Invoke(null);
        }
        else if (!playerAlive)
        {
            Debug.Log($"=== {_context.Enemy.Name} �̏����I ===");
            _battleEnded = true;
            OnBattleEnd?.Invoke(_context.Enemy);
        }
        else if (!enemyAlive)
        {
            Debug.Log($"=== {_context.Player.Name} �̏����I ===");
            _battleEnded = true;
            OnBattleEnd?.Invoke(_context.Player);
        }
    }

    private void ProcessRoundEnd()
    {
        Debug.Log($"\n=== ���E���h {_context.CurrentRound} �I������ ===");

        // HP/MP������
        _context.Player.Heal(_settings.roundEndHPRecover, _context);
        _context.Enemy.Heal(_settings.roundEndHPRecover, _context);
        _context.Player.RestoreMP(_settings.roundEndMPRecover, _context);
        _context.Enemy.RestoreMP(_settings.roundEndMPRecover, _context);

        // �s�Ҕ���ƃ����b�N�t�^�i�~�σV�X�e���j
        Character loser = null;
        if (_context.Player.Stats.currentHP < _context.Enemy.Stats.currentHP)
            loser = _context.Player;
        else if (_context.Enemy.Stats.currentHP < _context.Player.Stats.currentHP)
            loser = _context.Enemy;

        if (loser != null)
        {
            // ���I�Ƀ����b�N�𐶐�����ꍇ�̗�
            // ���ۂɂ̓v���Z�b�g�̃����b�N����I�����邱�Ƃ𐄏�
            _context.Log($"{loser.Name} �ɋ~�ϑ[�u��K�p");
        }
    }

    private void DetermineFinalWinner()
    {
        Debug.Log("\n=== �ŏI���� ===");

        if (_context.Player.Stats.currentHP > _context.Enemy.Stats.currentHP)
        {
            Debug.Log($"{_context.Player.Name} �̏����I (HP��)");
            OnBattleEnd?.Invoke(_context.Player);
        }
        else if (_context.Enemy.Stats.currentHP > _context.Player.Stats.currentHP)
        {
            Debug.Log($"{_context.Enemy.Name} �̏����I (HP��)");
            OnBattleEnd?.Invoke(_context.Enemy);
        }
        else
        {
            Debug.Log("���S��������");
            OnBattleEnd?.Invoke(null);
        }
    }

    private void DisplayStatus()
    {
        var p = _context.Player.Stats;
        var e = _context.Enemy.Stats;

        Debug.Log($"[Player] HP:{p.currentHP}/{p.maxHP} MP:{p.currentMP}/{p.maxMP} Shield:{p.shield}");
        Debug.Log($"[Enemy]  HP:{e.currentHP}/{e.maxHP} MP:{e.currentMP}/{e.maxMP} Shield:{e.shield}");
    }
}