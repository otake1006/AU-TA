using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using TMPro;

public class ActtiveDrag : MonoBehaviour, IEndDragHandler, IBeginDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private ItemSwapper swapper;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        swapper = GetComponentInParent<ItemSwapper>();
    }

    public void SetText(String number)
    {
        text.text = number;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        swapper.SetCurrentItemBeingDragged(rectTransform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        swapper.NullifyCurrentItemBeingDragged();
    }
}
