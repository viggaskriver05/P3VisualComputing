using UnityEngine;
using System;
using System.Collections;

public class AdaptiveSound : MonoBehaviour
{

    public float coolDownDuration = 5f;
    public float coolDownDurationVoice = 10f;
    float nextPlayTimeVoice = 0f;
    float nextPlayTimeHeartbeat = 0f;
  
    [SerializeField] EmotionOscListener listener;
    [SerializeField] OpenAIResponseClient responseClient;
    [SerializeField] OpenAiResponseToTtsWEmotionDemo demoInstance;
    [SerializeField] PlayerInteraction playerInteraction;
    [SerializeField] ColliderManager colliderManager;

    public bool useCacheSound = true;



    void Awake()
    {
        listener = UnityEngine.Object.FindFirstObjectByType<EmotionOscListener>();
        responseClient = UnityEngine.Object.FindFirstObjectByType<OpenAIResponseClient>();
        demoInstance = UnityEngine.Object.FindFirstObjectByType<OpenAiResponseToTtsWEmotionDemo>();
        playerInteraction = UnityEngine.Object.FindFirstObjectByType<PlayerInteraction>();
        colliderManager = UnityEngine.Object.FindFirstObjectByType<ColliderManager>();
    }

    void Update()
    {
        // Start heartbeat coroutine when appropriate
        if (listener != null && listener.DominantEmotion == "fear")
        {
            if (Time.time >= nextPlayTimeHeartbeat && playerInteraction.hasEatenFood)
            {
                nextPlayTimeHeartbeat = Time.time + coolDownDuration;
                StartCoroutine(PlayHeartbeatSound());
            }
        }
        if (playerInteraction.hallwayScanTriggered && colliderManager.hasSpokenEndingPrompt == false)
        {
            PlayAdaptiveSound();
        } 
    }

    async void PlayAdaptiveSound() {
        if (Time.time < nextPlayTimeVoice) return;
        nextPlayTimeVoice = Time.time + coolDownDurationVoice;

        Debug.Log("[AdaptiveSound] Triggering adaptive voice.");
        
        bool useCacheHelper = responseClient.useCachedPrompts;
        responseClient.useCachedPrompts = useCacheSound; // force new response
        var promptHelper = demoInstance.prompt;
        var toneHelper = demoInstance.toneOverride;

        var dominant = listener != null ? listener.DominantEmotion : "unknown";
        demoInstance.prompt = "The player is trying to escape, comment on their emotional state";
        demoInstance.facialEmotionOverride = dominant;
        demoInstance.toneOverride = "angry";
        demoInstance.RunOnce();
        
        demoInstance.prompt = promptHelper;
        demoInstance.toneOverride = toneHelper;
        responseClient.useCachedPrompts = useCacheHelper;
    }
    IEnumerator PlayHeartbeatSound()
    {
        SoundManager.Instance.PlayAmbienceSFX("heartbeatFast_amb");
        Debug.Log("[AdaptiveSound] Playing heartbeat sound due to fear emotion.");
        yield return new WaitForSeconds(coolDownDuration);
        SoundManager.Instance.Stop("heartbeatFast_amb");
        Debug.Log("[AdaptiveSound] Stopping heartbeat sound.");
    }
}