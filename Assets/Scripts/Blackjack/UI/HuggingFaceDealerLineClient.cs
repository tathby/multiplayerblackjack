using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class HuggingFaceDealerLineClient
{
    public const string DefaultEndpoint = "https://router.huggingface.co/v1/chat/completions";
    public const string DefaultModel = "Qwen/Qwen2.5-0.5B-Instruct";

    [Serializable]
    private struct ChatRequest
    {
        public string model;
        public ChatMessage[] messages;
        public int max_tokens;
        public float temperature;
    }

    [Serializable]
    private struct ChatMessage
    {
        public string role;
        public string content;
    }

    [Serializable]
    private class ChatResponse
    {
        public ChatChoice[] choices;
    }

    [Serializable]
    private class ChatChoice
    {
        public ChatMessage message;
    }

    [Serializable]
    private class LegacyTextResponse
    {
        public string generated_text;
    }

    [Serializable]
    private class LegacyTextResponseList
    {
        public LegacyTextResponse[] items;
    }

    public static UnityWebRequest CreateRequest(string endpoint, string apiKey, string model, string action, int total)
    {
        string json = JsonUtility.ToJson(CreateChatRequest(model, action, total));
        byte[] body = Encoding.UTF8.GetBytes(json);
        UnityWebRequest request = new UnityWebRequest(endpoint, UnityWebRequest.kHttpVerbPOST)
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
        if (string.IsNullOrWhiteSpace(response)) return "The dealer says nothing. Somehow, it's worse.";

        string trimmed = response.Trim();
        if (trimmed.StartsWith("[")) return ParseLegacyResponse(trimmed);

        ChatResponse parsed = JsonUtility.FromJson<ChatResponse>(trimmed);
        if (parsed == null || parsed.choices == null || parsed.choices.Length == 0)
        {
            return trimmed;
        }

        string content = parsed.choices[0].message.content;
        return string.IsNullOrWhiteSpace(content) ? trimmed : content.Trim();
    }

    public static string GetFallbackDealerLine(string action, int total)
    {
        switch (action)
        {
            case "hit": return $"Another card at {total}? Bold. Possibly terminal.";
            case "stand": return $"Standing on {total}? The table respects cautious cowards.";
            case "double down": return $"Doubling down at {total}? Finally, a pulse.";
            default: return "The dealer watches in neon-lit silence.";
        }
    }

    private static ChatRequest CreateChatRequest(string model, string action, int total)
    {
        string prompt = $"The player chose to {action} with a blackjack total of {total}. Respond with one short sentence.";
        return new ChatRequest
        {
            model = string.IsNullOrWhiteSpace(model) ? DefaultModel : model,
            max_tokens = 40,
            temperature = 0.9f,
            messages = new[]
            {
                new ChatMessage { role = "system", content = "You are a sarcastic cyberpunk blackjack dealer." },
                new ChatMessage { role = "user", content = prompt }
            }
        };
    }

    private static string ParseLegacyResponse(string trimmed)
    {
        LegacyTextResponseList parsed = JsonUtility.FromJson<LegacyTextResponseList>($"{{\"items\":{trimmed}}}");
        if (parsed == null || parsed.items == null || parsed.items.Length == 0) return trimmed;
        string generatedText = parsed.items[0].generated_text;
        return string.IsNullOrWhiteSpace(generatedText) ? trimmed : generatedText.Trim();
    }
}
