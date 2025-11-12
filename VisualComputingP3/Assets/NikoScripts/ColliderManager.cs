using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class ColliderManager : MonoBehaviour
{
    // Shared dependencies
    private OpenAiResponseToTtsWEmotionDemo demoInstance;
    private EmotionOscListener oscClientInstance;

    [Header("Universal Trigger Base Settings")]
    [SerializeField] private float cooldownSeconds = 2.0f;
    [SerializeField] private bool triggerOnEnter = true;
    [SerializeField] private bool triggerOnStay = true;
    private bool cooldown;

    // FacialScanner-specific (active when GameObject.tag == "FacialScanner")
    [Header("FacialScanner Settings (active when GameObject.tag == 'FacialScanner')")]
    [SerializeField] private string requiredEmotionHappy = "happy";
    [SerializeField] private bool facialScanTriggered = false;
    [TextArea]
    [SerializeField] private string promptOnEmotionalScan = "The user is looking into a facial scanner while doing their daily emotional scan";
    [SerializeField] private GameObject faceScannerObj;
    [SerializeField] private Material greenMaterial;
    [SerializeField] private GameObject doorObj;

    // ExerciseScanner-specific (sctivate when GameObject.tag == "ExerciseScanner")
    [Header("FacialScanner Settings (active when GameObject.tag == 'ExerciseScanner')")]
    

    [Header("Intro Video Gate (FacialScanner)")]
    [SerializeField] private bool useIntroVideoGate = true;
    [SerializeField] private bool introVidIsDone = false;
    [SerializeField] private bool scanPromptDone = false;
    [SerializeField] private bool introVidStarted = false;
    [SerializeField] private string introVideoObjectName = "introvid";
    private VideoPlayer introVideo;

    void Awake()
    {
#if UNITY_2023_1_OR_NEWER
        if (demoInstance == null) demoInstance = FindFirstObjectByType<OpenAiResponseToTtsWEmotionDemo>();
        if (oscClientInstance == null) oscClientInstance = FindFirstObjectByType<EmotionOscListener>();
#else
        if (demoInstance == null) demoInstance = FindObjectOfType<OpenAiResponseToTtsWEmotionDemo>();
        if (oscClientInstance == null) oscClientInstance = FindObjectOfType<EmotionOscListener>();
#endif
    }

    void Update()
    {
        // Only run the intro gating logic while attached to a FacialScanner-tagged trigger.
        if (this.CompareTag("FacialScanner"))
        {
            FacialScanner_UpdateGate();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!triggerOnEnter) return;
        Debug.Log($"[ColliderManager] OnTriggerEnter by '{other.name}' (tag='{other.tag}') on trigger '{name}' (tag='{tag}')");
        HandleTrigger(other);
    }

    void OnTriggerStay(Collider other)
    {
        if (!triggerOnStay) return;
        Debug.Log($"[ColliderManager] OnTriggerStay by '{other.name}' (tag='{other.tag}') on trigger '{name}' (tag='{tag}')");
        HandleTrigger(other);
    }

    void HandleTrigger(Collider other)
    {
        if (cooldown) return;
         if (demoInstance == null)
        {
            Debug.LogWarning("[ColliderManager] Missing OpenAiResponseToTtsWEmotionDemo in scene.");
            return;
        }
        if (oscClientInstance == null)
        {
            Debug.LogWarning("[ColliderManager] Missing EmotionOscListener in scene.");
            return;
        }
        var dominant = oscClientInstance.DominantEmotion ?? "unknown";
        Debug.Log("[ColliderManager] FacialScanner candidate. Dominant emotion = " + dominant);
        if (!string.Equals(dominant, requiredEmotionHappy, System.StringComparison.OrdinalIgnoreCase)) return;
        demoInstance.facialEmotionOverride = dominant;

        // Use this.CompareTag to branch by the trigger object's own tag
        if (this.CompareTag("FacialScanner") && !facialScanTriggered)
        {
            TryTrigger_FacialScanner(other);
        } else if (this.CompareTag("ExerciseScanner")) {
            TryTrigger_ExerciseScanner(other);
        } else
        {
            Debug.LogWarning("ColliderManager: Unhandled trigger for tag: " + this.tag);
        }
        // Extend with more tag-specific behaviors here (else if CompareTag("Door") ... )
    }

    // ===================== FacialScanner =====================
    void FacialScanner_UpdateGate()
    {
        if (introVidIsDone) return;

        if (introVideo == null && !string.IsNullOrEmpty(introVideoObjectName))
        {
            var obj = GameObject.Find(introVideoObjectName);
            if (obj != null) introVideo = obj.GetComponent<VideoPlayer>();
        }

        if (introVideo == null)
        {
            // Fallback: proceed if video missing
            introVidIsDone = true;
            PlayScanPrompt();
            useIntroVideoGate = false;
            return;
        }

        if (introVideo.isPlaying)
        {
            if (!introVidStarted) introVidStarted = true;
            return;
        }

        if (introVidStarted && !introVidIsDone)
        {
            introVidIsDone = true;
            PlayScanPrompt();
            useIntroVideoGate = false;
        }
    }

    async void PlayScanPrompt()
    {
        await Task.Delay(10000);
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySpeechSFX("oai_facial_scan_sp");
        }
        scanPromptDone = true;
    }

    void TryTrigger_FacialScanner(Collider other)
    {
        if (useIntroVideoGate && !scanPromptDone)
        {
            Debug.Log("[ColliderManager] Gated: waiting for scan prompt to complete.");
            return;
        }
        // update prompt to fit FacialScanner and infrence
        demoInstance.prompt = promptOnEmotionalScan;
        _ = demoInstance.RunOnce();

        if (faceScannerObj != null)
        {
            var renderer = faceScannerObj.GetComponent<Renderer>();
            if (renderer != null && greenMaterial != null)
            {
                // instance material change
                renderer.material = greenMaterial;
                facialScanTriggered = true;
            }
        }
        if (doorObj != null) doorObj.SetActive(false);

        cooldown = true;
        Invoke(nameof(ResetCooldown), cooldownSeconds);
    }

    void TryTrigger_ExerciseScanner(Collider other)
    {
        // Placeholder for future ExerciseScanner logic
    }
    void ResetCooldown() => cooldown = false;

    void OnValidate()
    {
        var col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning("[ColliderManager] Collider is not marked as Trigger. OnTrigger events may not fire unless the other collider has a Rigidbody and physics matrix allows it.", this);
        }
    }
}
