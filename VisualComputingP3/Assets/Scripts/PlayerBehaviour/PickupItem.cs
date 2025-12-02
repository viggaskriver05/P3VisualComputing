using UnityEngine;
using System.Threading.Tasks;


public class PickupItem : InteractionScript
{
    public bool pickable = true;
    public GameObject pill;
    private GameRestarter gameRestarter;
    public GameObject alarm;

    public void Start()
    {
        gameRestarter = FindObjectOfType<GameRestarter>();
    }

    public override void Interact()
    {
        PlayerInteraction player = FindAnyObjectByType<PlayerInteraction>();

        // Gates for certain items
        if (CompareTag("Pill") && !player.hasReadToDoList) return;

        if (CompareTag("StaffCard") && !player.hasEatenFood) return;

        if (CompareTag("FacialScanner") && !player.hasSwallowedPill) return;

        // Trashcan: play speech SFX and stop further interaction
        if (CompareTag("StaffCard"))
        {
            SoundManager.Instance.PlaySpeechSFX("oai_lockdown_mode_sp");
            
            player.hasPickedUpStaffCard = true;
            player.isHoldingKey = true;
            Debug.Log($"GameObject with tag {gameObject.tag} activated StaffCard; destroying objects tagged 'Lights'.");

            // Find and destroy all GameObjects with tag "Lights" after 5 seconds
            GameObject[] lights = GameObject.FindGameObjectsWithTag("Lights"); 
            foreach (var lightObj in lights)
            {
                Destroy(lightObj);
            }
            alarm.SetActive(true);
        }

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
        
        if (CompareTag("Food") && !player.hasEatenFood)
        {
            EatFood eatFood = GetComponent<EatFood>();
            SoundManager.Instance.PlaySpeechSFX("oai_food_prompt_sp");
            player.hasEatenFood = true;
            Debug.Log("You have eaten the food!");
            if (eatFood != null)
            {
                StartCoroutine(eatFood.EatFoodAnimation());
            }
            else
            {
                Debug.LogWarning("EatFood component not found on the food item.");
            }
        }

        if (CompareTag("Jumpscare") && !player.hasBeenScared)
        {
            DoorJumpscare jumpscare = GetComponent<DoorJumpscare>();
            if (jumpscare != null)
            {
                StartCoroutine(jumpscare.Jumpscare());
            }
            player.hasBeenScared = true;
        }

        if (CompareTag("exitDoor")) 
        {
            gameRestarter.RestartGame();
            SoundManager.Instance.StopAll();
        }

        if (player != null && pickable)
        {
            Transform rightHandPos = player.rightHandPosition;
            transform.SetParent(rightHandPos);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

    }
}
