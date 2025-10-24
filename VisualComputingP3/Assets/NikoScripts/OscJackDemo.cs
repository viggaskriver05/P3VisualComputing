using UnityEngine;

public class OscJackDemo : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        var emo = FindObjectOfType<EmotionOscListener>()?.CurrentEmotion;
        Debug.Log($"Current Emotion: {emo}");
    }
}
