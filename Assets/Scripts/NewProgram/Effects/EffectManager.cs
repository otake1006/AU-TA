using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    [Header("Effect Prefabs")]
    public GameObject[] effectPrefabs;

    [Header("Damage Text")]
    public GameObject damageTextPrefab;
    public GameObject healTextPrefab;

    [Header("Settings")]
    public float defaultEffectDuration = 2f;
    public int maxActiveEffects = 20;

    private Dictionary<string, GameObject> effectDictionary = new Dictionary<string, GameObject>();
    private List<GameObject> activeEffects = new List<GameObject>();
    private GameConfig gameConfig;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(GameConfig config)
    {
        gameConfig = config;
        LoadEffectPrefabs();
        SetupEventListeners();
    }

    void LoadEffectPrefabs()
    {
        foreach (var prefab in effectPrefabs)
        {
            if (prefab != null)
                effectDictionary[prefab.name] = prefab;
        }
    }

    void SetupEventListeners()
    {
        GameEvents.OnEffectPlay += PlayEffect;
        GameEvents.OnDamageTextShow += ShowDamageText;
        GameEvents.OnHealTextShow += ShowHealText;
    }

    public void PlayEffect(string effectName, Vector3 position)
    {
        if (effectDictionary.TryGetValue(effectName, out GameObject prefab))
        {
            GameObject effect = Instantiate(prefab, position, Quaternion.identity);
            activeEffects.Add(effect);

            // エフェクト自動削除
            StartCoroutine(DestroyEffectAfterTime(effect, defaultEffectDuration));

            // 最大数制限
            if (activeEffects.Count > maxActiveEffects)
            {
                DestroyOldestEffect();
            }
        }
        else
        {
            Debug.LogWarning($"Effect '{effectName}' not found!");
        }
    }

    public void ShowDamageText(Vector3 position, int damage, DamageType damageType)
    {
        if (damageTextPrefab != null)
        {
            GameObject textObj = Instantiate(damageTextPrefab, position, Quaternion.identity);
            var damageText = textObj.GetComponent<DamageText>();

            if (damageText != null)
            {
                Color textColor = GetDamageColor(damageType);
                float duration = gameConfig?.damageTextDuration ?? GameConstants.DAMAGE_TEXT_FADE_TIME;
                damageText.Initialize(damage.ToString(), textColor, duration, 2f);
            }
        }
    }

    public void ShowHealText(Vector3 position, int healAmount)
    {
        if (healTextPrefab != null)
        {
            GameObject textObj = Instantiate(healTextPrefab, position, Quaternion.identity);
            var healText = textObj.GetComponent<DamageText>();

            if (healText != null)
            {
                float duration = gameConfig?.damageTextDuration ?? GameConstants.DAMAGE_TEXT_FADE_TIME;
                healText.Initialize($"+{healAmount}", Color.green, duration, 2f);
            }
        }
    }

    Color GetDamageColor(DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.Normal: return Color.white;
            case DamageType.Critical: return Color.yellow;
            case DamageType.Shield: return Color.cyan;
            case DamageType.Poison: return Color.magenta;
            case DamageType.Burn: return Color.red;
            case DamageType.Magic: return Color.blue;
            case DamageType.True: return Color.black;
            default: return Color.white;
        }
    }

    IEnumerator DestroyEffectAfterTime(GameObject effect, float time)
    {
        yield return new WaitForSeconds(time);

        if (effect != null)
        {
            activeEffects.Remove(effect);
            Destroy(effect);
        }
    }

    void DestroyOldestEffect()
    {
        if (activeEffects.Count > 0)
        {
            GameObject oldest = activeEffects[0];
            activeEffects.RemoveAt(0);
            if (oldest != null)
                Destroy(oldest);
        }
    }

    public void ClearAllEffects()
    {
        foreach (var effect in activeEffects)
        {
            if (effect != null)
                Destroy(effect);
        }
        activeEffects.Clear();
    }

    void OnDestroy()
    {
        GameEvents.OnEffectPlay -= PlayEffect;
        GameEvents.OnDamageTextShow -= ShowDamageText;
        GameEvents.OnHealTextShow -= ShowHealText;
    }
}