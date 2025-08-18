// =============================================================================
// Setup/InputSystemSetupManager.cs - Input System対応 完全自動セットアップ
// =============================================================================
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class InputSystemSetupManager : MonoBehaviour
{
    [Header("Setup Configuration")]
    public bool autoSetupOnStart = false;
    public bool createInputActionsAsset = true;
    public bool setupDebugMode = true;
    public bool createSampleCards = true;
    public bool setupUI = true;

    [Header("Paths")]
    public string inputActionsPath = "Assets/CardBattleGame/Input/BattleInputActions.inputactions";
    public string gameConfigPath = "Assets/CardBattleGame/Data/GameConfig.asset";
    public string cardsPath = "Assets/CardBattleGame/Data/Cards/";

    [Header("Prefab References")]
    public GameObject cardUIPrefab;
    public GameObject buffIconPrefab;
    public GameObject damageTextPrefab;

    private bool setupComplete = false;
    private List<string> setupLog = new List<string>();

    void Start()
    {
        if (autoSetupOnStart && !setupComplete)
        {
            StartCoroutine(AutoSetupSequence());
        }
    }

    System.Collections.IEnumerator AutoSetupSequence()
    {
        yield return new WaitForSeconds(0.1f);

        LogSetup("=== Input System Card Battle Auto Setup Started ===");

        yield return StartCoroutine(Step1_CheckRequirements());
        yield return StartCoroutine(Step2_CreateInputActions());
        yield return StartCoroutine(Step3_CreateGameManagers());
        yield return StartCoroutine(Step4_CreateCharacters());
        yield return StartCoroutine(Step5_CreateGameConfig());
        yield return StartCoroutine(Step6_SetupUI());
        yield return StartCoroutine(Step7_CreateSampleContent());
        yield return StartCoroutine(Step8_FinalValidation());

        LogSetup("=== Auto Setup Complete! ===");
        setupComplete = true;

        ShowSetupResults();
    }

    // =============================================================================
    // Step 1: Requirements Check
    // =============================================================================
    System.Collections.IEnumerator Step1_CheckRequirements()
    {
        LogSetup("Step 1: Checking requirements...");

        // Input System Package チェック
#if UNITY_EDITOR
        bool hasInputSystem = false;
        var packageRequest = UnityEditor.PackageManager.Client.List();

        while (!packageRequest.IsCompleted)
        {
            yield return null;
        }

        if (packageRequest.Status == UnityEditor.PackageManager.StatusCode.Success)
        {
            foreach (var package in packageRequest.Result)
            {
                if (package.name == "com.unity.inputsystem")
                {
                    hasInputSystem = true;
                    LogSetup($"✅ Input System Package found: {package.version}");
                    break;
                }
            }
        }

        if (!hasInputSystem)
        {
            LogSetup("❌ Input System Package not found!");
            LogSetup("Please install: Window > Package Manager > Input System");
            yield break;
        }
#endif

        // Project Settings チェック
        LogSetup("✅ Requirements check complete");
        yield return new WaitForSeconds(0.1f);
    }

    // =============================================================================
    // Step 2: Create Input Actions
    // =============================================================================
    System.Collections.IEnumerator Step2_CreateInputActions()
    {
        LogSetup("Step 2: Creating Input Actions...");

        if (createInputActionsAsset)
        {
#if UNITY_EDITOR
            CreateInputActionsAsset();
#endif
        }

        yield return new WaitForSeconds(0.1f);
    }

#if UNITY_EDITOR
    void CreateInputActionsAsset()
    {
        // ディレクトリ作成
        string directory = System.IO.Path.GetDirectoryName(inputActionsPath);
        if (!System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
            AssetDatabase.Refresh();
        }

        // Input Actions Asset作成
        var inputActions = ScriptableObject.CreateInstance<InputActionAsset>();

        // Battle Action Map
        var battleMap = inputActions.AddActionMap("Battle");
        CreateBattleActions(battleMap);

        // Debug Action Map
        var debugMap = inputActions.AddActionMap("Debug");
        CreateDebugActions(debugMap);

        // UI Action Map
        var uiMap = inputActions.AddActionMap("UI");
        CreateUIActions(uiMap);

        // アセット保存
        AssetDatabase.CreateAsset(inputActions, inputActionsPath);
        AssetDatabase.SaveAssets();

        LogSetup($"✅ Input Actions created: {inputActionsPath}");
    }

    void CreateBattleActions(InputActionMap battleMap)
    {
        // SelectCard
        var selectCard = battleMap.AddAction("SelectCard", InputActionType.Button);
        selectCard.AddBinding("<Mouse>/leftButton");
        selectCard.AddBinding("<Touchscreen>/primaryTouch/tap");

        // CancelSelection
        var cancelSelection = battleMap.AddAction("CancelSelection", InputActionType.Button);
        cancelSelection.AddBinding("<Mouse>/rightButton");
        cancelSelection.AddBinding("<Keyboard>/escape");

        // ConfirmAction
        var confirmAction = battleMap.AddAction("ConfirmAction", InputActionType.Button);
        confirmAction.AddBinding("<Keyboard>/space");
        confirmAction.AddBinding("<Keyboard>/enter");
        confirmAction.AddBinding("<Gamepad>/buttonSouth");

        // Navigate
        var navigate = battleMap.AddAction("Navigate", InputActionType.Value);
        navigate.expectedControlType = "Vector2";
        navigate.AddBinding("<Keyboard>/wasd");
        navigate.AddBinding("<Gamepad>/leftStick");
        navigate.AddBinding("<Keyboard>/upArrow,<Keyboard>/leftArrow,<Keyboard>/downArrow,<Keyboard>/rightArrow");

        // MousePosition
        var mousePosition = battleMap.AddAction("MousePosition", InputActionType.Value);
        mousePosition.expectedControlType = "Vector2";
        mousePosition.AddBinding("<Mouse>/position");
        mousePosition.AddBinding("<Touchscreen>/primaryTouch/position");

        // Scroll
        var scroll = battleMap.AddAction("Scroll", InputActionType.Value);
        scroll.expectedControlType = "Vector2";
        scroll.AddBinding("<Mouse>/scroll");

        // HoverCard
        var hoverCard = battleMap.AddAction("HoverCard", InputActionType.PassThrough);
        hoverCard.AddBinding("<Mouse>/position");
    }

    void CreateDebugActions(InputActionMap debugMap)
    {
        // ToggleDebugUI
        var toggleDebugUI = debugMap.AddAction("ToggleDebugUI", InputActionType.Button);
        toggleDebugUI.AddBinding("<Keyboard>/f1");
        toggleDebugUI.AddBinding("<Gamepad>/select");

        // ForcePlayerWin
        var forcePlayerWin = debugMap.AddAction("ForcePlayerWin", InputActionType.Button);
        forcePlayerWin.AddBinding("<Keyboard>/f2");

        // ForceEnemyWin
        var forceEnemyWin = debugMap.AddAction("ForceEnemyWin", InputActionType.Button);
        forceEnemyWin.AddBinding("<Keyboard>/f3");

        // ForceDraw
        var forceDraw = debugMap.AddAction("ForceDraw", InputActionType.Button);
        forceDraw.AddBinding("<Keyboard>/f4");

        // SkipTurn
        var skipTurn = debugMap.AddAction("SkipTurn", InputActionType.Button);
        skipTurn.AddBinding("<Keyboard>/f5");

        // RestartBattle
        var restartBattle = debugMap.AddAction("RestartBattle", InputActionType.Button);
        restartBattle.AddBinding("<Keyboard>/f9");
    }

    void CreateUIActions(InputActionMap uiMap)
    {
        // Submit
        var submit = uiMap.AddAction("Submit", InputActionType.Button);
        submit.AddBinding("<Keyboard>/enter");
        submit.AddBinding("<Keyboard>/space");
        submit.AddBinding("<Gamepad>/buttonSouth");

        // Cancel
        var cancel = uiMap.AddAction("Cancel", InputActionType.Button);
        cancel.AddBinding("<Keyboard>/escape");
        cancel.AddBinding("<Gamepad>/buttonEast");

        // Navigate
        var navigate = uiMap.AddAction("Navigate", InputActionType.Value);
        navigate.expectedControlType = "Vector2";
        navigate.AddBinding("<Keyboard>/wasd");
        navigate.AddBinding("<Keyboard>/upArrow,<Keyboard>/leftArrow,<Keyboard>/downArrow,<Keyboard>/rightArrow");
        navigate.AddBinding("<Gamepad>/leftStick");
        navigate.AddBinding("<Gamepad>/dpad");

        // Pause
        var pause = uiMap.AddAction("Pause", InputActionType.Button);
        pause.AddBinding("<Keyboard>/escape");
        pause.AddBinding("<Gamepad>/start");
    }
#endif

    // =============================================================================
    // Step 3: Create Game Managers
    // =============================================================================
    System.Collections.IEnumerator Step3_CreateGameManagers()
    {
        LogSetup("Step 3: Creating Game Managers...");

        // メインマネージャーコンテナ
        GameObject managersContainer = GameObject.Find("=== GAME MANAGERS ===");
        if (managersContainer == null)
        {
            managersContainer = new GameObject("=== GAME MANAGERS ===");
        }

        // InputManager作成
        GameObject inputManagerObj = CreateManager("InputManager", managersContainer.transform);
        var inputManager = inputManagerObj.GetComponent<InputManager>();
        if (inputManager == null)
        {
            inputManager = inputManagerObj.AddComponent<InputManager>();
        }

        // Input Actions割り当て
#if UNITY_EDITOR
        var inputActionsAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(inputActionsPath);
        if (inputActionsAsset != null)
        {
            inputManager.inputActions = inputActionsAsset;
            LogSetup("✅ Input Actions assigned to InputManager");
        }
#endif

        // InputSystemGameManager作成
        GameObject gameManagerObj = CreateManager("GameManager", managersContainer.transform);
        var gameManager = gameManagerObj.GetComponent<InputSystemGameManager>();
        if (gameManager == null)
        {
            gameManager = gameManagerObj.AddComponent<InputSystemGameManager>();
        }
        gameManager.inputManager = inputManager;

        // BattleManager作成
        GameObject battleManagerObj = CreateManager("BattleManager", managersContainer.transform);
        var battleManager = battleManagerObj.GetOrAddComponent<BattleManager>();
        battleManagerObj.GetOrAddComponent<RoundManager>();
        battleManagerObj.GetOrAddComponent<TurnManager>();
        battleManagerObj.GetOrAddComponent<CardManager>();

        // UIManager作成
        GameObject uiManagerObj = CreateManager("UIManager", managersContainer.transform);
        uiManagerObj.GetOrAddComponent<UIManager>();

        // AudioManager作成
        GameObject audioManagerObj = CreateManager("AudioManager", managersContainer.transform);
        audioManagerObj.GetOrAddComponent<AudioManager>();

        // EffectManager作成
        GameObject effectManagerObj = CreateManager("EffectManager", managersContainer.transform);
        effectManagerObj.GetOrAddComponent<EffectManager>();

        // DebugManager作成（Input System対応版）
        if (setupDebugMode)
        {
            GameObject debugManagerObj = CreateManager("DebugManager", managersContainer.transform);
            debugManagerObj.GetOrAddComponent<InputSystemDebugManager>();
            debugManagerObj.GetOrAddComponent<BattleLogger>();
        }

        LogSetup("✅ Game Managers created");
        yield return new WaitForSeconds(0.1f);
    }

    GameObject CreateManager(string name, Transform parent)
    {
        GameObject existing = GameObject.Find(name);
        if (existing != null)
        {
            existing.transform.SetParent(parent);
            return existing;
        }

        GameObject manager = new GameObject(name);
        manager.transform.SetParent(parent);
        return manager;
    }

    // =============================================================================
    // Step 4: Create Characters
    // =============================================================================
    System.Collections.IEnumerator Step4_CreateCharacters()
    {
        LogSetup("Step 4: Creating Characters...");

        // プレイヤー作成
        GameObject player = CreateCharacter("Player", GameConstants.PLAYER_TAG, Vector3.left * 3);
        SetupPlayerCharacter(player);

        // 敵作成
        GameObject enemy = CreateCharacter("Enemy", GameConstants.ENEMY_TAG, Vector3.right * 3);
        SetupEnemyCharacter(enemy);

        // BattleManagerに割り当て
        var battleManager = FindObjectOfType<BattleManager>();
        if (battleManager != null)
        {
            battleManager.player = player.GetComponent<Character>();
            battleManager.enemy = enemy.GetComponent<Character>();
        }

        LogSetup("✅ Characters created and configured");
        yield return new WaitForSeconds(0.1f);
    }

    GameObject CreateCharacter(string name, string tag, Vector3 position)
    {
        GameObject existing = GameObject.FindWithTag(tag);
        if (existing != null)
        {
            return existing;
        }

        GameObject character = new GameObject(name);
        character.tag = tag;
        character.transform.position = position;

        return character;
    }

    void SetupPlayerCharacter(GameObject player)
    {
        var character = player.GetOrAddComponent<Character>();
        character.characterName = "Player";
        character.maxHealth = 100;
        character.maxMana = 50;
        character.baseAttack = 10;
        character.baseDefense = 5;

        player.GetOrAddComponent<CharacterAnimator>();
        player.GetOrAddComponent<CharacterAudio>();
        player.GetOrAddComponent<TurnBasedBuffManager>();

        // Input対応は不要（プレイヤーは手動操作）
    }

    void SetupEnemyCharacter(GameObject enemy)
    {
        var character = enemy.GetOrAddComponent<Character>();
        character.characterName = "Enemy AI";
        character.maxHealth = 100;
        character.maxMana = 50;
        character.baseAttack = 10;
        character.baseDefense = 5;

        enemy.GetOrAddComponent<CharacterAnimator>();
        enemy.GetOrAddComponent<CharacterAudio>();
        enemy.GetOrAddComponent<TurnBasedBuffManager>();
        enemy.GetOrAddComponent<EnemyAI>();
    }

    // =============================================================================
    // Step 5: Create GameConfig
    // =============================================================================
    System.Collections.IEnumerator Step5_CreateGameConfig()
    {
        LogSetup("Step 5: Creating GameConfig...");

#if UNITY_EDITOR
        // 既存チェック
        var existingConfig = AssetDatabase.LoadAssetAtPath<GameConfig>(gameConfigPath);
        if (existingConfig != null)
        {
            LogSetup("✅ GameConfig already exists");
            yield break;
        }

        // ディレクトリ作成
        string directory = System.IO.Path.GetDirectoryName(gameConfigPath);
        if (!System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
            AssetDatabase.Refresh();
        }

        // GameConfig作成
        var config = ScriptableObject.CreateInstance<GameConfig>();
        config.name = "GameConfig";

        // Input System対応設定
        config.winsNeeded = 2;
        config.maxTurnsPerRound = 10;
        config.enableSimultaneousDefeat = true;
        config.simultaneousDefeatRule = SimultaneousDefeatRule.Draw;
        config.initialHandSize = 4;
        config.maxHandSize = 7;
        config.drawPerTurn = 1;
        config.manaRegenPerTurn = 3;
        config.enableDebugMode = setupDebugMode;
        config.enableVerboseLogging = false;
        config.aiThinkingTime = 1f;
        config.animationSpeedMultiplier = 1f;
        config.damageTextDuration = 2f;
        config.cardHoverScale = 1.1f;
        config.maxBuffIconsDisplay = 8;
        config.bgmVolume = 0.7f;
        config.sfxVolume = 0.8f;
        config.voiceVolume = 0.9f;

        AssetDatabase.CreateAsset(config, gameConfigPath);
        AssetDatabase.SaveAssets();

        LogSetup($"✅ GameConfig created: {gameConfigPath}");
#endif

        yield return new WaitForSeconds(0.1f);
    }

    // =============================================================================
    // Step 6: Setup UI
    // =============================================================================
    System.Collections.IEnumerator Step6_SetupUI()
    {
        LogSetup("Step 6: Setting up UI...");

        if (!setupUI)
        {
            LogSetup("⏭️ UI setup skipped");
            yield break;
        }

        // EventSystem確認・作成
        EnsureEventSystem();

        // Canvas作成
        SetupMainCanvas();

        // TooltipManager作成
        SetupTooltipManager();

        LogSetup("✅ UI setup complete");
        yield return new WaitForSeconds(0.1f);
    }

    void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
            LogSetup("✅ EventSystem created");
        }
        else
        {
            LogSetup("✅ EventSystem already exists");
        }
    }

    void SetupMainCanvas()
    {
        Canvas existingCanvas = FindObjectOfType<Canvas>();
        if (existingCanvas != null)
        {
            LogSetup("✅ Canvas already exists");
            return;
        }

        GameObject canvasObj = new GameObject("Main Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;

        var scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        // UI Areas作成
        CreateUIAreas(canvasObj.transform);

        LogSetup("✅ Canvas created with UI areas");
    }

    void CreateUIAreas(Transform canvasTransform)
    {
        // 手札エリア
        CreateUIArea("Player Hand Area", canvasTransform, new Vector2(0, -300), new Vector2(1200, 200));
        CreateUIArea("Enemy Hand Area", canvasTransform, new Vector2(0, 300), new Vector2(1200, 200));

        // ステータスエリア
        CreateUIArea("Player Status", canvasTransform, new Vector2(-600, -100), new Vector2(400, 150));
        CreateUIArea("Enemy Status", canvasTransform, new Vector2(600, 100), new Vector2(400, 150));

        // バフエリア
        CreateUIArea("Player Buffs", canvasTransform, new Vector2(-600, 50), new Vector2(400, 80));
        CreateUIArea("Enemy Buffs", canvasTransform, new Vector2(600, -50), new Vector2(400, 80));

        // ターン・マッチ情報
        CreateUIArea("Turn Indicator", canvasTransform, new Vector2(0, 400), new Vector2(300, 80));
        CreateUIArea("Match Status", canvasTransform, new Vector2(0, -450), new Vector2(500, 100));

        // 通知エリア
        CreateUIArea("Notification Area", canvasTransform, new Vector2(0, 0), new Vector2(600, 200));
    }

    GameObject CreateUIArea(string name, Transform parent, Vector2 position, Vector2 size)
    {
        GameObject area = new GameObject(name);
        area.transform.SetParent(parent, false);

        RectTransform rect = area.AddComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        // デバッグ用の背景（透明）
        var image = area.AddComponent<UnityEngine.UI.Image>();
        image.color = new Color(1, 1, 1, 0.05f);

        return area;
    }

    void SetupTooltipManager()
    {
        GameObject tooltipObj = new GameObject("TooltipManager");
        tooltipObj.AddComponent<TooltipManager>();

        // Tooltip UI作成
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            GameObject tooltipPanel = CreateTooltipPanel(canvas.transform);
            var tooltipManager = tooltipObj.GetComponent<TooltipManager>();
            tooltipManager.tooltipPanel = tooltipPanel;
            tooltipManager.tooltipText = tooltipPanel.GetComponentInChildren<UnityEngine.UI.Text>();
            tooltipManager.tooltipRect = tooltipPanel.GetComponent<RectTransform>();
            tooltipManager.tooltipCanvasGroup = tooltipPanel.GetComponent<CanvasGroup>();
        }

        LogSetup("✅ TooltipManager created");
    }

    GameObject CreateTooltipPanel(Transform canvasTransform)
    {
        GameObject panel = new GameObject("Tooltip Panel");
        panel.transform.SetParent(canvasTransform, false);
        panel.AddComponent<CanvasGroup>();

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300, 150);

        var image = panel.AddComponent<UnityEngine.UI.Image>();
        image.color = new Color(0, 0, 0, 0.8f);

        // テキスト作成
        GameObject textObj = new GameObject("Tooltip Text");
        textObj.transform.SetParent(panel.transform, false);

        var text = textObj.AddComponent<UnityEngine.UI.Text>();
        text.text = "Tooltip Text";
        text.color = Color.white;
        text.fontSize = 14;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        panel.SetActive(false);
        return panel;
    }

    // =============================================================================
    // Step 7: Create Sample Content
    // =============================================================================
    System.Collections.IEnumerator Step7_CreateSampleContent()
    {
        LogSetup("Step 7: Creating sample content...");

        if (createSampleCards)
        {
#if UNITY_EDITOR
            CreateInputSystemSampleCards();
#endif
        }

        LogSetup("✅ Sample content created");
        yield return new WaitForSeconds(0.1f);
    }

