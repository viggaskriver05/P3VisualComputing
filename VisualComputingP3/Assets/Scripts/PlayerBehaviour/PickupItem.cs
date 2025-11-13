using UnityEngine;

public class PickupItem : InteractionScript
{
    public override void Interact()
    {
        PlayerInteraction player = FindAnyObjectByType<PlayerInteraction>();

        // Gate: Pills only interactable after ToDo has been read
        if (CompareTag("Pill") && !player.hasReadToDoList) return;
        if (CompareTag("FacialScanner") && !player.hasSwallowedPill) return;

        if (CompareTag("ToDoList"))
        {
            if (!player.hasReadToDoList)
            {
                SoundManager.Instance.PlaySpeechSFX("oai_take_pills_prompt_sp");
                player.hasReadToDoList = true;
                Debug.Log("You have read the To Do list!");
            }
            return;
        }
        if (CompareTag("Pill")) {
            if (!player.hasSwallowedPill)
            {
                SoundManager.Instance.PlaySpeechSFX("oai_facial_scan_sp");
                player.hasSwallowedPill = true;
                Debug.Log("You have swallowed a pill.");
            }
        }
        if (CompareTag("FacialScanner")) return;
        
        if (player != null)
        {
            Transform rightHandPos = player.rightHandPosition;
            transform.SetParent(rightHandPos);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        if (CompareTag("Key"))
        {
            player.isHoldingKey = true;
            Debug.Log("You have picked up a key!");
        }
        if (CompareTag("Pill"))
        {
            player.hasSwallowedPill = true;
            Debug.Log("You have swallowed a pill!");
        }
    }
}
