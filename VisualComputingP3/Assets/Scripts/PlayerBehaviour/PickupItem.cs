using UnityEngine;
using System.Threading.Tasks;

public class PickupItem : InteractionScript
{
    public bool pickable = true;
    public GameObject pill;
    public override void Interact()
    {
        PlayerInteraction player = FindAnyObjectByType<PlayerInteraction>();

        // Gates for certain items
        if (CompareTag("Pill") && !player.hasReadToDoList) return;
        if (CompareTag("StaffCard") && !player.hasEatenFood) return; 

        if (CompareTag("ToDoList"))
        {
            if (!player.hasReadToDoList)
            {
                SoundManager.Instance.Stop("oai_take_pills_prompt_sp");
                SoundManager.Instance.PlaySpeechSFX("oai_take_pills_prompt_sp");
                player.hasReadToDoList = true;
                Debug.Log("You have read the To Do list!");
            }
        }

        if (CompareTag("Pill")) {
            if (!player.hasSwallowedPill)
            {
                SoundManager.Instance.Stop("oai_facial_scan_sp");
                SoundManager.Instance.PlaySpeechSFX("oai_facial_scan_sp");
                player.hasSwallowedPill = true;
                Debug.Log("You have swallowed a pill.");
                pill.SetActive(false);
            }
        }
        
        if (CompareTag("Food"))
        {
            SoundManager.Instance.PlaySpeechSFX("oai_food_prompt_sp");
            player.hasEatenFood = true;
            Debug.Log("You have eaten the food!");
            gameObject.SetActive(false);
        }

        if (CompareTag("StaffCard"))
        {
            SoundManager.Instance.PlaySpeechSFX("go_sp");
            player.hasPickedUpStaffCard = true;
            Debug.Log($"GameObject with tag {gameObject.tag} activated StaffCard; destroying objects tagged 'Lights'.");
            GameObject[] lights = GameObject.FindGameObjectsWithTag("Lights");
            foreach (var lightObj in lights)
            {
                Destroy(lightObj, 2f);
            }
            Debug.Log("You have picked up the Staff Card!");
        }

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
    }
}
