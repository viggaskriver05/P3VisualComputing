using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    float interactionRange = 5f;
    public KeyCode interactionKey = KeyCode.E;
    public Camera playerCamera;
    public Transform rightHandPosition;
    public bool hasReadToDoList = false;
    public bool isHoldingKey = false;
    public bool hasSwallowedPill = false;

    [Header("UI")]
    public TMPro.TextMeshProUGUI interactionPromptText;

    void Start()
    {
        // Hide prompt at start
        if (interactionPromptText != null) interactionPromptText.gameObject.SetActive(false);
    }

    void Update()
    {
        
        TryInteract();

        // Always update the on-screen prompt based on what the player is looking at 
        DisplayInteractionPrompt();
    }
    
    void TryInteract() {
         if (Input.GetKeyDown(interactionKey))
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionRange))
            {
                // Use GetComponentInParent so interaction scripts on parent objects still work when colliders are on children
                InteractionScript interactionScript = hit.collider.GetComponentInParent<InteractionScript>();
                if (interactionScript != null)
                {
                    interactionScript.Interact();
                }
            }
        }
    }

    public void DisplayInteractionPrompt()
    {
        // Super-simple: raycast from center, if a PickupItem is hit, show "Press E to pick up <tag>"
        if (interactionPromptText == null || playerCamera == null)
            return;

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange))
        {
            var pickup = hit.collider.GetComponentInParent<PickupItem>();
            if (pickup != null)
            {
                // Don't show prompt for ToDoList if already read
                if (hasReadToDoList && pickup.CompareTag("ToDoList"))
                {
                    interactionPromptText.gameObject.SetActive(false);
                    return;
                }
                // Show prompt
                string itemTag = hit.collider.tag;
                //specific for the facialscanner
                interactionPromptText.text = $"Press {interactionKey} to pick up {itemTag}";
                if (!interactionPromptText.gameObject.activeSelf)
                    interactionPromptText.gameObject.SetActive(true);
                return;
            }
        }

        // Hide if not looking at a pickup
        if (interactionPromptText.gameObject.activeSelf)
            interactionPromptText.gameObject.SetActive(false);
    }
}


