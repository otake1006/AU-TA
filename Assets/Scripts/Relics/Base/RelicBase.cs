using CardBattle.Core;
using UnityEngine;

public abstract class RelicBase : ScriptableObject
{
    [Header("Šî–{î•ñ")]
    public string relicId;
    public string relicName;
    [TextArea(2, 4)]
    public string description;
    public Sprite icon;

    [Header("”­“®ğŒ")]
    public RelicTrigger trigger = RelicTrigger.TurnStart;

    [Header("g—p§ŒÀ")]
    public bool hasUsageLimit = false;
    public int maxUses = 3;

    [HideInInspector]
    public int remainingUses;
    [HideInInspector]
    public bool isActive = true;

    public virtual void Initialize()
    {
        remainingUses = maxUses;
        isActive = true;
    }

    public abstract void OnTrigger(BattleContext context, Character owner, TriggerEvent evt);

    protected bool ConsumeUse()
    {
        if (!isActive) return false;
        if (!hasUsageLimit) return true;

        remainingUses--;
        if (remainingUses <= 0)
        {
            isActive = false;
            return true;
        }
        return true;
    }
}