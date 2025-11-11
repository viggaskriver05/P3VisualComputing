using UnityEngine;
using static UnityEngine.Object;

public class Door : InteractionScript
{
    public bool isOpen = false;
    public bool needsKey = true;
    public Transform LDoor;
    public Transform RDoor;
    public float openWidth = 1f;
    //private float openSpeed = 3f;

    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {


    }

    public override void Interact()
    {
        PlayerInteraction player = FindAnyObjectByType<PlayerInteraction>();
        

        if (player.isHoldingKey || !needsKey)
        {
            LDoor.position = new Vector3(LDoor.position.x, LDoor.position.y, LDoor.position.z - openWidth);
            RDoor.position = new Vector3(RDoor.position.x, RDoor.position.y, RDoor.position.z + openWidth);

            isOpen = !isOpen;
            Debug.Log("Door " + (isOpen ? "opened." : "closed."));
        }
        else
        {
            Debug.Log("The door is locked. You need a key to open it.");
        }
    }
}
