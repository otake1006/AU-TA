using UnityEngine;
using UnityEngine.UI;

public class SetupManager : MonoBehaviour
{
    [ContextMenu("1. Create Game Managers")]
    void CreateGameManagers()
    {
        // メインマネージャー作成
        GameObject gameManagerObj = new GameObject("=== GAME MANAGERS ===");

        // GameManager
        GameObject gmObj = new GameObject("GameManager");
        gmObj.transform.SetParent(gameManagerObj.transform);
        gmObj.AddComponent<GameManager>();

        // BattleManager
        GameObject bmObj = new GameObject("BattleManager");
        bmObj.transform.SetParent(gameManagerObj.transform);
        bmObj.AddComponent<BattleManager>();
        bmObj.AddComponent<RoundManager>();
        bmObj.AddComponent<TurnManager>();
        bmObj.AddComponent<CardManager>();

        // UIManager
        GameObject uiObj = new GameObject("UIManager");
        uiObj.transform.SetParent(gameManagerObj.transform);
        uiObj.AddComponent<UIManager>();

        // AudioManager
        GameObject audioObj = new GameObject("AudioManager");
        audioObj.transform.SetParent(gameManagerObj.transform);
        audioObj.AddComponent<AudioManager>();

        // EffectManager
        GameObject effectObj = new GameObject("EffectManager");
        effectObj.transform.SetParent(gameManagerObj.transform);
        effectObj.AddComponent<EffectManager>();

        // DebugManager
        GameObject debugObj = new GameObject("InputSystemDebugManager");
        debugObj.transform.SetParent(gameManagerObj.transform);
        debugObj.AddComponent<InputSystemDebugManager>();
        debugObj.AddComponent<BattleLogger>();

        Debug.Log("✅ Game Managers created successfully!");
    }

    [ContextMenu("2. Create Characters")]
    void CreateCharacters()
    {
        // プレイヤー作成
        GameObject player = new GameObject("Player");
        player.tag = GameConstants.PLAYER_TAG;

        var playerChar = player.AddComponent<Character>();
        playerChar.characterName = "Player";
        playerChar.maxHealth = 100;
        playerChar.maxMana = 50;
        playerChar.baseAttack = 10;
        playerChar.baseDefense = 5;

        player.AddComponent<CharacterAnimator>();
        player.AddComponent<CharacterAudio>();
        player.AddComponent<TurnBasedBuffManager>();

        // 敵作成
        GameObject enemy = new GameObject("Enemy");
        enemy.tag = GameConstants.ENEMY_TAG;

        var enemyChar = enemy.AddComponent<Character>();
        enemyChar.characterName = "Enemy";
        enemyChar.maxHealth = 100;
        enemyChar.maxMana = 50;
        enemyChar.baseAttack = 10;
        enemyChar.baseDefense = 5;

        enemy.AddComponent<CharacterAnimator>();
        enemy.AddComponent<CharacterAudio>();
        enemy.AddComponent<TurnBasedBuffManager>();
        enemy.AddComponent<EnemyAI>();

        Debug.Log("✅ Characters created successfully!");
    }

    [ContextMenu("3. Create GameConfig")]
    void CreateGameConfig()
    {
        var config = ScriptableObject.CreateInstance<GameConfig>();
        config.name = "GameConfig";

        // デフォルト設定
        config.winsNeeded = 2;
        config.maxTurnsPerRound = 10;
        config.enableSimultaneousDefeat = true;
        config.simultaneousDefeatRule = SimultaneousDefeatRule.Draw;
        config.initialHandSize = 4;
        config.maxHandSize = 7;
        config.drawPerTurn = 1;
        config.manaRegenPerTurn = 3;
        config.enableDebugMode = true;
        config.animationSpeedMultiplier = 1f;

#if UNITY_EDITOR
        string path = "Assets/CardBattleGame/Data/GameConfig.asset";
        UnityEditor.AssetDatabase.CreateAsset(config, path);
        UnityEditor.AssetDatabase.SaveAssets();
        Debug.Log($"✅ GameConfig created at: {path}");
#endif
    }

