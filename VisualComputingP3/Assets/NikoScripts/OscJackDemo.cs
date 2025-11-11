using System;
using UnityEngine;
using System.Collections.Generic;

public class OscJackDemo : MonoBehaviour
{
    [SerializeField] EmotionOscListener listener;

    string current = "unknown";
    string dominant = "unknown";

    void Awake()
    {
            if (listener == null)
            {
                listener = UnityEngine.Object.FindFirstObjectByType<EmotionOscListener>();
            }
    }

    // Update is called once per frame
    void Update()
    {
        current = listener != null ? listener.CurrentEmotion : "unknown";
        dominant = listener != null ? listener.DominantEmotion : "unknown";

        Debug.Log($"Current Emotion: {current}");
        Debug.Log($"Dominant Emotion (listener): {dominant}");
    }
    }
