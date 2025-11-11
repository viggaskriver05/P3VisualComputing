using UnityEngine;

public class IntroVideoSound : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SoundManager.Instance.PlaySpeechSFX("oai_pre_video_sp");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
