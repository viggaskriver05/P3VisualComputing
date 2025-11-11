using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class OpenAITtsClient : MonoBehaviour
{
    public string apiKey = "";
    public string model = "gpt-4o-mini-tts";
    public string voice = "alloy";
    public AudioSource audioSource;

    private static readonly HttpClient http = new HttpClient();

    [Serializable] private class TtsRequest { public string model; public string input; public string voice; }

    void Awake()
    {
        // Ensure we have an AudioSource component to play the audio
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// Generates speech from text using OpenAI TTS API, caches it, and plays it.
    /// </summary>
    /// <param name="text">The text to convert to speech</param>
    /// <param name="overrideVoice">Optional: Override the default voice (e.g., "alloy", "shimmer")</param>
    /// <param name="overrideModel">Optional: Override the default model (e.g., "gpt-4o-mini-tts")</param>
    /// <returns>The generated AudioClip</returns>
    public async Task<AudioClip> SpeakAsync(string text, string overrideVoice = null, string overrideModel = null)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;

        // Use overrides if provided, otherwise use inspector defaults
        var voiceToUse = string.IsNullOrEmpty(overrideVoice) ? voice : overrideVoice;
        var modelToUse = string.IsNullOrEmpty(overrideModel) ? model : overrideModel;

        // Create unique hash for this combination of model/voice/text for caching
        var hash = Hash($"{modelToUse}|{voiceToUse}|{text}");
        var path = Path.Combine(Application.persistentDataPath, "tts_cache", hash + ".mp3");

        // Download only if not already cached
        if (!File.Exists(path))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            await DownloadAsync(text, path, voiceToUse, modelToUse);
        }

        // Copy to Assets/GeneratedTTS/ for version control and sharing
        var assetPath = Path.Combine(Application.dataPath, "GeneratedTTS", hash + ".mp3");
        Directory.CreateDirectory(Path.GetDirectoryName(assetPath));
        File.Copy(path, assetPath, true);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.ImportAsset("Assets/GeneratedTTS/" + hash + ".mp3");
#endif

        // Load the audio file and play it
        var clip = await LoadAsync(path);
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        return clip;
    }

    /// <summary>
    /// Downloads audio from OpenAI TTS API and saves it to a file.
    /// </summary>
    private async Task DownloadAsync(string text, string outPath, string voiceToUse, string modelToUse)
    {
        // Get API key from inspector or environment variable
        var key = string.IsNullOrEmpty(apiKey) ? Environment.GetEnvironmentVariable("OPENAI_API_KEY") : apiKey;
        if (string.IsNullOrEmpty(key)) throw new Exception("API key missing.");

        // Set up HTTP request with authorization
        http.DefaultRequestHeaders.Clear();
        http.DefaultRequestHeaders.Add("Authorization", $"Bearer {key}");

        // Build JSON request body and send to OpenAI API
        var json = JsonUtility.ToJson(new TtsRequest { model = modelToUse, input = text, voice = voiceToUse });
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var resp = await http.PostAsync("https://api.openai.com/v1/audio/speech", content);

        if (!resp.IsSuccessStatusCode)
        {
            var err = await resp.Content.ReadAsStringAsync();
            throw new Exception($"TTS error: {resp.StatusCode}\n{err}");
        }

        // Save the audio bytes to disk
        var bytes = await resp.Content.ReadAsByteArrayAsync();
        File.WriteAllBytes(outPath, bytes);
    }

    /// <summary>
    /// Loads an MP3 file from disk as a Unity AudioClip.
    /// </summary>
    private async Task<AudioClip> LoadAsync(string filePath)
    {
        // Load audio file using Unity's UnityWebRequest
        var uri = "file://" + filePath;
        using var req = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.MPEG);
        var op = req.SendWebRequest();
        while (!op.isDone) await Task.Yield();
        return req.result == UnityWebRequest.Result.Success ? DownloadHandlerAudioClip.GetContent(req) : null;
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
}