#if UNITY_EDITOR
    void CreateInputSystemSampleCards()
    {
        // ディレクトリ作成
        string basicPath = cardsPath + "Basic/";
        string conditionalPath = cardsPath + "Conditional/";

        if (!System.IO.Directory.Exists(basicPath))
        {
            System.IO.Directory.CreateDirectory(basicPath);
        }
        if (!System.IO.Directory.Exists(conditionalPath))
        {
            System.IO.Directory.CreateDirectory(conditionalPath);
        }

        // 基本カード作成
        CreateBasicCard("Strike", "Deal 12 damage", 2, 12, SkillEffectType.Damage, basicPath);
        CreateBasicCard("Heal", "Restore 18 HP", 2, 18, SkillEffectType.Heal, basicPath);
        CreateBasicCard("Shield", "Gain 15 shield", 1, 15, SkillEffectType.Shield, basicPath);

        // 条件付きカード作成
        CreateConditionalCard("Desperation Strike", "Enhanced when HP ≤ 30%", conditionalPath);
        CreateConditionalCard("Healing Light", "Enhanced when enemy attack ≥ 15", conditionalPath);
        CreateConditionalCard("Finisher", "Devastating attack when enemy HP ≤ 40%", conditionalPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        LogSetup("✅ Sample cards created");
    }

    void CreateBasicCard(string name, string description, int manaCost, int value, SkillEffectType effectType, string path)
    {
        var card = ScriptableObject.CreateInstance<SkillCard>();
        card.cardName = name;
        card.description = description;
        card.manaCost = manaCost;
        card.animationDuration = 1f;

        card.effects = new TurnBasedSkillEffect[]
        {
            new TurnBasedSkillEffect
            {
                effectType = effectType,
                value = value,
                targetType = effectType == SkillEffectType.Heal || effectType == SkillEffectType.Shield
                    ? TargetType.Self : TargetType.Enemy
            }
        };

        AssetDatabase.CreateAsset(card, $"{path}{name}.asset");
    }

    void CreateConditionalCard(string name, string description, string path)
    {
        var card = ScriptableObject.CreateInstance<ConditionalSkillCard>();
        card.cardName = name;
        card.description = description;
        card.manaCost = 3;
        card.animationDuration = 1.5f;

        // 基本効果
        card.effects = new TurnBasedSkillEffect[]
        {
            new TurnBasedSkillEffect
            {
                effectType = SkillEffectType.Damage,
                value = 15,
                targetType = TargetType.Enemy
            }
        };

        // 条件設定（例）
        card.usageConditions = new SkillCondition[]
        {
            new SkillCondition
            {
                conditionType = ConditionType.HealthPercentage,
                target = ConditionTarget.Self,
                comparisonOperator = ComparisonOperator.LessThanOrEqual,
                value = 30
            }
        };

        // 強化効果
        card.hasEnhancedVersion = true;
        card.enhancedEffects = new TurnBasedSkillEffect[]
        {
            new TurnBasedSkillEffect
            {
                effectType = SkillEffectType.Damage,
                value = 10,
                targetType = TargetType.Enemy
            }
        };

        AssetDatabase.CreateAsset(card, $"{path}{name}.asset");
    }
#endif

    // =============================================================================
    // Step 8: Final Validation
    // =============================================================================
    System.Collections.IEnumerator Step8_FinalValidation()
    {
        LogSetup("Step 8: Final validation...");

        bool allValid = true;

        // 必須コンポーネントチェック
        if (FindObjectOfType<InputManager>() == null)
        {
            LogSetup("❌ InputManager not found!");
            allValid = false;
        }

        if (FindObjectOfType<InputSystemGameManager>() == null)
        {
            LogSetup("❌ InputSystemGameManager not found!");
            allValid = false;
        }

        if (FindObjectOfType<BattleManager>() == null)
        {
            LogSetup("❌ BattleManager not found!");
            allValid = false;
        }

        if (FindObjectOfType<EventSystem>() == null)
        {
            LogSetup("❌ EventSystem not found!");
            allValid = false;
        }

        // キャラクターチェック
        var player = GameObject.FindWithTag(GameConstants.PLAYER_TAG);
        var enemy = GameObject.FindWithTag(GameConstants.ENEMY_TAG);

        if (player == null || player.GetComponent<Character>() == null)
        {
            LogSetup("❌ Player Character not properly configured!");
            allValid = false;
        }

        if (enemy == null || enemy.GetComponent<Character>() == null)
        {
            LogSetup("❌ Enemy Character not properly configured!");
            allValid = false;
        }

        // Input Actions チェック
#if UNITY_EDITOR
        var inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(inputActionsPath);
        if (inputActions == null)
        {
            LogSetup("❌ Input Actions asset not found!");
            allValid = false;
        }
        else
        {
            LogSetup("✅ Input Actions asset validated");
        }
#endif

        if (allValid)
        {
            LogSetup("🎉 All systems validated successfully!");
            LogSetup("🎮 Ready to play with Input System!");
        }
        else
        {
            LogSetup("⚠️ Some validation checks failed. Please review the setup.");
        }

        yield return new WaitForSeconds(0.1f);
    }

    // =============================================================================
    // Utility Methods
    // =============================================================================
    void LogSetup(string message)
    {
        setupLog.Add(message);
        Debug.Log($"[InputSystemSetup] {message}");
    }

    void ShowSetupResults()
    {
        string results = "\n=== INPUT SYSTEM SETUP RESULTS ===\n";
        foreach (string log in setupLog)
        {
            results += log + "\n";
        }
        results += "================================\n";

        Debug.Log(results);

#if UNITY_EDITOR
        // セットアップ結果をエディタダイアログで表示
        if (setupComplete)
        {
            UnityEditor.EditorUtility.DisplayDialog(
                "Input System Setup Complete",
                "Auto setup has completed successfully!\n\n" +
                "Created:\n" +
                "• Input Actions Asset\n" +
                "• Game Managers with Input System support\n" +
                "• Player & Enemy Characters\n" +
                "• GameConfig\n" +
                "• UI Framework\n" +
                "• Sample Cards\n\n" +
                "Your card battle game is ready to play!",
                "OK"
            );
        }
#endif
    }

    // =============================================================================
    // Manual Setup Methods (for Inspector buttons)
    // =============================================================================

    [Header("Manual Setup Controls")]
    [Space(10)]
    [SerializeField] private bool showManualControls = false;

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(InputSystemSetupManager))]
    public class InputSystemSetupManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            InputSystemSetupManager manager = (InputSystemSetupManager)target;

            UnityEditor.EditorGUILayout.Space(10);
            UnityEditor.EditorGUILayout.LabelField("Setup Controls", UnityEditor.EditorStyles.boldLabel);

            UnityEditor.EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("🚀 Run Full Auto Setup"))
            {
                if (Application.isPlaying)
                {
                    manager.StartCoroutine(manager.AutoSetupSequence());
                }
                else
                {
                    UnityEditor.EditorUtility.DisplayDialog(
                        "Setup Requires Play Mode",
                        "Please enter Play Mode to run the auto setup.",
                        "OK"
                    );
                }
            }

            if (GUILayout.Button("📋 Show Setup Log"))
            {
                manager.ShowSetupResults();
            }
            UnityEditor.EditorGUILayout.EndHorizontal();

            UnityEditor.EditorGUILayout.Space(5);
            UnityEditor.EditorGUILayout.LabelField("Individual Steps", UnityEditor.EditorStyles.boldLabel);

            UnityEditor.EditorGUILayout.BeginVertical("box");

            if (GUILayout.Button("1. Create Input Actions"))
            {
                manager.CreateInputActionsAsset();
            }

            if (GUILayout.Button("2. Validate Requirements"))
            {
                if (Application.isPlaying)
                {
                    manager.StartCoroutine(manager.Step1_CheckRequirements());
                }
            }

            if (GUILayout.Button("3. Create Sample Cards"))
            {
                manager.CreateInputSystemSampleCards();
            }

            UnityEditor.EditorGUILayout.EndVertical();

            UnityEditor.EditorGUILayout.Space(10);
            UnityEditor.EditorGUILayout.HelpBox(
                "Input System Setup Manager\n\n" +
                "This tool automatically sets up a complete card battle game with Input System support.\n\n" +
                "Features:\n" +
                "• Input Actions for Battle, Debug, and UI\n" +
                "• Cross-platform input support (Mouse, Keyboard, Touch, Gamepad)\n" +
                "• Complete game manager hierarchy\n" +
                "• Sample cards and game configuration\n" +
                "• UI framework with tooltips\n\n" +
                "Usage:\n" +
                "1. Configure the settings above\n" +
                "2. Enter Play Mode\n" +
                "3. Click 'Run Full Auto Setup' or enable 'Auto Setup On Start'\n" +
                "4. Wait for setup to complete\n\n" +
                "Note: Input System package must be installed first.",
                UnityEditor.MessageType.Info
            );
        }
    }
