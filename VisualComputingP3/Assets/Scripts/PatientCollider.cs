using UnityEngine;

public class PatientCollider : MonoBehaviour
{
    private bool hasTriggered = false;
    public GameObject targetGameObject;
    public AudioSource targetAudioSource;

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
            
            if (targetAudioSource != null)
            {
                targetAudioSource.enabled = true;
                targetAudioSource.Play();
            }
            
            //SoundManager.Instance.PlayOneShotSFX("P0");
        }
    }
}
