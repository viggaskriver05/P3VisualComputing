using System.Threading.Tasks;
using UnityEngine;

public class OpenAiResponseToTtsWEmotionDemo : MonoBehaviour
{
    [Header("Response Prompting")]
    [TextArea]
    public string prompt = "Describe the scene in two sentences.";
    [TextArea]
    public string systemPromptOverride = "You are a friendly AI in a game. Please keep conversation with the player engaging. You address the player directly and talk in a modern tone. The response should never exceed 20 words."; // Leave empty to use client's defaultSystemPrompt
    public string facialEmotionOverride; // e.g., happy, sad, angry
    public string toneOverride = "friendly"; // e.g., neutral, scary, friendly
    string dominant = "unknown";


    [Header("TTS Settings")]
    public string ttsVoice = "alloy"; // alloy, shimmer, onyx, etc.
    public string ttsModel = "gpt-4o-mini-tts"; // or gpt-audio-mini

    private OpenAIResponseClient responseClient;
    private OpenAITtsClient ttsClient;
    [SerializeField] EmotionOscListener listener;
    private bool isRunning; // guard against overlapping calls

    void Awake()
    {
        if (listener == null)
        {
            listener = UnityEngine.Object.FindFirstObjectByType<EmotionOscListener>();
        }
    }
    async void Start()
    {
#if UNITY_2023_1_OR_NEWER
        responseClient = FindFirstObjectByType<OpenAIResponseClient>();
        ttsClient = FindFirstObjectByType<OpenAITtsClient>();
#else
        responseClient = FindObjectOfType<OpenAIResponseClient>();
        ttsClient = FindObjectOfType<OpenAITtsClient>();
#endif
        if (!responseClient) responseClient = gameObject.AddComponent<OpenAIResponseClient>();
        if (!ttsClient) ttsClient = gameObject.AddComponent<OpenAITtsClient>();
    }

    void Update()
    {
        dominant = listener != null ? listener.DominantEmotion : "unknown";
        facialEmotionOverride = dominant;
    }

    [ContextMenu("Run Once")]
    public  async Task RunOnce()
    {
        if (isRunning) { Debug.Log("RunOnce already in progress."); return; }
        isRunning = true;
        try
        {
            // Ensure clients exist even if Start hasn't run yet
#if UNITY_2023_1_OR_NEWER
            responseClient ??= FindFirstObjectByType<OpenAIResponseClient>();
            ttsClient ??= FindFirstObjectByType<OpenAITtsClient>();
#else
            responseClient ??= FindObjectOfType<OpenAIResponseClient>();
            ttsClient ??= FindObjectOfType<OpenAITtsClient>();
#endif
            if (!responseClient) responseClient = gameObject.AddComponent<OpenAIResponseClient>();
            if (!ttsClient) ttsClient = gameObject.AddComponent<OpenAITtsClient>();

            Debug.Log($"Responding. (voice={ttsVoice}, model={ttsModel}, dominant={dominant})");

            // 1) Get text response
            var raw = await responseClient.GetResponseAsync(prompt, systemPromptOverride, facialEmotionOverride, toneOverride);
            if (string.IsNullOrWhiteSpace(raw))
            {
                Debug.LogWarning("Response text was empty or null.");
                return;
            }

            // If API returned a JSON blob, try to extract assistant message content
            var text = TryExtractAssistantContent(raw) ?? raw;
            Debug.Log($"Assistant Text: {text}");

            // 2) Speak the text via TTS
            await ttsClient.SpeakAsync(text, ttsVoice, ttsModel);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"RunOnce failed: {ex}");
        }
        finally
        {
            isRunning = false;
        }
    }

    // Minimal JSON extraction for OpenAI Chat response
    [System.Serializable]
    private class ChatResponse { public Choice[] choices; }
    [System.Serializable]
    private class Choice { public Message message; }
    [System.Serializable]
    private class Message { public string content; }

    private string TryExtractAssistantContent(string maybeJson)
    {
        if (!maybeJson.TrimStart().StartsWith("{")) return null;
        try
        {
            var parsed = JsonUtility.FromJson<ChatResponse>(maybeJson);
            if (parsed != null && parsed.choices != null && parsed.choices.Length > 0)
            {
                return parsed.choices[0]?.message?.content;
            }
        }
        catch
        {
            // ignore and fall back
        }
        return null;
    }
}
