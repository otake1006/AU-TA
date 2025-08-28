using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ItemSwapper : MonoBehaviour
{
    [SerializeField] private List<ActtiveDrag> items;
    [SerializeField] private RectTransform currentItemBeingDragged;
    [SerializeField] private RectTransform itemToSwapwish;

    private void Start()
    {
        PopulataThaItems();
    }

    private void Update()
    {
        if (currentItemBeingDragged != null)
        {
            SwapCurrentItemBeingDragged();
        }
    }

    private void PopulataThaItems()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            items.Add(transform.GetChild(i).GetComponent<ActtiveDrag>());
        }
    }

    public void SetCurrentItemBeingDragged(RectTransform currentItemBeingDragged)
    {
        this.currentItemBeingDragged = currentItemBeingDragged;
    }

    public void NullifyCurrentItemBeingDragged()
    {
        currentItemBeingDragged = null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    private void SwapCurrentItemBeingDragged()
    {
        foreach (var item in items)
        {
            if (item == currentItemBeingDragged) continue;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(item.GetComponent<RectTransform>(), Mouse.current.position.ReadValue(), null, out Vector2 localPoint);

            if (currentItemBeingDragged.rect.Contains(localPoint))
            {
                itemToSwapwish = item.GetComponent<RectTransform>();
                if (itemToSwapwish == null) return;
                if (!item.gameObject.activeInHierarchy) return;
                if (!item.enabled) return;

                currentItemBeingDragged.transform.SetSiblingIndex(item.transform.GetSiblingIndex());
                item.SetText((item.transform.GetSiblingIndex()+1).ToString());
                itemToSwapwish = null;
            }

        }

    }
}