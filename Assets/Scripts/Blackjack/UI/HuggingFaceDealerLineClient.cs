using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class HuggingFaceDealerLineClient
{
    public const string Endpoint = "https://api-inference.huggingface.co/models/google/flan-t5-large";

    [Serializable]
    private struct PromptRequest
    {
        public string inputs;
    }

    [Serializable]
    private class TextResponse
    {
        public string generated_text;
    }

    [Serializable]
    private class TextResponseList
    {
        public TextResponse[] items;
    }

    public static UnityWebRequest CreateRequest(string apiKey, string action, int total)
    {
        string prompt =
            "You are a sarcastic cyberpunk blackjack dealer. " +
            $"The player chose to {action} with a total of {total}. " +
            "Respond with ONE short sentence.";
        string json = JsonUtility.ToJson(new PromptRequest { inputs = prompt });
        byte[] body = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(Endpoint, UnityWebRequest.kHttpVerbPOST)
        {
            uploadHandler = new UploadHandlerRaw(body),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);
        return request;
    }

    public static string ParseDealerLine(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
        {
            return "The dealer says nothing. Somehow, it's worse.";
        }

        string trimmed = response.Trim();
        if (!trimmed.StartsWith("["))
        {
            return trimmed;
        }

        TextResponseList parsed = JsonUtility.FromJson<TextResponseList>($"{{\"items\":{trimmed}}}");
        if (parsed == null || parsed.items == null || parsed.items.Length == 0)
        {
            return trimmed;
        }

        string generatedText = parsed.items[0].generated_text;
        return string.IsNullOrWhiteSpace(generatedText) ? trimmed : generatedText.Trim();
    }

    public static string GetFallbackDealerLine(string action, int total)
    {
        switch (action)
        {
            case "hit":
                return $"Another card at {total}? Bold. Possibly terminal.";
            case "stand":
                return $"Standing on {total}? The table respects cautious cowards.";
            case "double down":
                return $"Doubling down at {total}? Finally, a pulse.";
            default:
                return "The dealer watches in neon-lit silence.";
        }
    }
}
