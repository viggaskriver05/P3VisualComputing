using UnityEngine;

public class PickupItem : InteractionScript
{
    public bool pickable = true;
    public GameObject pill;
    public override void Interact()
    {
        PlayerInteraction player = FindAnyObjectByType<PlayerInteraction>();

        // Gate: Pills only interactable after ToDo has been read
        if (CompareTag("Pill") && !player.hasReadToDoList) return;
        if (CompareTag("FacialScanner") && !player.hasSwallowedPill) return;

        // Trashcan: play speech SFX and stop further interaction
        if (CompareTag("StaffCard"))
        {
            SoundManager.Instance.PlaySpeechSFX("go_sp");
            Debug.Log($"GameObject with tag {gameObject.tag} activated StaffCard; destroying objects tagged 'Lights'.");

            // Find and destroy all GameObjects with tag "Lights" after 5 seconds
            GameObject[] lights = GameObject.FindGameObjectsWithTag("Lights");
            foreach (var lightObj in lights)
            {
                Destroy(lightObj, 5f);
            }
            Destroy(gameObject);
            return;
        }
        
        if (CompareTag("Food"))
        {
            SoundManager.Instance.PlaySpeechSFX("go_sp");
            Destroy(gameObject);
            return;
        }

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
                pill.SetActive(false);
            }
        }
        if (CompareTag("FacialScanner")) return;
        
        if (player != null && pickable)
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
