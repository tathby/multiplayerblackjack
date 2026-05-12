using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class AIDealer : MonoBehaviour
{
    public TextMeshProUGUI dealerText;

    private string apiKey;

    void Awake()
    {
        apiKey = System.IO.File.ReadAllText("Assets/Secrets/api_key.txt").Trim();
    }

    public void GenerateDealerLine(string playerAction, int playerTotal)
    {
        StartCoroutine(SendPrompt(playerAction, playerTotal));
    }

    IEnumerator SendPrompt(string action, int total)
    {
        string prompt =
            $"You are a sarcastic cyberpunk blackjack dealer. " +
            $"The player chose to {action} with a total of {total}. " +
            $"Respond with ONE short sentence.";

        string json =
            "{\"inputs\":\"" + prompt + "\"}";

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(
            "https://api-inference.huggingface.co/models/google/flan-t5-large",
            "POST"
        );

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;

            dealerText.text = response;
        }
        else
        {
            dealerText.text = "AI Dealer unavailable.";
            Debug.Log(request.error);
        }
    }
}