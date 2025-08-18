using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    private Text textComponent;
    private float lifetime;
    private float speed;
    private float timer;
    private Vector3 startPosition;
    private Color originalColor;

    void Awake()
    {
        textComponent = GetComponent<Text>();
        if (textComponent == null)
        {
            // Textコンポーネントがない場合は作成
            textComponent = gameObject.AddComponent<Text>();
            textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            textComponent.fontSize = 24;
            textComponent.alignment = TextAnchor.MiddleCenter;
        }
    }

    public void Initialize(string text, Color color, float lifetime, float speed)
    {
        textComponent.text = text;
        textComponent.color = color;
        originalColor = color;
        this.lifetime = lifetime;
        this.speed = speed;
        startPosition = transform.position;
        timer = 0f;

        // キャンバスを探して親に設定
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            transform.SetParent(canvas.transform, false);

            // RectTransformの設定
            RectTransform rectTransform = GetComponent<RectTransform>();
            if (rectTransform == null)
                rectTransform = gameObject.AddComponent<RectTransform>();

            rectTransform.anchoredPosition = Camera.main.WorldToScreenPoint(startPosition);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 上方向に移動
        Vector3 worldPos = startPosition + Vector3.up * (speed * timer);

        // スクリーン座標に変換
        if (Camera.main != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            RectTransform rectTransform = GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.position = screenPos;
            }
        }

        // フェードアウト
        float alpha = Mathf.Lerp(1f, 0f, timer / lifetime);
        textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

        // サイズ変化（オプション）
        float scale = Mathf.Lerp(1f, 1.2f, timer / lifetime);
        transform.localScale = Vector3.one * scale;

        // 削除
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}