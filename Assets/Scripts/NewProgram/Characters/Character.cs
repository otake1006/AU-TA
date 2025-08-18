using System;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Character Data")]
    public CharacterData characterData;
    public string characterName;

    [Header("Stats")]
    public int maxHealth = 100;
    public int maxMana = 50;
    public int baseAttack = 10;
    public int baseDefense = 5;

    // 現在の値
    private CharacterStats currentStats;
    private int currentShield = 0;
    private bool canUseSkills = true;

    // コンポーネント参照
    private CharacterAnimator characterAnimator;
    private CharacterAudio characterAudio;
    private TurnBasedBuffManager buffManager;

    // プロパティ
    public int CurrentHealth => currentStats.currentHealth;
    public int CurrentMana => currentStats.currentMana;
    public int CurrentAttack => currentStats.currentAttack;
    public int CurrentDefense => currentStats.currentDefense;
    public int CurrentShield => currentShield;
    public bool CanUseSkills => canUseSkills;
    public bool IsDead => currentStats.currentHealth <= 0;

    // イベント
    public Action<int, int> OnHealthChanged;  // current, max
    public Action<int, int> OnManaChanged;    // current, max
    public Action<int> OnAttackChanged;
    public Action<int> OnDefenseChanged;
    public Action<int> OnShieldChanged;
    public Action OnDeath;

    void Start()
    {
        InitializeCharacter();
        SetupComponents();
    }

    void InitializeCharacter()
    {
        // データから初期化
        if (characterData != null)
        {
            characterName = characterData.characterName;
            maxHealth = characterData.maxHealth;
            maxMana = characterData.maxMana;
            baseAttack = characterData.baseAttack;
            baseDefense = characterData.baseDefense;
        }

        // ステータス初期化
        currentStats = new CharacterStats();
        ResetToBaseStats();

        // UI更新
        NotifyStatsChanged();
    }

    void SetupComponents()
    {
        // アニメーター
        characterAnimator = GetComponent<CharacterAnimator>();
        if (characterAnimator == null)
            characterAnimator = gameObject.AddComponent<CharacterAnimator>();
        characterAnimator.Initialize(this);

        // オーディオ
        characterAudio = GetComponent<CharacterAudio>();
        if (characterAudio == null)
            characterAudio = gameObject.AddComponent<CharacterAudio>();
        characterAudio.Initialize(this);

        // バフマネージャー
        buffManager = GetComponent<TurnBasedBuffManager>();
        if (buffManager == null)
            buffManager = gameObject.AddComponent<TurnBasedBuffManager>();
    }

    void ResetToBaseStats()
    {
        currentStats.currentHealth = maxHealth;
        currentStats.currentMana = maxMana;
        currentStats.currentAttack = baseAttack;
        currentStats.currentDefense = baseDefense;
        currentShield = 0;
        canUseSkills = true;
    }

    // =============================================================================
    // ダメージ・回復システム
    // =============================================================================
    public void TakeDamage(int damage, DamageType damageType = DamageType.Normal)
    {
        if (IsDead) return;

        int actualDamage = CalculateActualDamage(damage, damageType);

        // シールドがある場合の処理
        if (currentShield > 0)
        {
            int shieldDamage = Mathf.Min(actualDamage, currentShield);
            currentShield -= shieldDamage;
            actualDamage -= shieldDamage;

            OnShieldChanged?.Invoke(currentShield);
            GameEvents.OnShieldChanged?.Invoke(this, currentShield);

            // シールドダメージエフェクト
            ShowDamageEffect(shieldDamage, DamageType.Shield);
        }

        // 残りダメージをHPに適用
        if (actualDamage > 0)
        {
            currentStats.currentHealth = Mathf.Max(0, currentStats.currentHealth - actualDamage);
            OnHealthChanged?.Invoke(currentStats.currentHealth, maxHealth);
            GameEvents.OnCharacterDamaged?.Invoke(this, actualDamage, damageType);

            ShowDamageEffect(actualDamage, damageType);
            characterAnimator?.PlayAnimation(GameConstants.ANIM_DAMAGED);
            characterAudio?.PlaySFX(GameConstants.SFX_DAMAGE);
        }

        // 死亡判定
        if (currentStats.currentHealth <= 0)
        {
            Die();
        }
    }

    int CalculateActualDamage(int baseDamage, DamageType damageType)
    {
        if (damageType == DamageType.True)
        {
            return baseDamage; // 真ダメージは防御無視
        }

        // 防御力を考慮
        int damage = Mathf.Max(1, baseDamage - currentStats.currentDefense);

        // クリティカル判定
        if (damageType == DamageType.Critical)
        {
            damage = Mathf.RoundToInt(damage * GameConstants.CRITICAL_CHANCE_BASE);
        }

        return damage;
    }

    public void Heal(int healAmount)
    {
        if (IsDead) return;

        int actualHeal = Mathf.Min(healAmount, maxHealth - currentStats.currentHealth);
        if (actualHeal > 0)
        {
            currentStats.currentHealth += actualHeal;
            OnHealthChanged?.Invoke(currentStats.currentHealth, maxHealth);
            GameEvents.OnCharacterHealed?.Invoke(this, actualHeal);

            ShowHealEffect(actualHeal);
            characterAnimator?.PlayAnimation(GameConstants.ANIM_HEAL);
            characterAudio?.PlaySFX(GameConstants.SFX_HEAL);
        }
    }

    // =============================================================================
    // ステータス変更システム
    // =============================================================================
    public void ModifyAttack(int amount)
    {
        currentStats.currentAttack = Mathf.Max(1, currentStats.currentAttack + amount);
        OnAttackChanged?.Invoke(currentStats.currentAttack);
        NotifyStatsChanged();
    }

    public void ModifyDefense(int amount)
    {
        currentStats.currentDefense = Mathf.Max(0, currentStats.currentDefense + amount);
        OnDefenseChanged?.Invoke(currentStats.currentDefense);
        NotifyStatsChanged();
    }

    public void AddShield(int shieldAmount)
    {
        currentShield += shieldAmount;
        OnShieldChanged?.Invoke(currentShield);
        GameEvents.OnShieldChanged?.Invoke(this, currentShield);
        ShowShieldEffect();
    }

    public void ClearShield()
    {
        currentShield = 0;
        OnShieldChanged?.Invoke(currentShield);
        GameEvents.OnShieldChanged?.Invoke(this, currentShield);
    }

    public void SetCanUseSkills(bool canUse)
    {
        canUseSkills = canUse;
    }

    // =============================================================================
    // マナシステム
    // =============================================================================
    public bool ConsumeMana(int amount)
    {
        if (currentStats.currentMana >= amount)
        {
            currentStats.currentMana -= amount;
            OnManaChanged?.Invoke(currentStats.currentMana, maxMana);
            GameEvents.OnManaChanged?.Invoke(this, currentStats.currentMana);
            return true;
        }
        return false;
    }

    public void RestoreMana(int amount)
    {
        currentStats.currentMana = Mathf.Min(maxMana, currentStats.currentMana + amount);
        OnManaChanged?.Invoke(currentStats.currentMana, maxMana);
        GameEvents.OnManaChanged?.Invoke(this, currentStats.currentMana);
    }

    public void ResetToFullMana()
    {
        currentStats.currentMana = maxMana;
        OnManaChanged?.Invoke(currentStats.currentMana, maxMana);
        GameEvents.OnManaChanged?.Invoke(this, currentStats.currentMana);
    }

    public void ResetToFullHealth()
    {
        currentStats.currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentStats.currentHealth, maxHealth);
    }

    // =============================================================================
    // 死亡処理
    // =============================================================================
    void Die()
    {
        characterAnimator?.PlayAnimation(GameConstants.ANIM_DEATH);
        characterAudio?.PlaySFX("Death");

        // 全バフを削除
        buffManager?.ClearAllBuffs();

        OnDeath?.Invoke();
        GameEvents.OnCharacterDeath?.Invoke(this);

        GameEvents.OnDebugMessage?.Invoke($"{characterName} has been defeated!");
    }

    public void Revive(int healthAmount = -1)
    {
        if (healthAmount == -1)
            healthAmount = maxHealth;

        currentStats.currentHealth = Mathf.Min(maxHealth, healthAmount);
        OnHealthChanged?.Invoke(currentStats.currentHealth, maxHealth);

        characterAnimator?.PlayAnimation(GameConstants.ANIM_IDLE);
        GameEvents.OnDebugMessage?.Invoke($"{characterName} has been revived!");
    }

    // =============================================================================
    // エフェクト表示
    // =============================================================================
    void ShowDamageEffect(int damage, DamageType damageType)
    {
        Vector3 position = transform.position + Vector3.up * 2f;
        GameEvents.OnDamageTextShow?.Invoke(position, damage, damageType);

        // ダメージエフェクト
        string effectName = GetDamageEffectName(damageType);
        GameEvents.OnEffectPlay?.Invoke(effectName, position);
    }

    void ShowHealEffect(int healAmount)
    {
        Vector3 position = transform.position + Vector3.up * 2f;
        GameEvents.OnHealTextShow?.Invoke(position, healAmount);
        GameEvents.OnEffectPlay?.Invoke("Heal", position);
    }

    void ShowShieldEffect()
    {
        GameEvents.OnEffectPlay?.Invoke("Shield", transform.position);
    }

    string GetDamageEffectName(DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.Critical: return "CriticalHit";
            case DamageType.Shield: return "ShieldHit";
            case DamageType.Poison: return "PoisonDamage";
            case DamageType.Burn: return "BurnDamage";
            case DamageType.Magic: return "MagicDamage";
            default: return "Hit";
        }
    }

    void NotifyStatsChanged()
    {
        GameEvents.OnUIUpdate?.Invoke();
    }

    // =============================================================================
    // デバッグ用メソッド
    // =============================================================================
    [ContextMenu("Debug - Take 10 Damage")]
    public void DebugTakeDamage()
    {
        TakeDamage(10);
    }

    [ContextMenu("Debug - Heal 20")]
    public void DebugHeal()
    {
        Heal(20);
    }

    [ContextMenu("Debug - Add Shield")]
    public void DebugAddShield()
    {
        AddShield(15);
    }

    [ContextMenu("Debug - Reset Stats")]
    public void DebugResetStats()
    {
        ResetToBaseStats();
        NotifyStatsChanged();
    }

    public void DebugSetHealth(int health)
    {
        currentStats.currentHealth = Mathf.Clamp(health, 0, maxHealth);
        OnHealthChanged?.Invoke(currentStats.currentHealth, maxHealth);

        if (currentStats.currentHealth <= 0)
            Die();
    }

    public void DebugSetMana(int mana)
    {
        currentStats.currentMana = Mathf.Clamp(mana, 0, maxMana);
        OnManaChanged?.Invoke(currentStats.currentMana, maxMana);
        GameEvents.OnManaChanged?.Invoke(this, currentStats.currentMana);
    }
}