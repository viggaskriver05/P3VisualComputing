using UnityEngine;

public class SoundStarter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SoundManager.Instance.PlayMusicSFX("quest_mus");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