    [ContextMenu("4. Create Sample Cards")]
    void CreateSampleCards()
    {
        var cardCreator = FindObjectOfType<CardCreator>();
        if (cardCreator == null)
        {
            GameObject temp = new GameObject("CardCreator");
            cardCreator = temp.AddComponent<CardCreator>();
        }

        cardCreator.CreateSampleCards();

        Debug.Log("✅ Sample cards created successfully!");
    }

    [ContextMenu("5. Setup UI Canvas")]
    void SetupUICanvas()
    {
        // メインキャンバス作成
        GameObject canvasObj = new GameObject("Main Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;

        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // EventSystem作成
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // UIエリア作成
        CreateUIAreas(canvasObj.transform);

        Debug.Log("✅ UI Canvas setup complete!");
    }

    void CreateUIAreas(Transform canvasTransform)
    {
        // 手札エリア
        CreateUIArea("Player Hand Area", canvasTransform, new Vector2(0, 50), new Vector2(800, 150));
        CreateUIArea("Enemy Hand Area", canvasTransform, new Vector2(0, -50), new Vector2(800, 150));

        // ステータスエリア
        CreateUIArea("Player Status", canvasTransform, new Vector2(-350, 250), new Vector2(300, 100));
        CreateUIArea("Enemy Status", canvasTransform, new Vector2(350, 250), new Vector2(300, 100));

        // バフエリア
        CreateUIArea("Player Buffs", canvasTransform, new Vector2(-350, 150), new Vector2(300, 80));
        CreateUIArea("Enemy Buffs", canvasTransform, new Vector2(350, 150), new Vector2(300, 80));

        // ターン表示
        CreateUIArea("Turn Indicator", canvasTransform, new Vector2(0, 300), new Vector2(200, 50));

        // マッチ状況
        CreateUIArea("Match Status", canvasTransform, new Vector2(0, -300), new Vector2(400, 100));
    }

    GameObject CreateUIArea(string name, Transform parent, Vector2 position, Vector2 size)
    {
        GameObject area = new GameObject(name);
        area.transform.SetParent(parent);

        RectTransform rect = area.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        return area;
    }

    [ContextMenu("6. Final Setup Check")]
    void FinalSetupCheck()
    {
        bool allGood = true;

        // 必要なマネージャーチェック
        if (FindObjectOfType<GameManager>() == null)
        {
            Debug.LogError("❌ GameManager not found!");
            allGood = false;
        }

        if (FindObjectOfType<BattleManager>() == null)
        {
            Debug.LogError("❌ BattleManager not found!");
            allGood = false;
        }

        if (FindObjectOfType<UIManager>() == null)
        {
            Debug.LogError("❌ UIManager not found!");
            allGood = false;
        }

        // キャラクターチェック
        var player = GameObject.FindWithTag(GameConstants.PLAYER_TAG);
        if (player == null || player.GetComponent<Character>() == null)
        {
            Debug.LogError("❌ Player Character not found or missing Character component!");
            allGood = false;
        }

        var enemy = GameObject.FindWithTag(GameConstants.ENEMY_TAG);
        if (enemy == null || enemy.GetComponent<Character>() == null)
        {
            Debug.LogError("❌ Enemy Character not found or missing Character component!");
            allGood = false;
        }

        // GameConfigチェック
        var config = Resources.Load<GameConfig>("GameConfig");
        if (config == null)
        {
            Debug.LogError("❌ GameConfig not found in Resources folder!");
            allGood = false;
        }

        if (allGood)
        {
            Debug.Log("🎉 Setup complete! All systems ready.");
            Debug.Log("Press F1 to toggle debug mode in play mode.");
            Debug.Log("Press F2/F3/F4 for debug commands.");
        }
        else
        {
            Debug.LogError("❌ Setup incomplete. Fix the errors above.");
        }
    }
}
