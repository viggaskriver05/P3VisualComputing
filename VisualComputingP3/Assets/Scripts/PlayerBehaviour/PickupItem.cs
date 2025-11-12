using UnityEngine;

public class PickupItem : InteractionScript
{
    public override void Interact()
    {
        PlayerInteraction player = FindAnyObjectByType<PlayerInteraction>();

        Debug.Log("Item picked up!");

        if (CompareTag("ToDoList")) {
            if (!player.hasReadToDoList)
            {
                player.hasReadToDoList = true;
                Debug.Log("You have read the To Do list!");
            }
            return;
        }
        

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
