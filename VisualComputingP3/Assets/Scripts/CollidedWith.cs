using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BoxPlaySfxOnEnter : MonoBehaviour
{
    [Header("Assign in Inspector (can be inactive)")]
    [SerializeField] private GameObject jumpScareObject;
    
    private Camera mainCamera;
    private Vector3 originalCameraPosition;
    private bool playerInside = false;

    private void Awake()
    {
        if (jumpScareObject == null)
            Debug.LogWarning("[BoxPlaySfxOnEnter] JumpScare object not assigned!");
            
        mainCamera = Camera.main;
        if (mainCamera == null)
            Debug.LogWarning("[BoxPlaySfxOnEnter] Main camera not found!");
            
        originalCameraPosition = mainCamera.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (playerInside) return;

        playerInside = true;
        Debug.Log("Play SFX here");

        // Guard against missing SoundManager too
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayOneShotSFX("leverTree_OS");
        else
            Debug.LogWarning("SoundManager.Instance is null");

        if (jumpScareObject != null)
            jumpScareObject.SetActive(true);
        else
            Debug.LogError("jumpScareObject is null — assign it in the Inspector.");

        // Move camera to collider edge
        if (mainCamera != null)
        {
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                // Get the closest point on the collider bounds to the player
                Vector3 closestPoint = collider.ClosestPoint(other.transform.position);
                mainCamera.transform.position = closestPoint;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerInside = false;
        
        // Reset camera position
        if (mainCamera != null)
        {
            mainCamera.transform.position = originalCameraPosition;
        }
    }
}
// needed: We need a script that can enable movement and mouse scripts to deactivate them in this script.