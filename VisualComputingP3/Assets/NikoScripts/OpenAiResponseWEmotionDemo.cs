using System.Threading.Tasks;
using UnityEngine;

public class OpenAiResponseWEmotionDemo : MonoBehaviour
{
        [TextArea] public string prompt = "Edit this prompt to test OpenAI text generation.";
        [TextArea] public string systemPromptOverride = "You are a friendly AI in a game. Please keep conversation with the player engaging. You address the player directly and talk in a modern tone. The response should never exceed 20 words.";
        public string facialEmotionOverride;
        public string toneOverride = "friendly";
        string dominant = "unknown";


        private OpenAIResponseClient client;
        [SerializeField] EmotionOscListener listener;
        void Awake()
        {
                if (listener == null)
                {
                        listener = UnityEngine.Object.FindFirstObjectByType<EmotionOscListener>();
                }
                SoundManager.Instance.PlaySpeechSFX("oai_intro_sp");
        }
        async void Start()
        {
#if UNITY_2023_1_OR_NEWER
                client = FindFirstObjectByType<OpenAIResponseClient>();
#else
        client = FindObjectOfType<OpenAIResponseClient>();
#endif
                if (!client) client = gameObject.AddComponent<OpenAIResponseClient>();
        }

        void Update()
        {
                dominant = listener != null ? listener.DominantEmotion : "unknown";
                facialEmotionOverride = dominant;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                        _ = RespondOnce();
                }
        }

        [ContextMenu("Respond Once")]
        public async Task RespondOnce()
        {
                var response = await client.GetResponseAsync(prompt, systemPromptOverride, facialEmotionOverride, toneOverride);
                Debug.Log($"Response: {response}");
        }
}

