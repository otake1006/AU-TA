using UnityEngine;
using UnityEngine.UI;

public class TurnBasedBuffIconUI : MonoBehaviour
{
    [Header("UI Components")]
    public Image iconImage;
    public Image backgroundImage;
    public Text durationText;
    public Text stackText;

    private TurnBasedBuffEffect associatedBuff;

    public void SetBuff(TurnBasedBuffEffect buff)
    {
        associatedBuff = buff;

        // アイコン設定
        if (iconImage != null && buff.buffIcon != null)
        {
            iconImage.sprite = buff.buffIcon;
        }

        // 背景色設定
        if (backgroundImage != null)
        {
            switch (buff.buffType)
            {
                case BuffType.Buff:
                    backgroundImage.color = new Color(0.2f, 0.8f, 0.2f, 0.8f); // 緑
                    break;
                case BuffType.Debuff:
                    backgroundImage.color = new Color(0.8f, 0.2f, 0.2f, 0.8f); // 赤
                    break;
                case BuffType.Neutral:
                    backgroundImage.color = new Color(0.5f, 0.5f, 0.5f, 0.8f); // グレー
                    break;
            }
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (associatedBuff == null) return;

        // 残りターン数テキスト
        if (durationText != null && !associatedBuff.isPermanent)
        {
            durationText.text = associatedBuff.RemainingTurns.ToString();
            durationText.gameObject.SetActive(true);
        }
        else if (durationText != null)
        {
            durationText.gameObject.SetActive(false);
        }

        // スタック数テキスト
        if (stackText != null && associatedBuff.maxStacks > 1)
        {
            stackText.text = associatedBuff.stackCount.ToString();
            stackText.gameObject.SetActive(associatedBuff.stackCount > 1);
        }
        else if (stackText != null)
        {
            stackText.gameObject.SetActive(false);
        }
    }
}