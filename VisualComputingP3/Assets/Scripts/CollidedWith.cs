using UnityEngine;

public class BoxPlaySfxOnEnter : MonoBehaviour
{
    

    // Track whether the player is currently inside this trigger
    private bool playerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (playerInside) return;                // already handled this entry (e.g., another child collider)

        playerInside = true;
        Debug.Log("Play SFX here");
        SoundManager.Instance.PlayOneShotSFX("leverTree_OS");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;                    // allow next entry to play again
    }
}
