using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class HuggingFaceDealerLineClient
{
    public const string DefaultEndpoint = "https://router.huggingface.co/v1/chat/completions";
    public const string DefaultModel = "openai/gpt-oss-120b:fastest";

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
        public string reasoning;
    }

    [Serializable]
    private class ChatResponse
    {
        public ChatChoice[] choices;
    }

    [Serializable]
    private class ChatChoice
    {
        public string finish_reason;
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

    public static string ParseDealerLine(string response, string action, int total)
    {
        if (string.IsNullOrWhiteSpace(response)) return GetFallbackDealerLine(action, total);

        string trimmed = response.Trim();
        if (trimmed.StartsWith("[")) return ParseLegacyResponse(trimmed, action, total);
        if (!trimmed.StartsWith("{")) return trimmed;

        ChatResponse parsed = JsonUtility.FromJson<ChatResponse>(trimmed);
        if (parsed == null || parsed.choices == null || parsed.choices.Length == 0)
        {
            return GetFallbackDealerLine(action, total);
        }

        ChatChoice choice = parsed.choices[0];
        string content = choice.message.content;
        if (!string.IsNullOrWhiteSpace(content)) return CleanDealerLine(content);

        Debug.LogWarning($"AI Dealer response did not include final content. finish_reason={choice.finish_reason}. Using fallback line.");
        return GetFallbackDealerLine(action, total);
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
        string prompt = $"The player chose to {action} with a blackjack total of {total}. Reply with exactly one short dealer quote. Do not explain.";
        return new ChatRequest
        {
            model = string.IsNullOrWhiteSpace(model) ? DefaultModel : model,
            max_tokens = 160,
            temperature = 0.9f,
            messages = new[]
            {
                new ChatMessage { role = "system", content = "You are a sarcastic cyberpunk blackjack dealer. Give only the spoken dealer line." },
                new ChatMessage { role = "user", content = prompt }
            }
        };
    }

    private static string ParseLegacyResponse(string trimmed, string action, int total)
    {
        LegacyTextResponseList parsed = JsonUtility.FromJson<LegacyTextResponseList>($"{{\"items\":{trimmed}}}");
        if (parsed == null || parsed.items == null || parsed.items.Length == 0) return GetFallbackDealerLine(action, total);
        string generatedText = parsed.items[0].generated_text;
        return string.IsNullOrWhiteSpace(generatedText) ? GetFallbackDealerLine(action, total) : CleanDealerLine(generatedText);
    }

    private static string CleanDealerLine(string line)
    {
        string cleaned = line.Trim();
        return cleaned.Length > 160 ? cleaned.Substring(0, 160).TrimEnd() : cleaned;
    }
}
