using UnityEngine;
using UnityEngine.UI;

public class ManaBarUI : MonoBehaviour
{
    [Header("UI Components")]
    public Slider manaSlider;
    public Text manaText;
    public Image fillImage;

    [Header("Colors")]
    public Color fullManaColor = Color.blue;
    public Color lowManaColor = Color.cyan;

    [Header("Animation")]
    public float updateSpeed = 3f;
    public bool smoothUpdate = true;

    private float targetValue;
    private float currentValue;

    void Update()
    {
        if (smoothUpdate && Mathf.Abs(currentValue - targetValue) > 0.01f)
        {
            currentValue = Mathf.Lerp(currentValue, targetValue, updateSpeed * Time.deltaTime);
            UpdateVisuals();
        }
    }

    public void UpdateManaBar(Character character)
    {
        if (character == null) return;

        float manaPercent = (float)character.CurrentMana / character.maxMana;

        if (smoothUpdate)
        {
            targetValue = manaPercent;
        }
        else
        {
            currentValue = targetValue = manaPercent;
            UpdateVisuals();
        }

        if (manaText != null)
            manaText.text = $"{character.CurrentMana}/{character.maxMana}";
    }

    void UpdateVisuals()
    {
        if (manaSlider != null)
            manaSlider.value = currentValue;

        if (fillImage != null)
        {
            fillImage.color = Color.Lerp(lowManaColor, fullManaColor, currentValue);
        }
    }
}