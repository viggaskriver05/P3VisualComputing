using System.Threading.Tasks;
using UnityEngine;

public class OpenAITtsDemo : MonoBehaviour
{
    [TextArea] public string line = "Welcome to the demo. This is OpenAI TTS running inside Unity.";
    public string voice = "alloy";            // alloy, onyx, fable, echo, shimmer, coral, verse
    public string model = "gpt-4o-mini-tts";  // or "gpt-audio-mini"

    private OpenAITtsClient client;

    async void Start()
    {
#if UNITY_2023_1_OR_NEWER
    client = FindFirstObjectByType<OpenAITtsClient>();
#else
    client = FindObjectOfType<OpenAITtsClient>();
#endif
        if (!client) client = gameObject.AddComponent<OpenAITtsClient>();
        await SpeakOnce();
    }

    [ContextMenu("Speak Once")]
    public async Task SpeakOnce()
    {
        await client.SpeakAsync(line, voice, model);
    }
}
