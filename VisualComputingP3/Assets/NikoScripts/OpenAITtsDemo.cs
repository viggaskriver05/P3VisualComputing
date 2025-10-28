using System.Threading.Tasks;
using UnityEngine;

public class OpenAITtsDemo : MonoBehaviour
{
    [TextArea] public string line = "Welcome. You’ve arrived at the EMPATH Rehabilitation Center, year 2054. You’re here because you asked for help to feel steady again. Some details may feel unclear at first. That’s completely normal. The process works best without too many of your old memories. Just breathe. Let us guide you.";
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
