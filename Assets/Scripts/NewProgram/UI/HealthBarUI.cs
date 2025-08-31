using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("UI Components")]
    public Slider healthSlider;
    public Text healthText;
    public Image fillImage;

    [Header("Colors")]
    public Color healthyColor = Color.green;
    public Color warningColor = Color.yellow;
    public Color dangerColor = Color.red;

    [Header("Animation")]
    public float updateSpeed = 2f;
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

    public void UpdateHealthBar(Character character)
    {
        if (character == null) return;

        float healthPercent = (float)character.CurrentHealth / character.maxHealth;

        if (smoothUpdate)
        {
            targetValue = healthPercent;
        }
        else
        {
            currentValue = targetValue = healthPercent;
            UpdateVisuals();
        }

        if (healthText != null)
            healthText.text = $"{character.CurrentHealth}/{character.maxHealth}";
    }

    void UpdateVisuals()
    {
        if (healthSlider != null)
            healthSlider.value = currentValue;

        if (fillImage != null)
        {
            if (currentValue > 0.6f)
                fillImage.color = healthyColor;
            else if (currentValue > 0.3f)
                fillImage.color = warningColor;
            else
                fillImage.color = dangerColor;
        }
    }
}