using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;
//using Unity.Events;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class ColliderManager : MonoBehaviour
{
    // Shared dependencies
    private OpenAiResponseToTtsWEmotionDemo demoInstance;
    private EmotionOscListener oscClientInstance;
    private PlayerInteraction player;
    private Door door;


    [Header("Universal Trigger Base Settings")]
    [SerializeField] private bool triggerOnEnter = true;
    [SerializeField] private bool triggerOnStay = true;
    float coolDownDuration = 4f;
    float nextPlayEndingScan = 0f;


    // FacialScanner-specific (active when GameObject.tag == "FacialScanner")
    [Header("FacialScanner Settings (active when GameObject.tag == 'FacialScanner')")]
    [SerializeField] private string requiredEmotionHappy = "happy";
    [SerializeField] private string requiredEmotionNegativeHallway = "negative";
    [SerializeField] private string requiredEmotionSurprisedEnding = "surprise";
    [SerializeField] private string requiredEmotionHappyEnding = "happy";
    [SerializeField] private string requiredEmotionSadEnding = "sad";
    [SerializeField] public bool facialScanTriggered = false;
    [SerializeField] public bool hallwayScanTriggered = false;
    [SerializeField] public bool endingScan1Triggered = false;
    [SerializeField] public bool endingScan2Triggered = false;
    [SerializeField] public bool endingScan3Triggered = false;
    [SerializeField] public bool hasSpokenEndingPrompt = false;
    //[SerializeField] public UnityEvent oneFacialScanSuccess;
    [TextArea]
    [SerializeField] private string promptOnEmotionalScan = "The user is looking into a facial scanner while doing their daily emotional scan. Tell the player to eat their food.";
    [SerializeField] private string promptOnHallwayScan = "The user has opened a door which is not supposed to be opened. The output form this prompt should be angry outbursts from the AI.";
    [SerializeField] private GameObject faceScannerObj;
    [SerializeField] private Material greenMaterial;
    [SerializeField] public GameObject doorObj;
    [SerializeField] public GameObject keyCard;

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
        if (player == null) player = FindFirstObjectByType<PlayerInteraction>();
#else
        if (demoInstance == null) demoInstance = FindObjectOfType<OpenAiResponseToTtsWEmotionDemo>();
        if (oscClientInstance == null) oscClientInstance = FindObjectOfType<EmotionOscListener>();
        if (player == null) player = FindObjectOfType<PlayerInteraction>();
#endif
    }

    void Update()
    {
        // Only run the intro gating logic while attached to a FacialScanner-tagged trigger.
        if (this.CompareTag("FacialScanner"))
        {
            FacialScanner_UpdateGate();
        }
        if (this.CompareTag("HallwayScanner"))
        {
            FacialScanner_UpdateGate();
        }
        if (this.CompareTag("EndingScanner"))
        {
            FacialScanner_UpdateGate();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!triggerOnEnter) return;
        //Debug.Log($"[ColliderManager] OnTriggerEnter by '{other.name}' (tag='{other.tag}') on trigger '{name}' (tag='{tag}')");
        HandleTriggerFacialScanner(other);
    }

    void OnTriggerStay(Collider other)
    {
        if (!triggerOnStay) return;
        //Debug.Log($"[ColliderManager] OnTriggerStay by '{other.name}' (tag='{other.tag}') on trigger '{name}' (tag='{tag}')");
        HandleTriggerFacialScanner(other);
    }

    void HandleTriggerFacialScanner(Collider other)
    {
        //player.DisplayInteractionPrompt();
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
        var sentiment = oscClientInstance.SentimentEmotion ?? "unknown";
        //Debug.Log("[ColliderManager] FacialScanner candidate. Dominant emotion = " + dominant);
        if (CompareTag("FacialScanner") && !string.Equals(dominant, requiredEmotionHappy, System.StringComparison.OrdinalIgnoreCase)) return;
        if (CompareTag("HallwayScanner") && !string.Equals(sentiment, requiredEmotionNegativeHallway, System.StringComparison.OrdinalIgnoreCase)) return;
        demoInstance.facialEmotionOverride = dominant;
        demoInstance.toneOverride = "happy";
        if (CompareTag("HallwayScanner")) { demoInstance.facialEmotionOverride = sentiment; demoInstance.toneOverride = "angry";SoundManager.Instance.SpeechSource.pitch = 1.02f; }


        // Use this.CompareTag to branch by the trigger object's own tag
        if (this.CompareTag("FacialScanner") && !facialScanTriggered)
        {
            if (!player.hasSwallowedPill) return; // skip if pill is not yet swallowed
            TryTrigger_FacialScanner(other);
        } 
        // Extend with more tag-specific behaviors here (else if CompareTag("Door") ... )
        if (this.CompareTag("HallwayScanner") && !hallwayScanTriggered)
        {
            if (!player.hasPickedUpStaffCard) return; // skip if staff card not yet picked up
            TryTrigger_FacialScanner(other);
        } 
        if (this.CompareTag("EndingScanner"))
        {
            // Cooldown always checked once for any ending attempt
            if (!CheckCoolDown()) return;

            // Play the generic ending prompt once, before any specific ending
            if (!hasSpokenEndingPrompt)
            {
                SoundManager.Instance.PlaySpeechSFX("oai_ending_prompt_sp");
                hasSpokenEndingPrompt = true;
                return; // wait for next approach / frame after the prompt
            }

            // Now pick exactly ONE ending per call
            if (!endingScan1Triggered &&string.Equals(dominant, requiredEmotionHappyEnding, System.StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("[ColliderManager] EndingScanner: detected surprised emotion, triggering ending scan 1.");
                TryTrigger_FacialScanner(other);
                return;
            }

            if (!endingScan2Triggered && endingScan1Triggered && string.Equals(dominant, requiredEmotionSurprisedEnding, System.StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("[ColliderManager] EndingScanner: detected happy emotion, triggering ending scan 2.");
                TryTrigger_FacialScanner(other);
                return;
            }

            if (!endingScan3Triggered && endingScan2Triggered && string.Equals(dominant, requiredEmotionSadEnding, System.StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("[ColliderManager] EndingScanner: detected sad emotion, triggering ending scan 3.");
                TryTrigger_FacialScanner(other);
                return;
            }
        }
    }

   bool CheckCoolDown()
{
    if (Time.time < nextPlayEndingScan)
    {
        // still on cooldown
        return false;
    }

    nextPlayEndingScan = Time.time + coolDownDuration;
    return true; // allowed to play
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
            //PlayWelcomePrompt();
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
            //PlayWelcomePrompt();
            useIntroVideoGate = false;
        }
    }

    void PlayWelcomePrompt()
    {
        //SoundManager.Instance.PlaySpeechSFX("oai_welcome_sp");
    }

    async Task TryTrigger_FacialScanner(Collider other)
    {
        if (faceScannerObj != null)
        {
            if (useIntroVideoGate && !scanPromptDone)
            {
                Debug.Log("[ColliderManager] Gated: waiting for scan prompt to complete.");
                return;
            }

            // update prompt to fit FacialScanner and inference
            if (CompareTag("FacialScanner"))
            {
                demoInstance.prompt = promptOnEmotionalScan;
                facialScanTriggered = true;
            }
            else if (CompareTag("HallwayScanner"))
            {
                demoInstance.prompt = promptOnHallwayScan;
                player.hallwayScanTriggered = true;
                hallwayScanTriggered = true;
            }
            else if (CompareTag("EndingScanner"))
            {
                // Handle endings with fixed speech, no AI call
                if (!endingScan1Triggered)
                {
                    SoundManager.Instance.PlaySpeechSFX("oai_ending_one_sp");
                    endingScan1Triggered = true;
                    return;
                }
                else if (!endingScan2Triggered)
                {
                    SoundManager.Instance.PlaySpeechSFX("oai_ending_two_sp");
                    endingScan2Triggered = true;
                    return;
                }
                else if (!endingScan3Triggered)
                {
                    SoundManager.Instance.PlaySpeechSFX("oai_ending_three_sp");
                    endingScan3Triggered = true;
                }
            }

            // For non-ending scanners, run the AI response
            if (!CompareTag("EndingScanner"))
            {
                _ = demoInstance.RunOnce();
            }

            // Visual feedback on scanner object
            var renderer = faceScannerObj.GetComponent<Renderer>();
            if (renderer != null && greenMaterial != null)
            {
                var mats = renderer.materials;
                if (mats.Length > 2)
                {
                    mats[2] = greenMaterial;
                    renderer.materials = mats;
                }

                if (CompareTag("FacialScanner"))
                {
                    await Task.Delay(4000);
                    SoundManager.Instance.PlaySpeechSFX("oai_eat_food_prompt_sp");
                }
            }
        }

        // Door logic (independent of faceScannerObj)
        if (doorObj != null)
        {
            Door door = doorObj.GetComponent<Door>();
            if (door.needsKey && !player.isHoldingKey)
            {
                Debug.Log("Door needs a key and player is not holding one. Cannot open door.");
                return;
            }
            else if (!door.needsKey)
            {
                Debug.Log("Door does not need a key. Opening door.");
                StartCoroutine(door.OpenNow());
            }
            else
            {
                Debug.Log("Starting door animation");
                keyCard.SetActive(false);
                StartCoroutine(door.OpenNow());
            }
        }
    }
}
