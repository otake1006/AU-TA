using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RelicSelectionPanel : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panelObject;
    public Transform relicOptionParent;
    public GameObject relicOptionPrefab;
    public Button closeButton;

    [Header("Relic Database")]
    public List<RelicEffect> availableRelics = new List<RelicEffect>();

    private System.Action<RelicEffect> onRelicSelectedCallback;
    private List<GameObject> currentOptions = new List<GameObject>();

    void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HidePanel);
        }

        HidePanel();
    }

    public void ShowPanel(System.Action<RelicEffect> onRelicSelected)
    {
        onRelicSelectedCallback = onRelicSelected;

        CreateRelicOptions();

        if (panelObject != null)
        {
            panelObject.SetActive(true);
        }
    }

    public void HidePanel()
    {
        if (panelObject != null)
        {
            panelObject.SetActive(false);
        }

        ClearOptions();
    }

    void CreateRelicOptions()
    {
        ClearOptions();

        foreach (var relic in availableRelics)
        {
            CreateRelicOption(relic);
        }
    }

    void CreateRelicOption(RelicEffect relic)
    {
        if (relicOptionPrefab != null && relicOptionParent != null)
        {
            GameObject optionObject = Instantiate(relicOptionPrefab, relicOptionParent);
            RelicSelectionOption option = optionObject.GetComponent<RelicSelectionOption>();

            if (option != null)
            {
                option.SetupOption(relic, OnOptionSelected);
            }

            currentOptions.Add(optionObject);
        }
    }

    void OnOptionSelected(RelicEffect selectedRelic)
    {
        onRelicSelectedCallback?.Invoke(selectedRelic);
        HidePanel();
    }

    void ClearOptions()
    {
        foreach (var option in currentOptions)
        {
            if (option != null)
            {
                Destroy(option);
            }
        }
        currentOptions.Clear();
    }
}