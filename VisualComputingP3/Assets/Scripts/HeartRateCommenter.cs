using UnityEngine;

// Makes the AI comment out loud when the player's heart rate jumps
// from calm (e.g. 50-70 BPM) to scared (e.g. 90+ BPM).
// Put this on any GameObject in the scene (e.g. the same one as ArduinoSerial).
public class HeartRateCommenter : MonoBehaviour
{
    [Header("References (found automatically if left empty)")]
    [SerializeField] private ArduinoSerial arduino;
    [SerializeField] private OpenAiResponseToTtsWEmotionDemo ai;

    [Header("When to comment")]
    [SerializeField] private int calmMin = 50;
    [SerializeField] private int calmMax = 70;
    [Tooltip("The AI comments when the pulse reaches this or higher.")]
    [SerializeField] private int alertBpm = 90;
    [Tooltip("The jump only counts if the player was calm within this many seconds.")]
    [SerializeField] private float calmMemorySeconds = 30f;
    [Tooltip("Minimum time between two heartbeat comments.")]
    [SerializeField] private float cooldownSeconds = 30f;

    [Header("What the AI is asked")]
    [TextArea(3, 6)]
    [SerializeField] private string heartPrompt =
        "The player's heart rate just jumped from {from} to {to} BPM. " +
        "Comment on it directly, as if you can hear their heart racing.";

    [Header("Regular comments")]
    [SerializeField] private bool regularComments = true;
    [Tooltip("Seconds between regular heart rate comments.")]
    [SerializeField] private float regularInterval = 15f;
    [TextArea(3, 6)]
    [SerializeField] private string regularPrompt =
        "The player's heart rate is {bpm} BPM right now. Comment on it briefly.";

    private float nextRegularTime;
    private float lastCalmTime = -999f;
    private int lastCalmBpm;
    private bool armed;          // player has been calm since the last comment
    private bool pending;        // a comment is waiting for the AI to be free
    private int fromBpm, toBpm;
    private float nextAllowedTime;

    private void Start()
    {
        if (arduino == null) arduino = FindFirstObjectByType<ArduinoSerial>();
        if (ai == null) ai = FindFirstObjectByType<OpenAiResponseToTtsWEmotionDemo>();
        if (arduino == null) Debug.LogWarning("[HeartRate] No ArduinoSerial in the scene.");
        if (ai == null) Debug.LogWarning("[HeartRate] No OpenAiResponseToTtsWEmotionDemo in the scene.");
        nextRegularTime = Time.time + regularInterval;
    }

    private void Update()
    {
        if (arduino == null || ai == null) return;

        int bpm = arduino.CurrentBPM;
        if (bpm > 0)
        {
            // Remember the last time the player was calm
            if (bpm >= calmMin && bpm <= calmMax)
            {
                lastCalmTime = Time.time;
                lastCalmBpm = bpm;
                armed = true;
            }

            // Calm -> scared within the time window: queue a comment
            bool recentlyCalm = Time.time - lastCalmTime <= calmMemorySeconds;
            if (armed && bpm >= alertBpm && recentlyCalm && Time.time >= nextAllowedTime)
            {
                Trigger(lastCalmBpm, bpm);
            }
        }

        // Speak as soon as the AI isn't already talking
        if (pending && !ai.IsBusy)
        {
            pending = false;
            nextAllowedTime = Time.time + cooldownSeconds;
            string p = heartPrompt.Replace("{from}", fromBpm.ToString()).Replace("{to}", toBpm.ToString());
            Debug.Log($"[HeartRate] Jump {fromBpm} -> {toBpm} BPM, asking the AI to comment.");
            _ = ai.RunWithPrompt(p);
            nextRegularTime = Time.time + regularInterval;   // don't talk twice in a row
            return;
        }

        // Regular comment every few seconds (only once we have a reading)
        if (regularComments && bpm > 0 && !pending && !ai.IsBusy && Time.time >= nextRegularTime)
        {
            nextRegularTime = Time.time + regularInterval;
            Debug.Log($"[HeartRate] Regular comment at {bpm} BPM.");
            _ = ai.RunWithPrompt(regularPrompt.Replace("{bpm}", bpm.ToString()));
        }
    }

    private void Trigger(int from, int to)
    {
        fromBpm = from;
        toBpm = to;
        armed = false;   // must calm down again before the next comment
        pending = true;
    }

    // Right-click the component in the Inspector during Play to test without the sensor
    [ContextMenu("Test heart rate jump")]
    private void TestJump() => Trigger(62, 95);
}
