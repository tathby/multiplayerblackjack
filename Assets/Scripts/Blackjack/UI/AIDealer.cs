using System.Collections;
using System.IO;
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

    private string apiKey;
    private Coroutine activeRequest;

    private void Awake()
    {
        if (File.Exists(apiKeyFilePath))
        {
            apiKey = File.ReadAllText(apiKeyFilePath).Trim();
        }
    }

    private void OnEnable()
    {
        if (actionRequested != null)
        {
            actionRequested.OnEventRaised += OnPlayerActionRequested;
        }
    }

    private void OnDisable()
    {
        if (actionRequested != null)
        {
            actionRequested.OnEventRaised -= OnPlayerActionRequested;
        }
    }

    public void GenerateDealerLine(string playerAction, int playerTotal)
    {
        if (activeRequest != null)
        {
            StopCoroutine(activeRequest);
        }

        SetDealerText("Dealer is thinking...");
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            SetDealerText(HuggingFaceDealerLineClient.GetFallbackDealerLine(playerAction, playerTotal));
            return;
        }

        activeRequest = StartCoroutine(SendPrompt(playerAction, playerTotal));
    }

    private void OnPlayerActionRequested(PlayerActionMessage message)
    {
        if (!IsLocalPlayerAction(message))
        {
            return;
        }

        GenerateDealerLine(GetActionPromptText(message.Action), GetPlayerTotal(message.PlayerId));
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
        using (UnityWebRequest request = HuggingFaceDealerLineClient.CreateRequest(apiKey, action, total))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                SetDealerText(HuggingFaceDealerLineClient.ParseDealerLine(request.downloadHandler.text));
            }
            else
            {
                SetDealerText(HuggingFaceDealerLineClient.GetFallbackDealerLine(action, total));
                Debug.LogWarning($"AI Dealer unavailable: {request.error}");
            }
        }

        activeRequest = null;
    }

    private void SetDealerText(string text)
    {
        if (dealerText != null)
        {
            dealerText.text = text;
        }
    }
}
