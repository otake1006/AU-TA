using UnityEngine;
using System.Collections;

public class CharacterAnimator : MonoBehaviour
{
    private Animator animator;
    private Character character;

    [Header("Animation Settings")]
    public float animationSpeed = 1f;
    public bool enableAnimations = true;

    // アニメーション状態
    private bool isAnimating = false;
    private string currentAnimation;

    public void Initialize(Character owner)
    {
        character = owner;
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            animator = gameObject.AddComponent<Animator>();
        }

        // キャラクターデータからアニメーターを設定
        if (character.characterData?.animatorController != null)
        {
            animator.runtimeAnimatorController = character.characterData.animatorController;
        }

        SetAnimationSpeed(animationSpeed);
    }

    public void PlayAnimation(string animationName, bool forcePlay = false)
    {
        if (!enableAnimations || animator == null) return;

        if (isAnimating && !forcePlay) return;

        currentAnimation = animationName;
        animator.SetTrigger(animationName);

        StartCoroutine(TrackAnimation(animationName));
    }

    IEnumerator TrackAnimation(string animationName)
    {
        isAnimating = true;

        // アニメーション開始を待つ
        yield return new WaitForEndOfFrame();

        // アニメーション終了を待つ
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) ||
               animator.IsInTransition(0))
        {
            yield return null;
        }

        isAnimating = false;
        OnAnimationComplete(animationName);
    }

    void OnAnimationComplete(string animationName)
    {
        // アニメーション完了後の処理
        switch (animationName)
        {
            case GameConstants.ANIM_ATTACK:
                PlayAnimation(GameConstants.ANIM_IDLE);
                break;
            case GameConstants.ANIM_DAMAGED:
                PlayAnimation(GameConstants.ANIM_IDLE);
                break;
            case GameConstants.ANIM_HEAL:
                PlayAnimation(GameConstants.ANIM_IDLE);
                break;
        }
    }

    public void SetAnimationSpeed(float speed)
    {
        animationSpeed = speed;
        if (animator != null)
        {
            animator.speed = speed;
        }
    }

    public void PlayAttackAnimation()
    {
        PlayAnimation(GameConstants.ANIM_ATTACK);
    }

    public void PlayDamagedAnimation()
    {
        PlayAnimation(GameConstants.ANIM_DAMAGED);
    }

    public void PlayHealAnimation()
    {
        PlayAnimation(GameConstants.ANIM_HEAL);
    }

    public void PlayDeathAnimation()
    {
        PlayAnimation(GameConstants.ANIM_DEATH, true);
    }

    public void PlayIdleAnimation()
    {
        PlayAnimation(GameConstants.ANIM_IDLE, true);
    }

    public bool IsAnimating()
    {
        return isAnimating;
    }

    public string GetCurrentAnimation()
    {
        return currentAnimation;
    }

    // デバッグ用
    [ContextMenu("Play Attack Animation")]
    public void DebugPlayAttack()
    {
        PlayAttackAnimation();
    }

    [ContextMenu("Play Damaged Animation")]
    public void DebugPlayDamaged()
    {
        PlayDamagedAnimation();
    }
}