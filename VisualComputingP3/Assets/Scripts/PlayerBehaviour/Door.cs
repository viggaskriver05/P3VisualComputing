using UnityEngine;

public class Door : InteractionScript
{
    public bool isOpen = false;
    public Transform doorHinge;
    private float openAngle = 90f;
    private float openSpeed = 3f;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        if (doorHinge == null)
        {
            doorHinge = this.transform;
        }

        closedRotation = doorHinge.rotation;
        openRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        doorHinge.rotation = Quaternion.Lerp(doorHinge.rotation, targetRotation, Time.deltaTime * openSpeed);
    }

    public override void Interact()
    {
        PlayerInteraction player = FindAnyObjectByType<PlayerInteraction>();
        

        if (player.isHoldingKey)
        {
            isOpen = !isOpen;
            Debug.Log("Door " + (isOpen ? "opened." : "closed."));
        }
        else
        {
            Debug.Log("The door is locked. You need a key to open it.");
        }
    }
}
