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
    public bool facialScanTriggered = false;
    public bool hasEatenFood = false;
    public bool hasPickedUpStaffCard = false;
    public bool hallwayScanTriggered = false;
    public bool hasBeenScared = false;

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
                if (pickup.CompareTag("ToDoList") && hasReadToDoList || pickup.CompareTag("Pill") && hasSwallowedPill || pickup.CompareTag("FacialScanner") && facialScanTriggered || pickup.CompareTag("Food") && hasEatenFood || pickup.CompareTag("StaffCard") && hasPickedUpStaffCard || pickup.CompareTag("Jumpscare") && hasBeenScared)
                {
                    interactionPromptText.gameObject.SetActive(false);
                    return;
                }
                
                // Show prompt
                string itemTag = hit.collider.tag;
                //specific for the facialscanner
                if (pickup.CompareTag("ToDoList")) {
                    interactionPromptText.text = $"Press {interactionKey} to read to-do list";
                    if (!interactionPromptText.gameObject.activeSelf) interactionPromptText.gameObject.SetActive(true);
                    return;
                } 
                if (pickup.CompareTag("Pill") && hasReadToDoList) {
                    interactionPromptText.text = $"Press {interactionKey} to take your pills";
                    if (!interactionPromptText.gameObject.activeSelf) interactionPromptText.gameObject.SetActive(true);
                    return;
                }
                if (pickup.CompareTag("FacialScanner") && hasSwallowedPill) {
                    interactionPromptText.text = $"Give a big smile and look into the scanner to start your emotional scan!";
                    if (!interactionPromptText.gameObject.activeSelf) interactionPromptText.gameObject.SetActive(true);
                    return;
                } 
                if (pickup.CompareTag("Food")) {
                    interactionPromptText.text = $"Press {interactionKey} to eat your food!";
                    if (!interactionPromptText.gameObject.activeSelf) interactionPromptText.gameObject.SetActive(true);
                    return;
                } 
                if (pickup.CompareTag("StaffCard")) {
                    interactionPromptText.text = $"Press {interactionKey} to pick up staff card!";
                    if (!interactionPromptText.gameObject.activeSelf) interactionPromptText.gameObject.SetActive(true);
                    return;
                }
                if (pickup.CompareTag("Jumpscare"))
                {
                    interactionPromptText.text = $"Press {interactionKey} to peek into room";
                    if (!interactionPromptText.gameObject.activeSelf) interactionPromptText.gameObject.SetActive(true);
                    return;
                }
                if (pickup.CompareTag("exitDoor"))
                {
                    interactionPromptText.text = $"Press {interactionKey} to exit the facility";
                    if (!interactionPromptText.gameObject.activeSelf) interactionPromptText.gameObject.SetActive(true);
                    return;
                }

                if (!interactionPromptText.gameObject.activeSelf) interactionPromptText.gameObject.SetActive(false);

                return;
            }
        }

        // Hide if not looking at a pickup
        if (interactionPromptText.gameObject.activeSelf)
            interactionPromptText.gameObject.SetActive(false);
    }
}


