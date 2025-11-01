using UnityEngine;

public class FacialScanColliderManager2000 : MonoBehaviour
{

    private OpenAiResponseToTtsWEmotionDemo demoInstance;
    private EmotionOscListener oscClientInstance;

    [Header("Trigger Settings")]
    [SerializeField] private string requiredTag = "FacialScanner";
    [SerializeField] private string requiredEmotion = "happy";
    [SerializeField] private float cooldownSeconds = 2.0f;
    [TextArea]
    [SerializeField] private string promptOnScan = "The user is looking into a facial scanner while doing their daily emotional scan";

    [SerializeField] private bool triggerOnEnterFacialScan = true;
    [SerializeField] private bool triggerOnStayFacialScan = true;
    [Tooltip("Optional: reference to the scanner object in the scene (not required for functionality)")]
    [SerializeField] private GameObject scannerObj;
    [SerializeField] private Material greenMaterial;

    [SerializeField] private GameObject DoorObj;


    private bool cooldown;

    void Awake()
    {
#if UNITY_2023_1_OR_NEWER
        if (demoInstance == null) demoInstance = FindFirstObjectByType<OpenAiResponseToTtsWEmotionDemo>();
        if (oscClientInstance == null) oscClientInstance = FindFirstObjectByType<EmotionOscListener>();
#else
        if (demoInstance == null) demoInstance = FindObjectOfType<OpenAiResponseToTtsWEmotionDemo>();
        if (oscClientInstance == null) oscClientInstance = FindObjectOfType<EmotionOscListener>();
#endif
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySpeechSFX("oai_facial_scan_sp");
        else
            Debug.LogWarning("FacialScanColliderManager2000: SoundManager.Instance not found in Awake().");
    }

    void OnTriggerEnter(Collider other)
    {
        if (!triggerOnEnterFacialScan) return;
        TryTrigger(other);
    }

    void OnTriggerStay(Collider other)
    {
        if (!triggerOnStayFacialScan) return;
        TryTrigger(other);
    }

    private void TryTrigger(Collider other)
    {
        if (cooldown) return;

        if (demoInstance == null)
        {
            Debug.LogWarning("FacialScanColliderManager2000: No OpenAiResponseToTtsWEmotionDemo instance found in scene.");
            return;
        }
        if (oscClientInstance == null)
        {
            Debug.LogWarning("FacialScanColliderManager2000: No EmotionOscListener instance found in scene.");
            return;
        }
        if (!other.CompareTag(requiredTag)) return;

        var dominant = oscClientInstance.DominantEmotion ?? "unknown";
        // case-insensitive compare for robustness
        if (string.Equals(dominant, requiredEmotion, System.StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log($"Facial scan matched (tag={requiredTag}, emotion={dominant}). Triggering response.");
            demoInstance.facialEmotionOverride = dominant;
            demoInstance.prompt = promptOnScan;
            _ = demoInstance.RunOnce();

            // deactivate facial scan triggers after successful scan
            triggerOnEnterFacialScan = false;
            triggerOnStayFacialScan = false;
            if (scannerObj != null)
            {
                var renderer = scannerObj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.sharedMaterial = greenMaterial;
                }
            }
            DoorObj?.SetActive(false);

            // cooldown only when we actually fired
            cooldown = true;
            Invoke(nameof(ResetCooldown), cooldownSeconds);
        }
    }

    void ResetCooldown() => cooldown = false;
}
