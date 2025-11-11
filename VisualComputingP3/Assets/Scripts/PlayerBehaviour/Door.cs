using UnityEngine;
using static UnityEngine.Object;

public class Door : InteractionScript
{
    public bool isOpen = false;
    public bool needsKey = true;
    public Transform LDoor;
    public Transform RDoor;
    public float openWidth = 1f;
    public float openSpeed = 3f;

    private Vector3 lDoorClosedPos, lDoorOpenPos;
    private Vector3 rDoorClosedPos, rDoorOpenPos;
    private float lerpProgress = 1f; // 1 = at target, 0 = just started
    private bool isMoving = false;

    void Start()
    {
        lDoorClosedPos = LDoor.position;
        rDoorClosedPos = RDoor.position;
        lDoorOpenPos = lDoorClosedPos + new Vector3(0, 0, -openWidth);
        rDoorOpenPos = rDoorClosedPos + new Vector3(0, 0, openWidth);
    }

    void Update()
    {
        if (isMoving)
        {
            lerpProgress += Time.deltaTime * openSpeed;
            if (lerpProgress >= 1f)
            {
                lerpProgress = 1f;
                isMoving = false;
            }

            if (isOpen)
            {
                LDoor.position = Vector3.Lerp(lDoorClosedPos, lDoorOpenPos, lerpProgress);
                RDoor.position = Vector3.Lerp(rDoorClosedPos, rDoorOpenPos, lerpProgress);
            }
            else
            {
                LDoor.position = Vector3.Lerp(lDoorOpenPos, lDoorClosedPos, lerpProgress);
                RDoor.position = Vector3.Lerp(rDoorOpenPos, rDoorClosedPos, lerpProgress);
            }
        }
    }

    public override void Interact()
    {
        PlayerInteraction player = FindAnyObjectByType<PlayerInteraction>();

        if (player.isHoldingKey || !needsKey)
        {
            isOpen = !isOpen;
            lerpProgress = 0f;
            isMoving = true;
            Debug.Log("Door " + (isOpen ? "opened." : "closed."));
        }
        else
        {
            Debug.Log("The door is locked. You need a key to open it.");
        }
    }
}
