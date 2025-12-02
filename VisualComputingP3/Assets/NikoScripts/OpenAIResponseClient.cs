using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class OpenAIResponseClient : MonoBehaviour
{
    public string apiKey = "";
    public string model = "gpt-4o-mini";

    [SerializeField] private bool useCachedPrompts = true;

    [TextArea]
    public string defaultSystemPrompt = "";
    
    [Header("Style")]
    public bool useTonality = true;
    [Tooltip("High-level response style. Examples: neutral, scary, friendly, urgent, calm.")]
    public string tonality = "neutral";

    private static readonly HttpClient http = new HttpClient();

    [Serializable] private class ChatRequest { public string model; public string messages; }

    /// <summary>
    /// Generates a text response from a prompt using OpenAI's GPT-4o-mini model, with basic caching.
    /// </summary>
    /// <param name="prompt">The prompt to send to the model</param>
    /// <param name="systemPromptOverride">Optional: system prompt to set assistant behavior; falls back to defaultSystemPrompt</param>
    /// <param name="facialEmotionOverride">Optional: user's detected facial emotion to inject into system prompt</param>
    /// <param name="toneOverride">Optional: override for tonality (e.g., "neutral", "scary"); if null uses inspector setting</param>
    /// <returns>The generated response string</returns>
    public async Task<string> GetResponseAsync(string prompt, string systemPromptOverride = null, string facialEmotionOverride = null, string toneOverride = null)
    {
        if (string.IsNullOrWhiteSpace(prompt)) return null;

        // Resolve system prompt: override or default
        if (string.IsNullOrWhiteSpace(systemPromptOverride)) systemPromptOverride = defaultSystemPrompt;

        // Inject facial emotion context if provided
        if (!string.IsNullOrWhiteSpace(facialEmotionOverride))
        {
            var emotionInstruction = $"The user's facial emotion is: {facialEmotionOverride}. Consider this in your response.";
            systemPromptOverride = string.IsNullOrWhiteSpace(systemPromptOverride)
                ? emotionInstruction
                : systemPromptOverride + "\n" + emotionInstruction;
        }
        
        // Resolve effective tone (param takes precedence over inspector)
        string effectiveTone = null;
        if (!string.IsNullOrWhiteSpace(toneOverride)) effectiveTone = toneOverride;
        else if (useTonality && !string.IsNullOrWhiteSpace(tonality)) effectiveTone = tonality;

        // Optionally append tonality instruction
        if (!string.IsNullOrWhiteSpace(effectiveTone))
        {
            var toneInstruction = $"Respond with a {effectiveTone} tone.";
            if (string.IsNullOrWhiteSpace(systemPromptOverride)) systemPromptOverride = toneInstruction; else systemPromptOverride += "\n" + toneInstruction;
        }

        // Create unique hash for this prompt/model for caching
        var hash = "";
        if (useCachedPrompts){ 
            hash = Hash($"{model}|{systemPromptOverride}|{prompt}");
        } else {
            hash = Hash($"{model}|{systemPromptOverride}|{prompt}|{Mathf.FloorToInt(UnityEngine.Random.value *  1000000)}");
        }

        // Cache file lives under Assets/GeneratedText so it is versioned with the project
        var assetDir = Path.Combine(Application.dataPath, "GeneratedText");
        Directory.CreateDirectory(assetDir);
        var path = Path.Combine(assetDir, hash + ".txt");

        // Return cached response if available (check only in Assets/GeneratedText)
        if (File.Exists(path))
        {
            var cached = File.ReadAllText(path);
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.ImportAsset("Assets/GeneratedText/" + hash + ".txt");
#endif
            return cached;
        }

        // Download response from OpenAI and save directly into Assets/GeneratedText
        var response = await DownloadAsync(prompt, model, systemPromptOverride);
        File.WriteAllText(path, response);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.ImportAsset("Assets/GeneratedText/" + hash + ".txt");
#endif
        return response;
    }

    /// <summary>
    /// Calls OpenAI API to get a response for the given prompt.
    /// </summary>
    private async Task<string> DownloadAsync(string prompt, string modelToUse, string systemPrompt)
    {
        var key = string.IsNullOrEmpty(apiKey) ? Environment.GetEnvironmentVariable("OPENAI_API_KEY") : apiKey;
        if (string.IsNullOrEmpty(key)) throw new Exception("API key missing.");

        http.DefaultRequestHeaders.Clear();
        http.DefaultRequestHeaders.Add("Authorization", $"Bearer {key}");

        // Build messages array with system prompt first, then user prompt
        var sys = string.IsNullOrWhiteSpace(systemPrompt) ? "" : $"{{\"role\":\"system\",\"content\":\"{EscapeJson(systemPrompt)}\"}},";
        var json = $"{{\"model\":\"{modelToUse}\",\"messages\":[" + sys + $"{{\"role\":\"user\",\"content\":\"{EscapeJson(prompt)}\"}}]}}";
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var resp = await http.PostAsync("https://api.openai.com/v1/chat/completions", content);

        if (!resp.IsSuccessStatusCode)
        {
            var err = await resp.Content.ReadAsStringAsync();
            throw new Exception($"Chat error: {resp.StatusCode}\n{err}");
        }

        var result = await resp.Content.ReadAsStringAsync();
        return result;
    }

    /// <summary>
    /// Creates a SHA256 hash string from input text for cache filename generation.
    /// </summary>
    public static string Hash(string s)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(s));
        var sb = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes) sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    // Escapes special characters for JSON string values
    private static string EscapeJson(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;
        return s.Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t")
                .Replace("\b", "\\b")
                .Replace("\f", "\\f");
    }
}
