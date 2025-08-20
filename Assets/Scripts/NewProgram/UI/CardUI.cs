using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [Header("UI Components")]
    public Image cardImage;
    public Text cardNameText;
    public Text manaCostText;
    public Text descriptionText;

    public void SetCard(BaseCard card)
    {
        if (cardImage != null && card.cardImage != null)
        {
            cardImage.sprite = card.cardImage;
            cardImage.gameObject.SetActive(true);
        }

        if (cardNameText != null)
        {
            cardNameText.text = card.cardName;
            cardNameText.gameObject.SetActive(true);
        }

        if (manaCostText != null)
        {
            manaCostText.text = card.manaCost.ToString();
            manaCostText.gameObject.SetActive(true);
        }

        if (descriptionText != null)
        {
            descriptionText.text = card.description;
            descriptionText.gameObject.SetActive(true);
        }
    }
}