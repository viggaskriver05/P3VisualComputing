using System.Threading.Tasks;
using UnityEngine;

public class OpenAiResponseDemo : MonoBehaviour
{
    [TextArea] public string prompt = "Edit this prompt to test OpenAI text generation.";

    [TextArea] public string systemPromptOverride = "You are a friendly AI in a game. Please keep conversation with the player engaging. You address the player directly and talk in a modern tone. The response should never exceed 20 words.";
    public string facialEmotionOverride = "happy";

    public string toneOverride = "friendly";

    private OpenAIResponseClient client;

    async void Start()
    {
#if UNITY_2023_1_OR_NEWER
        client = FindFirstObjectByType<OpenAIResponseClient>();
#else
        client = FindObjectOfType<OpenAIResponseClient>();
#endif
        if (!client) client = gameObject.AddComponent<OpenAIResponseClient>();
        await RespondOnce();
    }

    [ContextMenu("Respond Once")]
    public async Task RespondOnce()
    {
    var response = await client.GetResponseAsync(prompt, systemPromptOverride, facialEmotionOverride, toneOverride);
    Debug.Log($"Response: {response}");
    }
}
