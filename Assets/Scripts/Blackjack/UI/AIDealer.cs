using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class AIDealer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dealerText;
    [SerializeField] private PlayerActionEventChannel actionRequested;
    [SerializeField] private BlackjackGameStateSO gameState;
    [SerializeField] private StringVariable localPlayerId;
    [SerializeField] private string apiKeyFilePath = "Assets/Secrets/api_key.txt";
    [SerializeField] private string endpoint = HuggingFaceDealerLineClient.DefaultEndpoint;
    [SerializeField] private string model = HuggingFaceDealerLineClient.DefaultModel;
    [SerializeField] private bool logDiagnostics = true;

    private string apiKey;
    private string apiKeySource;
    private Coroutine activeRequest;

    private void Awake()
    {
        ReloadApiKey();
    }

    private void OnEnable()
    {
        if (actionRequested != null) actionRequested.OnEventRaised += OnPlayerActionRequested;
    }

    private void OnDisable()
    {
        if (actionRequested != null) actionRequested.OnEventRaised -= OnPlayerActionRequested;
    }

    public void GenerateDealerLine(string playerAction, int playerTotal)
    {
        if (activeRequest != null)
        {
            StopCoroutine(activeRequest);
        }

        activeRequest = StartCoroutine(GenerateDealerLineRoutine(playerAction, playerTotal));
    }

    [ContextMenu("Test Dealer API Call")]
    public void TestDealerApiCall()
    {
        GenerateDealerLine("hit", 16);
    }

    [ContextMenu("Reload Dealer API Key")]
    public void ReloadApiKey()
    {
        bool loaded = DealerApiKeyLoader.TryLoad(apiKeyFilePath, out apiKey, out apiKeySource);
        if (logDiagnostics)
        {
            string message = loaded ? $"AI Dealer API key loaded from {apiKeySource}." : $"AI Dealer API key missing. Checked {apiKeySource}.";
            Debug.Log(message, this);
        }
    }

    private IEnumerator GenerateDealerLineRoutine(string action, int total)
    {
        SetDealerText("Dealer is thinking...");
        yield return null;

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            LogDiagnostic("AI Dealer is using fallback text because no API key is loaded; no web request was sent.");
            SetDealerText(HuggingFaceDealerLineClient.GetFallbackDealerLine(action, total));
            activeRequest = null;
            yield break;
        }

        yield return SendPrompt(action, total);
        activeRequest = null;
    }

    private void OnPlayerActionRequested(PlayerActionMessage message)
    {
        if (!IsLocalPlayerAction(message))
        {
            return;
        }

        string action = GetActionPromptText(message.Action);
        int total = GetPlayerTotal(message.PlayerId);
        LogDiagnostic($"AI Dealer received local action '{action}' with total {total}.");
        GenerateDealerLine(action, total);
    }

    private bool IsLocalPlayerAction(PlayerActionMessage message)
    {
        return localPlayerId == null || string.IsNullOrEmpty(localPlayerId.Value) || message.PlayerId == localPlayerId.Value;
    }

    private int GetPlayerTotal(string playerId)
    {
        PlayerSeatState player = gameState != null ? gameState.Data.GetPlayer(playerId) : null;
        return player != null ? player.Hand.GetBestValue() : 0;
    }

    private string GetActionPromptText(PlayerAction action)
    {
        switch (action)
        {
            case PlayerAction.Hit:
                return "hit";
            case PlayerAction.Stand:
                return "stand";
            case PlayerAction.DoubleDown:
                return "double down";
            default:
                return action.ToString().ToLowerInvariant();
        }
    }

    private IEnumerator SendPrompt(string action, int total)
    {
        using (UnityWebRequest request = HuggingFaceDealerLineClient.CreateRequest(endpoint, apiKey, model, action, total))
        {
            LogDiagnostic($"AI Dealer sending request to {endpoint} using model {model}.");
            yield return request.SendWebRequest();
            LogDiagnostic($"AI Dealer request completed: result={request.result}, status={request.responseCode}.");

            if (request.result == UnityWebRequest.Result.Success)
            {
                SetDealerText(HuggingFaceDealerLineClient.ParseDealerLine(request.downloadHandler.text));
            }
            else
            {
                SetDealerText(HuggingFaceDealerLineClient.GetFallbackDealerLine(action, total));
                Debug.LogWarning($"AI Dealer unavailable: {request.error}. Body: {request.downloadHandler.text}", this);
            }
        }
    }

    private void SetDealerText(string text)
    {
        if (dealerText != null) dealerText.text = text;
    }

    private void LogDiagnostic(string message)
    {
        if (logDiagnostics) Debug.Log(message, this);
    }
}
