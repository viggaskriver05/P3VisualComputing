using UnityEngine;

public class OscJackDemo : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        var emo = FindFirstObjectByType<EmotionOscListener>()?.CurrentEmotion;
        Debug.Log($"Current Emotion: {emo}");
    }
}