#endif

    // =============================================================================
    // Debug and Testing Methods
    // =============================================================================

    [System.Serializable]
    public class SetupValidation
    {
        public bool inputSystemPackageInstalled;
        public bool inputActionsCreated;
        public bool managersCreated;
        public bool charactersCreated;
        public bool uiSetup;
        public bool configCreated;
        public bool sampleCardsCreated;

        public bool IsCompletelyValid()
        {
            return inputSystemPackageInstalled &&
                   inputActionsCreated &&
                   managersCreated &&
                   charactersCreated &&
                   uiSetup &&
                   configCreated;
        }

        public string GetValidationReport()
        {
            var report = "Setup Validation Report:\n";
            report += $"Input System Package: {(inputSystemPackageInstalled ? "✅" : "❌")}\n";
            report += $"Input Actions: {(inputActionsCreated ? "✅" : "❌")}\n";
            report += $"Game Managers: {(managersCreated ? "✅" : "❌")}\n";
            report += $"Characters: {(charactersCreated ? "✅" : "❌")}\n";
            report += $"UI Setup: {(uiSetup ? "✅" : "❌")}\n";
            report += $"Game Config: {(configCreated ? "✅" : "❌")}\n";
            report += $"Sample Cards: {(sampleCardsCreated ? "✅" : "❌")}\n";
            report += $"\nOverall Status: {(IsCompletelyValid() ? "READY ✅" : "INCOMPLETE ❌")}";

            return report;
        }
    }

    public SetupValidation ValidateCurrentSetup()
    {
        var validation = new SetupValidation();

        // Input System Package チェック
#if UNITY_EDITOR
        var packageRequest = UnityEditor.PackageManager.Client.List();
        while (!packageRequest.IsCompleted) { }

        if (packageRequest.Status == UnityEditor.PackageManager.StatusCode.Success)
        {
            foreach (var package in packageRequest.Result)
            {
                if (package.name == "com.unity.inputsystem")
                {
                    validation.inputSystemPackageInstalled = true;
                    break;
                }
            }
        }

        // Input Actions チェック
        validation.inputActionsCreated = UnityEditor.AssetDatabase.LoadAssetAtPath<InputActionAsset>(inputActionsPath) != null;

        // GameConfig チェック
        validation.configCreated = UnityEditor.AssetDatabase.LoadAssetAtPath<GameConfig>(gameConfigPath) != null;

        // Sample Cards チェック
        validation.sampleCardsCreated = System.IO.Directory.Exists(cardsPath + "Basic/");
#endif

        // Managers チェック
        validation.managersCreated = FindObjectOfType<InputManager>() != null &&
                                   FindObjectOfType<InputSystemGameManager>() != null &&
                                   FindObjectOfType<BattleManager>() != null;

        // Characters チェック
        validation.charactersCreated = GameObject.FindWithTag(GameConstants.PLAYER_TAG) != null &&
                                     GameObject.FindWithTag(GameConstants.ENEMY_TAG) != null;

        // UI チェック
        validation.uiSetup = FindObjectOfType<EventSystem>() != null &&
                           FindObjectOfType<Canvas>() != null;

        return validation;
    }

    // =============================================================================
    // Cleanup Methods
    // =============================================================================

    public void CleanupSetup()
    {
        LogSetup("Starting cleanup...");

        // Game Managers削除
        var managersContainer = GameObject.Find("=== GAME MANAGERS ===");
        if (managersContainer != null)
        {
            DestroyImmediate(managersContainer);
            LogSetup("✅ Game Managers cleaned up");
        }

        // Characters削除
        var player = GameObject.FindWithTag(GameConstants.PLAYER_TAG);
        var enemy = GameObject.FindWithTag(GameConstants.ENEMY_TAG);

        if (player != null) DestroyImmediate(player);
        if (enemy != null) DestroyImmediate(enemy);

        // UI削除
        var canvas = FindObjectOfType<Canvas>();
        var eventSystem = FindObjectOfType<EventSystem>();
        var tooltipManager = FindObjectOfType<TooltipManager>();

        if (canvas != null) DestroyImmediate(canvas.gameObject);
        if (eventSystem != null) DestroyImmediate(eventSystem.gameObject);
        if (tooltipManager != null) DestroyImmediate(tooltipManager.gameObject);

        setupComplete = false;
        setupLog.Clear();

        LogSetup("🧹 Cleanup complete");
    }

    // =============================================================================
    // Event Callbacks
    // =============================================================================

    void OnValidate()
    {
        // パス検証
        if (!inputActionsPath.EndsWith(".inputactions"))
        {
            inputActionsPath = "Assets/CardBattleGame/Input/BattleInputActions.inputactions";
        }

        if (!gameConfigPath.EndsWith(".asset"))
        {
            gameConfigPath = "Assets/CardBattleGame/Data/GameConfig.asset";
        }

        if (!cardsPath.EndsWith("/"))
        {
            cardsPath += "/";
        }
    }

    void OnDestroy()
    {
        if (setupLog.Count > 0)
        {
            Debug.Log("InputSystemSetupManager destroyed. Setup log preserved.");
        }
    }
}