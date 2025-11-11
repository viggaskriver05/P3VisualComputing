using System.Threading.Tasks;
using UnityEngine;
public class FacialScanColliderManager2000 : MonoBehaviour
{

    private OpenAiResponseToTtsWEmotionDemo demoInstance;
    private EmotionOscListener oscClientInstance;

    [Header("Trigger Settings")]
    [SerializeField] private bool introVidIsDone = false;
    [SerializeField] private bool scanPromptDone = false;
    private bool introVidStarted = false;
    [SerializeField] private string requiredTag = "FacialScanner";
    [SerializeField] private string requiredEmotion = "happy";
    [SerializeField] private float cooldownSeconds = 2.0f;
    [TextArea]
    [SerializeField] private string promptOnEmotionalScan = "The user is looking into a facial scanner while doing their daily emotional scan";

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

        // minimal: no extra diagnostics
    }

    void Update()
    {
        if (!introVidIsDone)
        {
            CheckIfIntroDone();
            if (introVidIsDone)
            {
            // ready to scan
            _ = PlayScanPrompt();
            }
        }
        
        
    }
    async Task PlayScanPrompt()
    {
        await Task.Delay(10000);
        SoundManager.Instance.PlaySpeechSFX("oai_facial_scan_sp");
        scanPromptDone = true;
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
        //if (!scanPromptDone) return;

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
            demoInstance.prompt = promptOnEmotionalScan;
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

    void CheckIfIntroDone()
    {
        // the intro video is expected on an object named "introvid" with a VideoPlayer
        var introVideoObj = GameObject.Find("introvid");
        if (introVideoObj == null)
        {
            Debug.LogWarning("FacialScanColliderManager2000: No intro video object found in scene.");
            return;
        }

        var videoPlayer = introVideoObj.GetComponent<UnityEngine.Video.VideoPlayer>();
        if (videoPlayer == null)
        {
            Debug.LogWarning("FacialScanColliderManager2000: 'introvid' has no VideoPlayer component.");
            return;
        }

        // Transition logging: only log on state change
        if (videoPlayer.isPlaying)
        {
            if (!introVidStarted)
            {
                Debug.Log("FacialScanColliderManager2000: Vid playing.");
                introVidStarted = true;
            }
            return; // still playing
        }

        // Not playing: only mark done if we previously observed it start
        if (introVidStarted && !introVidIsDone)
        {
            Debug.Log("FacialScanColliderManager2000: Vid is done.");
            introVidIsDone = true;
        }
    }

    void ResetCooldown() => cooldown = false;
}
