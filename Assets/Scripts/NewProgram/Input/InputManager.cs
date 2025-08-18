using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.StandaloneInputModule;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("Input Actions")]
    public InputActionAsset inputActions;

    // Input Action Maps
    private InputActionMap battleMap;
    private InputActionMap debugMap;
    private InputActionMap uiMap;

    // Battle Actions
    private InputAction selectCardAction;
    private InputAction cancelSelectionAction;
    private InputAction confirmActionAction;
    private InputAction navigateAction;
    private InputAction mousePositionAction;
    private InputAction scrollAction;
    private InputAction hoverCardAction;

    // Debug Actions
    private InputAction toggleDebugUIAction;
    private InputAction forcePlayerWinAction;
    private InputAction forceEnemyWinAction;
    private InputAction forceDrawAction;
    private InputAction skipTurnAction;
    private InputAction restartBattleAction;

    // UI Actions
    private InputAction submitAction;
    private InputAction cancelAction;
    private InputAction uiNavigateAction;
    private InputAction pauseAction;

    // Current Input State
    public Vector2 MousePosition { get; private set; }
    public Vector2 NavigateInput { get; private set; }
    public Vector2 ScrollDelta { get; private set; }
    public bool IsPointerOverUI { get; private set; }

    // Events
    public static event Action OnCardSelect;
    public static event Action OnCancelSelection;
    public static event Action OnConfirmAction;
    public static event Action<Vector2> OnNavigate;
    public static event Action<Vector2> OnMouseMove;
    public static event Action<Vector2> OnScroll;

    // Debug Events
    public static event Action OnToggleDebugUI;
    public static event Action OnForcePlayerWin;
    public static event Action OnForceEnemyWin;
    public static event Action OnForceDraw;
    public static event Action OnSkipTurn;
    public static event Action OnRestartBattle;

    // UI Events
    public static event Action OnSubmit;
    public static event Action OnCancel;
    public static event Action<Vector2> OnUINavigate;
    public static event Action OnPause;

    [Header("Settings")]
    public bool enableDebugInputs = true;
    public float mouseSensitivity = 1f;
    public float scrollSensitivity = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeInputSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeInputSystem()
    {
        if (inputActions == null)
        {
            Debug.LogError("InputActions asset is not assigned!");
            return;
        }

        // Get Action Maps
        battleMap = inputActions.FindActionMap("Battle");
        debugMap = inputActions.FindActionMap("Debug");
        uiMap = inputActions.FindActionMap("UI");

        SetupBattleActions();
        //SetupDebugActions();
        SetupUIActions();

        // Enable default maps
        EnableBattleInput();
        if (enableDebugInputs)
            EnableDebugInput();
    }

    void SetupBattleActions()
    {
        if (battleMap == null) return;

        // Get Actions
        selectCardAction = battleMap.FindAction("SelectCard");
        cancelSelectionAction = battleMap.FindAction("CancelSelection");
        confirmActionAction = battleMap.FindAction("ConfirmAction");
        navigateAction = battleMap.FindAction("Navigate");
        mousePositionAction = battleMap.FindAction("MousePosition");
        scrollAction = battleMap.FindAction("Scroll");
        hoverCardAction = battleMap.FindAction("HoverCard");

        // Setup Callbacks
        selectCardAction.performed += OnSelectCardPerformed;
        cancelSelectionAction.performed += OnCancelSelectionPerformed;
        confirmActionAction.performed += OnConfirmActionPerformed;
        navigateAction.performed += OnNavigatePerformed;
        navigateAction.canceled += OnNavigateCanceled;
        mousePositionAction.performed += OnMousePositionPerformed;
        scrollAction.performed += OnScrollPerformed;
    }

    void SetupDebugActions()
    {
        if (debugMap == null) return;

        // Get Actions
        toggleDebugUIAction = debugMap.FindAction("ToggleDebugUI");
        //forcePlayerWinAction = debugMap.FindAction("ForcePlayerWin");
        //forceEnemyWinAction = debugMap.FindAction("ForceEnemyWin");
        //forceDrawAction = debugMap.FindAction("ForceDraw");
        //skipTurnAction = debugMap.FindAction("SkipTurn");
        //restartBattleAction = debugMap.FindAction("RestartBattle");

        // Setup Callbacks
        toggleDebugUIAction.performed += OnToggleDebugUIPerformed;
        //forcePlayerWinAction.performed += OnForcePlayerWinPerformed;
        //forceEnemyWinAction.performed += OnForceEnemyWinPerformed;
        //forceDrawAction.performed += OnForceDrawPerformed;
        //skipTurnAction.performed += OnSkipTurnPerformed;
        //restartBattleAction.performed += OnRestartBattlePerformed;
    }

    void SetupUIActions()
    {
        if (uiMap == null) return;

        // Get Actions
        submitAction = uiMap.FindAction("Submit");
        cancelAction = uiMap.FindAction("Cancel");
        uiNavigateAction = uiMap.FindAction("Navigate");
        pauseAction = uiMap.FindAction("Pause");

        // Setup Callbacks
        submitAction.performed += OnSubmitPerformed;
        cancelAction.performed += OnCancelPerformed;
        uiNavigateAction.performed += OnUINavigatePerformed;
        pauseAction.performed += OnPausePerformed;
    }

    // =============================================================================
    // Input Map Management
    // =============================================================================
    public void EnableBattleInput()
    {
        battleMap?.Enable();
        Debug.Log("Battle input enabled");
    }

    public void DisableBattleInput()
    {
        battleMap?.Disable();
        Debug.Log("Battle input disabled");
    }

    public void EnableDebugInput()
    {
        if (enableDebugInputs)
        {
            debugMap?.Enable();
            Debug.Log("Debug input enabled");
        }
    }

    public void DisableDebugInput()
    {
        debugMap?.Disable();
        Debug.Log("Debug input disabled");
    }

    public void EnableUIInput()
    {
        uiMap?.Enable();
        Debug.Log("UI input enabled");
    }

    public void DisableUIInput()
    {
        uiMap?.Disable();
        Debug.Log("UI input disabled");
    }

    public void SetInputMode(InputMode mode)
    {
        DisableAllInput();

        switch (mode)
        {
            case InputMode.Battle:
                EnableBattleInput();
                if (enableDebugInputs) EnableDebugInput();
                break;
            case InputMode.UI:
                EnableUIInput();
                if (enableDebugInputs) EnableDebugInput();
                break;
            case InputMode.Paused:
                EnableUIInput();
                if (enableDebugInputs) EnableDebugInput();
                break;
            case InputMode.GameOver:
                EnableUIInput();
                break;
        }
    }

    public void DisableAllInput()
    {
        DisableBattleInput();
        DisableDebugInput();
        DisableUIInput();
    }

    // =============================================================================
    // Battle Input Callbacks
    // =============================================================================
    void OnSelectCardPerformed(InputAction.CallbackContext context)
    {
        // UI上でなければカード選択
        if (!IsPointerOverUI)
        {
            OnCardSelect?.Invoke();
        }
    }

    void OnCancelSelectionPerformed(InputAction.CallbackContext context)
    {
        OnCancelSelection?.Invoke();
    }

    void OnConfirmActionPerformed(InputAction.CallbackContext context)
    {
        OnConfirmAction?.Invoke();
    }

    void OnNavigatePerformed(InputAction.CallbackContext context)
    {
        NavigateInput = context.ReadValue<Vector2>();
        OnNavigate?.Invoke(NavigateInput);
    }

    void OnNavigateCanceled(InputAction.CallbackContext context)
    {
        NavigateInput = Vector2.zero;
        OnNavigate?.Invoke(NavigateInput);
    }

    void OnMousePositionPerformed(InputAction.CallbackContext context)
    {
        MousePosition = context.ReadValue<Vector2>();
        OnMouseMove?.Invoke(MousePosition);

        // UI判定更新
        UpdateUIOverlayCheck();
    }

    void OnScrollPerformed(InputAction.CallbackContext context)
    {
        ScrollDelta = context.ReadValue<Vector2>() * scrollSensitivity;
        OnScroll?.Invoke(ScrollDelta);
    }

    // =============================================================================
    // Debug Input Callbacks
    // =============================================================================
    void OnToggleDebugUIPerformed(InputAction.CallbackContext context)
    {
        OnToggleDebugUI?.Invoke();
    }

    void OnForcePlayerWinPerformed(InputAction.CallbackContext context)
    {
        OnForcePlayerWin?.Invoke();
    }

    void OnForceEnemyWinPerformed(InputAction.CallbackContext context)
    {
        OnForceEnemyWin?.Invoke();
    }

    void OnForceDrawPerformed(InputAction.CallbackContext context)
    {
        OnForceDraw?.Invoke();
    }

    void OnSkipTurnPerformed(InputAction.CallbackContext context)
    {
        OnSkipTurn?.Invoke();
    }

    void OnRestartBattlePerformed(InputAction.CallbackContext context)
    {
        OnRestartBattle?.Invoke();
    }

    // =============================================================================
    // UI Input Callbacks
    // =============================================================================
    void OnSubmitPerformed(InputAction.CallbackContext context)
    {
        OnSubmit?.Invoke();
    }

    void OnCancelPerformed(InputAction.CallbackContext context)
    {
        OnCancel?.Invoke();
    }

    void OnUINavigatePerformed(InputAction.CallbackContext context)
    {
        Vector2 navigate = context.ReadValue<Vector2>();
        OnUINavigate?.Invoke(navigate);
    }

    void OnPausePerformed(InputAction.CallbackContext context)
    {
        OnPause?.Invoke();
    }

    // =============================================================================
    // Utility Methods
    // =============================================================================
    void UpdateUIOverlayCheck()
    {
        // UIの上にマウスがあるかチェック
        IsPointerOverUI = UnityEngine.EventSystems.EventSystem.current?.IsPointerOverGameObject() ?? false;
    }

    public Vector3 GetWorldMousePosition()
    {
        if (Camera.main != null)
        {
            return Camera.main.ScreenToWorldPoint(new Vector3(MousePosition.x, MousePosition.y, Camera.main.nearClipPlane));
        }
        return Vector3.zero;
    }

    public bool IsActionPressed(string actionName)
    {
        var action = inputActions.FindAction(actionName);
        return action?.IsPressed() ?? false;
    }

    public bool WasActionPressedThisFrame(string actionName)
    {
        var action = inputActions.FindAction(actionName);
        return action?.WasPressedThisFrame() ?? false;
    }

    // =============================================================================
    // Settings
    // =============================================================================
    public void SetMouseSensitivity(float sensitivity)
    {
        mouseSensitivity = Mathf.Clamp(sensitivity, 0.1f, 5f);
    }

    public void SetScrollSensitivity(float sensitivity)
    {
        scrollSensitivity = Mathf.Clamp(sensitivity, 0.1f, 5f);
    }

    public void SetEnableDebugInputs(bool enable)
    {
        enableDebugInputs = enable;
        if (enable)
            EnableDebugInput();
        else
            DisableDebugInput();
    }

    // =============================================================================
    // Cleanup
    // =============================================================================
    void OnEnable()
    {
        inputActions?.Enable();
    }

    void OnDisable()
    {
        inputActions?.Disable();
    }

    void OnDestroy()
    {
        // Unsubscribe from all events
        if (selectCardAction != null)
        {
            selectCardAction.performed -= OnSelectCardPerformed;
            cancelSelectionAction.performed -= OnCancelSelectionPerformed;
            confirmActionAction.performed -= OnConfirmActionPerformed;
            navigateAction.performed -= OnNavigatePerformed;
            navigateAction.canceled -= OnNavigateCanceled;
            mousePositionAction.performed -= OnMousePositionPerformed;
            scrollAction.performed -= OnScrollPerformed;
        }

        if (toggleDebugUIAction != null)
        {
            toggleDebugUIAction.performed -= OnToggleDebugUIPerformed;
            //forcePlayerWinAction.performed -= OnForcePlayerWinPerformed;
            //forceEnemyWinAction.performed -= OnForceEnemyWinPerformed;
            //forceDrawAction.performed -= OnForceDrawPerformed;
            //skipTurnAction.performed -= OnSkipTurnPerformed;
            //restartBattleAction.performed -= OnRestartBattlePerformed;
        }

        if (submitAction != null)
        {
            submitAction.performed -= OnSubmitPerformed;
            cancelAction.performed -= OnCancelPerformed;
            uiNavigateAction.performed -= OnUINavigatePerformed;
            pauseAction.performed -= OnPausePerformed;
        }
    }
}