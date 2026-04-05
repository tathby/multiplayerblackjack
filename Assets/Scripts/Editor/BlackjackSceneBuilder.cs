#if UNITY_EDITOR
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class BlackjackSceneBuilder
{
    private const string PrefabFolder = "Assets/Prefabs";
    private const string SystemsPrefabPath = "Assets/Prefabs/BlackjackSystems.prefab";
    private const string UiPrefabPath = "Assets/Prefabs/BlackjackUI.prefab";

    [MenuItem("Tools/Blackjack/Create Starter Scene And Prefabs")]
    public static void CreateStarterSceneAndPrefabs()
    {
        EnsureFolder(PrefabFolder);

        GameObject systemsPrefab = CreateSystemsPrefab();
        GameObject uiPrefab = CreateUiPrefab();

        Scene scene = OpenOrCreateGameScene();
        PlaceSystemsPrefab(scene, systemsPrefab);
        PlaceUiPrefab(scene, uiPrefab);
        AutoWireGameEventListeners();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("Blackjack starter scene and prefabs created.");
    }

    [MenuItem("Tools/Blackjack/Auto Wire GameEventListeners")]
    public static void AutoWireGameEventListeners()
    {
        GameObject systemsRoot = GameObject.Find("BlackjackSystems");
        if (systemsRoot == null)
        {
            Debug.LogWarning("BlackjackSystems not found in scene. Open Game.unity or run Create Starter Scene And Prefabs.");
            return;
        }

        Transform wiringRoot = systemsRoot.transform.Find("GameEventWiring");
        if (wiringRoot == null)
        {
            GameObject wiringObj = new GameObject("GameEventWiring");
            wiringObj.transform.SetParent(systemsRoot.transform);
            wiringRoot = wiringObj.transform;
        }

        EnsureListenerCount(wiringRoot, 5);

        DealController deal = systemsRoot.GetComponent<DealController>();
        DealerTurnController dealer = systemsRoot.GetComponent<DealerTurnController>();
        PayoutController payout = systemsRoot.GetComponent<PayoutController>();
        HostStateSyncEmitter sync = systemsRoot.GetComponent<HostStateSyncEmitter>();
        RoundResetController reset = systemsRoot.GetComponent<RoundResetController>();

        GameEvent allBetsPlaced = FindGameEvent("AllBetsPlaced");
        GameEvent allPlayersActed = FindGameEvent("AllPlayersActed");
        GameEvent dealerTurnEnded = FindGameEvent("DealerTurnEnded");
        GameEvent gameStateChanged = FindGameEvent("GameStateChanged");
        GameEvent roundResetRequested = FindGameEvent("RoundResetRequested");

        GameEventListener[] listeners = wiringRoot.GetComponents<GameEventListener>();
        WireListener(listeners[0], allBetsPlaced, deal, nameof(DealController.BeginDeal));
        WireListener(listeners[1], allPlayersActed, dealer, nameof(DealerTurnController.PlayDealerTurn));
        WireListener(listeners[2], dealerTurnEnded, payout, nameof(PayoutController.ResolveRound));
        WireListener(listeners[3], gameStateChanged, sync, nameof(HostStateSyncEmitter.EmitSync));
        WireListener(listeners[4], roundResetRequested, reset, nameof(RoundResetController.StartNewRound));

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        Debug.Log("GameEventListeners auto-wired.");
    }

    private static void EnsureFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }
    }

    private static GameObject CreateSystemsPrefab()
    {
        GameObject root = new GameObject("BlackjackSystems");
        root.AddComponent<LocalPlayerBootstrapper>();
        root.AddComponent<LocalInputRelay>();
        root.AddComponent<BettingController>();
        root.AddComponent<DealController>();
        root.AddComponent<PlayerTurnController>();
        root.AddComponent<DealerTurnController>();
        root.AddComponent<PayoutController>();
        root.AddComponent<RoundResetController>();
        root.AddComponent<HostStateSyncEmitter>();
        root.AddComponent<StateSyncApplyController>();
        root.AddComponent<BlackjackPhotonEventSender>();
        root.AddComponent<BlackjackPhotonEventReceiver>();
        root.AddComponent<HostJoinSyncController>();

        GameObject wiring = new GameObject("GameEventWiring");
        wiring.transform.SetParent(root.transform);
        wiring.AddComponent<GameEventListener>();
        wiring.AddComponent<GameEventListener>();
        wiring.AddComponent<GameEventListener>();
        wiring.AddComponent<GameEventListener>();
        wiring.AddComponent<GameEventListener>();

        GameObject prefab = SaveAsPrefab(root, SystemsPrefabPath);
        Object.DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateUiPrefab()
    {
        GameObject root = new GameObject("BlackjackUI", typeof(RectTransform));
        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        GameObject background = CreatePanel(root.transform, "Background", new Color(0.06f, 0.2f, 0.12f, 0.9f));
        Stretch(background.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        GameObject phaseBanner = CreateTMP("PhaseBanner", root.transform, "Phase: Betting", 24, TextAlignmentOptions.TopLeft);
        SetRect(phaseBanner.GetComponent<RectTransform>(), new Vector2(0, 1), new Vector2(0, 1), new Vector2(20, -20), new Vector2(400, 40));
        phaseBanner.AddComponent<PhaseBannerView>();

        GameObject dealerGroup = new GameObject("DealerHand", typeof(RectTransform));
        dealerGroup.transform.SetParent(root.transform);
        SetRect(dealerGroup.GetComponent<RectTransform>(), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -80), new Vector2(600, 100));
        GameObject dealerCards = CreateTMP("DealerCards", dealerGroup.transform, "Dealer Cards", 22, TextAlignmentOptions.Center);
        GameObject dealerValue = CreateTMP("DealerValue", dealerGroup.transform, "Value: --", 18, TextAlignmentOptions.Center);
        SetRect(dealerCards.GetComponent<RectTransform>(), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -10), new Vector2(600, 30));
        SetRect(dealerValue.GetComponent<RectTransform>(), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -45), new Vector2(600, 24));
        HandTextView dealerView = dealerGroup.AddComponent<HandTextView>();
        AssignHandView(dealerView, dealerCards, dealerValue, true);

        GameObject playerGroup = new GameObject("PlayerHand", typeof(RectTransform));
        playerGroup.transform.SetParent(root.transform);
        SetRect(playerGroup.GetComponent<RectTransform>(), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 140), new Vector2(600, 100));
        GameObject playerCards = CreateTMP("PlayerCards", playerGroup.transform, "Your Cards", 22, TextAlignmentOptions.Center);
        GameObject playerValue = CreateTMP("PlayerValue", playerGroup.transform, "Value: --", 18, TextAlignmentOptions.Center);
        SetRect(playerCards.GetComponent<RectTransform>(), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -10), new Vector2(600, 30));
        SetRect(playerValue.GetComponent<RectTransform>(), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -45), new Vector2(600, 24));
        HandTextView playerView = playerGroup.AddComponent<HandTextView>();
        AssignHandView(playerView, playerCards, playerValue, false);

        GameObject chipsGroup = new GameObject("ChipsPanel", typeof(RectTransform));
        chipsGroup.transform.SetParent(root.transform);
        SetRect(chipsGroup.GetComponent<RectTransform>(), new Vector2(0, 0), new Vector2(0, 0), new Vector2(20, 140), new Vector2(220, 80));
        GameObject chipsText = CreateTMP("ChipsText", chipsGroup.transform, "Chips: --", 18, TextAlignmentOptions.Left);
        GameObject betText = CreateTMP("BetText", chipsGroup.transform, "Bet: --", 18, TextAlignmentOptions.Left);
        SetRect(chipsText.GetComponent<RectTransform>(), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, -10), new Vector2(220, 24));
        SetRect(betText.GetComponent<RectTransform>(), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, -40), new Vector2(220, 24));
        PlayerChipsView chipsView = chipsGroup.AddComponent<PlayerChipsView>();
        AssignChipsView(chipsView, chipsText, betText);

        GameObject betGroup = new GameObject("BetControls", typeof(RectTransform));
        betGroup.transform.SetParent(root.transform);
        SetRect(betGroup.GetComponent<RectTransform>(), new Vector2(1, 0), new Vector2(1, 0), new Vector2(-260, 140), new Vector2(240, 120));
        GameObject betLabel = CreateTMP("BetLabel", betGroup.transform, "Bet: 100", 18, TextAlignmentOptions.Center);
        SetRect(betLabel.GetComponent<RectTransform>(), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -5), new Vector2(200, 24));
        GameObject sliderObj = DefaultControls.CreateSlider(new DefaultControls.Resources());
        sliderObj.name = "BetSlider";
        sliderObj.transform.SetParent(betGroup.transform, false);
        SetRect(sliderObj.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -10), new Vector2(200, 20));
        GameObject betButtonObj = DefaultControls.CreateButton(new DefaultControls.Resources());
        betButtonObj.name = "BetButton";
        betButtonObj.transform.SetParent(betGroup.transform, false);
        SetRect(betButtonObj.GetComponent<RectTransform>(), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 10), new Vector2(180, 30));
        SetButtonText(betButtonObj, "Place Bet");

        BetControlsUI betControls = betGroup.AddComponent<BetControlsUI>();
        AssignBetControls(betControls, sliderObj.GetComponent<Slider>(), betButtonObj.GetComponent<Button>(), betLabel.GetComponent<TextMeshProUGUI>());

        GameObject actionGroup = new GameObject("ActionButtons", typeof(RectTransform));
        actionGroup.transform.SetParent(root.transform);
        SetRect(actionGroup.GetComponent<RectTransform>(), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 40), new Vector2(500, 60));
        GameObject hitButtonObj = DefaultControls.CreateButton(new DefaultControls.Resources());
        hitButtonObj.name = "HitButton";
        hitButtonObj.transform.SetParent(actionGroup.transform, false);
        SetRect(hitButtonObj.GetComponent<RectTransform>(), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0), new Vector2(140, 36));
        SetButtonText(hitButtonObj, "Hit");
        GameObject standButtonObj = DefaultControls.CreateButton(new DefaultControls.Resources());
        standButtonObj.name = "StandButton";
        standButtonObj.transform.SetParent(actionGroup.transform, false);
        SetRect(standButtonObj.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0), new Vector2(140, 36));
        SetButtonText(standButtonObj, "Stand");
        GameObject doubleButtonObj = DefaultControls.CreateButton(new DefaultControls.Resources());
        doubleButtonObj.name = "DoubleButton";
        doubleButtonObj.transform.SetParent(actionGroup.transform, false);
        SetRect(doubleButtonObj.GetComponent<RectTransform>(), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(0, 0), new Vector2(140, 36));
        SetButtonText(doubleButtonObj, "Double");

        ActionButtonsUI actionButtons = actionGroup.AddComponent<ActionButtonsUI>();
        AssignActionButtons(actionButtons, hitButtonObj.GetComponent<Button>(), standButtonObj.GetComponent<Button>(), doubleButtonObj.GetComponent<Button>());

        GameObject resultBanner = CreateTMP("ResultBanner", root.transform, "", 26, TextAlignmentOptions.Center);
        SetRect(resultBanner.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 40), new Vector2(400, 30));
        resultBanner.AddComponent<RoundResultBannerView>();

        GameObject debugPanel = CreatePanel(root.transform, "DebugPanel", new Color(0, 0, 0, 0.4f));
        SetRect(debugPanel.GetComponent<RectTransform>(), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-260, -20), new Vector2(240, 220));
        DebugPanelUI debugPanelUi = debugPanel.AddComponent<DebugPanelUI>();
        CreateDebugButtons(debugPanel.transform, debugPanelUi);

        GameObject prefab = SaveAsPrefab(root, UiPrefabPath);
        Object.DestroyImmediate(root);
        return prefab;
    }

    private static Scene OpenOrCreateGameScene()
    {
        string scenePath = "Assets/Scenes/Game.unity";
        if (File.Exists(scenePath))
        {
            return EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }

        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        EditorSceneManager.SaveScene(newScene, scenePath);
        return newScene;
    }

    private static void PlaceSystemsPrefab(Scene scene, GameObject prefab)
    {
        if (GameObject.Find("BlackjackSystems") != null)
        {
            return;
        }

        PrefabUtility.InstantiatePrefab(prefab, scene);
    }

    private static void PlaceUiPrefab(Scene scene, GameObject prefab)
    {
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        if (GameObject.Find("BlackjackUI") != null)
        {
            return;
        }

        GameObject uiInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, canvas.transform);
        uiInstance.transform.SetParent(canvas.transform, false);
    }

    private static GameObject CreatePanel(Transform parent, string name, Color color)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(parent, false);
        Image image = panel.GetComponent<Image>();
        image.color = color;
        return panel;
    }

    private static GameObject CreateTMP(string name, Transform parent, string text, int size, TextAlignmentOptions align)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.alignment = align;
        if (TMP_Settings.defaultFontAsset != null)
        {
            tmp.font = TMP_Settings.defaultFontAsset;
        }
        tmp.color = Color.white;
        return obj;
    }

    private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 size)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = size;
    }

    private static void Stretch(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }

    private static void SetButtonText(GameObject buttonObj, string text)
    {
        Text label = buttonObj.GetComponentInChildren<Text>();
        if (label != null)
        {
            label.text = text;
        }
    }

    private static void AssignHandView(HandTextView view, GameObject cards, GameObject value, bool isDealer)
    {
        SerializedObject so = new SerializedObject(view);
        so.FindProperty("isDealer").boolValue = isDealer;
        so.FindProperty("cardsText").objectReferenceValue = cards.GetComponent<TextMeshProUGUI>();
        so.FindProperty("valueText").objectReferenceValue = value.GetComponent<TextMeshProUGUI>();
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void AssignChipsView(PlayerChipsView view, GameObject chips, GameObject bet)
    {
        SerializedObject so = new SerializedObject(view);
        so.FindProperty("chipsText").objectReferenceValue = chips.GetComponent<TextMeshProUGUI>();
        so.FindProperty("betText").objectReferenceValue = bet.GetComponent<TextMeshProUGUI>();
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void AssignBetControls(BetControlsUI view, Slider slider, Button button, TextMeshProUGUI label)
    {
        SerializedObject so = new SerializedObject(view);
        so.FindProperty("betSlider").objectReferenceValue = slider;
        so.FindProperty("betButton").objectReferenceValue = button;
        so.FindProperty("betAmountText").objectReferenceValue = label;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void AssignActionButtons(ActionButtonsUI view, Button hit, Button stand, Button dbl)
    {
        SerializedObject so = new SerializedObject(view);
        so.FindProperty("hitButton").objectReferenceValue = hit;
        so.FindProperty("standButton").objectReferenceValue = stand;
        so.FindProperty("doubleButton").objectReferenceValue = dbl;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateDebugButtons(Transform parent, DebugPanelUI debugPanel)
    {
        string[] labels = { "Auto Bet 100", "Force Deal", "Force Dealer", "Force Payout", "Reset Round", "Give Chips" };
        UnityEngine.Events.UnityAction[] actions = { debugPanel.AutoBet, debugPanel.ForceDeal, debugPanel.ForceDealerTurn, debugPanel.ForcePayout, debugPanel.ResetRound, debugPanel.GiveChips };

        for (int i = 0; i < labels.Length; i++)
        {
            GameObject buttonObj = DefaultControls.CreateButton(new DefaultControls.Resources());
            buttonObj.name = labels[i];
            buttonObj.transform.SetParent(parent, false);
            RectTransform rect = buttonObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1);
            rect.anchorMax = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(200, 24);
            rect.anchoredPosition = new Vector2(0, -20 - i * 30);
            SetButtonText(buttonObj, labels[i]);

            Button button = buttonObj.GetComponent<Button>();
            UnityEventTools.AddPersistentListener(button.onClick, actions[i]);
        }
    }

    private static GameObject SaveAsPrefab(GameObject root, string path)
    {
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        AssetDatabase.SaveAssets();
        return prefab;
    }

    private static void EnsureListenerCount(Transform wiringRoot, int count)
    {
        GameEventListener[] existing = wiringRoot.GetComponents<GameEventListener>();
        for (int i = existing.Length; i < count; i++)
        {
            wiringRoot.gameObject.AddComponent<GameEventListener>();
        }
    }

    private static GameEvent FindGameEvent(string assetName)
    {
        string[] guids = AssetDatabase.FindAssets($"{assetName} t:GameEvent");
        if (guids.Length == 0)
        {
            Debug.LogWarning($"GameEvent asset not found: {assetName}");
            return null;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<GameEvent>(path);
    }

    private static void WireListener(GameEventListener listener, GameEvent gameEvent, Object target, string methodName)
    {
        if (listener == null)
        {
            return;
        }

        var eventField = typeof(GameEventListener).GetField("gameEvent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var responseField = typeof(GameEventListener).GetField("response", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (eventField != null)
        {
            eventField.SetValue(listener, gameEvent);
        }

        UnityEngine.Events.UnityEvent response = responseField != null
            ? responseField.GetValue(listener) as UnityEngine.Events.UnityEvent
            : null;

        if (response == null)
        {
            response = new UnityEngine.Events.UnityEvent();
            responseField?.SetValue(listener, response);
        }
        else
        {
            int count = response.GetPersistentEventCount();
            for (int i = count - 1; i >= 0; i--)
            {
                UnityEventTools.RemovePersistentListener(response, i);
            }
        }

        if (target != null && !string.IsNullOrEmpty(methodName))
        {
            UnityEventTools.AddPersistentListener(response, (UnityEngine.Events.UnityAction)System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), target, methodName));
        }

        EditorUtility.SetDirty(listener);
    }
}
#endif
