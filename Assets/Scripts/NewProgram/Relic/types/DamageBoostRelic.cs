using UnityEngine;

[CreateAssetMenu(fileName = "New Damage Boost Relic", menuName = "Relics/Damage Boost Relic")]
public class DamageBoostRelic : RelicEffect
{
    [Header("Damage Boost Settings")]
    public float damageMultiplier = 1.1f; // 10%ダメージ増加

    public override void OnAcquired()
    {
        GameEvents.OnDebugMessage?.Invoke($"{owner.characterName} acquired {relicName}!");

        // パッシブ効果として、キャラクターのダメージ倍率を増加
        if (owner != null)
        {
            // ここでキャラクターのステータスを変更
            // owner.damageMultiplier *= damageMultiplier;
        }
    }

    public override void OnRemoved()
    {
        GameEvents.OnDebugMessage?.Invoke($"{owner.characterName} lost {relicName}!");

        // 効果を元に戻す
        if (owner != null)
        {
            // owner.damageMultiplier /= damageMultiplier;
        }
    }

    protected override void OnStackEffect()
    {
        // スタック時の追加効果
        GameEvents.OnDebugMessage?.Invoke($"{relicName} stacked! ({stackCount}/{maxStacks})");
    }

    public override float GetEffectValue()
    {
        return damageMultiplier;
    }
}
