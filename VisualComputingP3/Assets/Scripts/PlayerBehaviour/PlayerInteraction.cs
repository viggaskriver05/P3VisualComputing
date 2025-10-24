using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    float interactionRange = 5f;
    public KeyCode interactionKey = KeyCode.E;
    public Camera playerCamera;
    public Transform rightHandPosition;
    public bool isHoldingKey = false;

    void Update()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionRange))
            {
                InteractionScript interactionScript = hit.collider.GetComponent<InteractionScript>();
                if (interactionScript != null)
                {
                    interactionScript.Interact();
                }
            }
        }
    }
}
