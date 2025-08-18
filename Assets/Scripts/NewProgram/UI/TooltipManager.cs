using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }

    [Header("Tooltip UI")]
    public GameObject tooltipPanel;
    public Text tooltipText;
    public RectTransform tooltipRect;
    public CanvasGroup tooltipCanvasGroup;

    [Header("Settings")]
    public float showDelay = 0.5f;
    public float fadeSpeed = 5f;
    public Vector2 offset = new Vector2(10, 10);
    public bool followMouse = true;

    private Coroutine showTooltipCoroutine;
    private bool isVisible = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);

        SetupInputListeners();
    }

    void SetupInputListeners()
    {
        InputManager.OnMouseMove += OnMouseMove;
    }

    void OnMouseMove(Vector2 mousePosition)
    {
        if (isVisible && followMouse)
        {
            UpdateTooltipPosition(mousePosition);
        }
    }

    public void ShowTooltip(string text, Vector3 worldPosition)
    {
        if (showTooltipCoroutine != null)
        {
            StopCoroutine(showTooltipCoroutine);
        }

        showTooltipCoroutine = StartCoroutine(ShowTooltipCoroutine(text, worldPosition));
    }

    public void ShowTooltip(string text, Vector2 screenPosition)
    {
        if (showTooltipCoroutine != null)
        {
            StopCoroutine(showTooltipCoroutine);
        }

        showTooltipCoroutine = StartCoroutine(ShowTooltipCoroutine(text, screenPosition));
    }

    public void HideTooltip()
    {
        if (showTooltipCoroutine != null)
        {
            StopCoroutine(showTooltipCoroutine);
        }

        StartCoroutine(HideTooltipCoroutine());
    }

    IEnumerator ShowTooltipCoroutine(string text, Vector3 position)
    {
        yield return new WaitForSeconds(showDelay);

        if (tooltipText != null)
            tooltipText.text = text;

        if (tooltipPanel != null)
            tooltipPanel.SetActive(true);

        UpdateTooltipPosition(position);

        isVisible = true;

        // フェードイン
        if (tooltipCanvasGroup != null)
        {
            tooltipCanvasGroup.alpha = 0f;
            while (tooltipCanvasGroup.alpha < 1f)
            {
                tooltipCanvasGroup.alpha += fadeSpeed * Time.deltaTime;
                yield return null;
            }
        }
    }

    IEnumerator HideTooltipCoroutine()
    {
        if (!isVisible) yield break;

        // フェードアウト
        if (tooltipCanvasGroup != null)
        {
            while (tooltipCanvasGroup.alpha > 0f)
            {
                tooltipCanvasGroup.alpha -= fadeSpeed * Time.deltaTime;
                yield return null;
            }
        }

        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);

        isVisible = false;
    }

    void UpdateTooltipPosition(Vector3 position)
    {
        if (tooltipRect == null) return;

        Vector2 screenPosition;

        // World座標からScreen座標への変換
        if (position.z == 0) // すでにScreen座標
        {
            screenPosition = position;
        }
        else // World座標
        {
            screenPosition = Camera.main.WorldToScreenPoint(position);
        }

        // オフセット適用
        screenPosition += offset;

        // 画面外に出ないよう調整
        float tooltipWidth = tooltipRect.sizeDelta.x;
        float tooltipHeight = tooltipRect.sizeDelta.y;

        if (screenPosition.x + tooltipWidth > Screen.width)
            screenPosition.x = Screen.width - tooltipWidth;
        if (screenPosition.y + tooltipHeight > Screen.height)
            screenPosition.y = Screen.height - tooltipHeight;

        if (screenPosition.x < 0)
            screenPosition.x = 0;
        if (screenPosition.y < 0)
            screenPosition.y = 0;

        tooltipRect.position = screenPosition;
    }

    void OnDestroy()
    {
        InputManager.OnMouseMove -= OnMouseMove;
    }
}