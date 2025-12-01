using UnityEngine;

public class PatientCollider : MonoBehaviour
{
    private bool hasTriggered = false;
    public GameObject targetGameObject;
    public AudioSource targetAudioSource1;
    public GameObject targetAudioAlarm;

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            Debug.Log("Player collided with P0");
            
            if (targetGameObject != null)
            {
                targetGameObject.SetActive(true);
            }
            
            if (targetAudioSource1 != null)
            {
                targetAudioSource1.enabled = true;
                targetAudioSource1.Play();
            }
             if (targetAudioAlarm != null)
            {
                targetAudioAlarm.SetActive(true);
                Debug.Log("Alarm activated");
            }
            
            //SoundManager.Instance.PlayOneShotSFX("P0");
        }
    }
}
