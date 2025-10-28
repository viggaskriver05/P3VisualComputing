using UnityEngine;

public class Door : InteractionScript
{
    public bool isOpen = false;
    public Transform doorHinge;
    [SerializeField] float openAngle = 90f;
    [SerializeField] float openSpeed = 3f;

    Quaternion closedRotation;
    Quaternion openRotation;

    void Start()
    {
        if (doorHinge == null)
            doorHinge = transform;

        closedRotation = doorHinge.rotation;
        openRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
    }

    void Update()
    {
        Quaternion target = isOpen ? openRotation : closedRotation;
        doorHinge.rotation = Quaternion.Lerp(
            doorHinge.rotation,
            target,
            Time.deltaTime * openSpeed
        );
    }

    public override void Interact()
    {
        PlayerInteraction player = FindAnyObjectByType<PlayerInteraction>();

        if (player != null && player.isHoldingKey)
        {
            isOpen = !isOpen;
            Debug.Log("Door " + (isOpen ? "opened." : "closed."));

            if (isOpen)
            {
                GameDirector gd = FindAnyObjectByType<GameDirector>();
                if (gd != null)
                    gd.OnDoorScanAccepted(); // notify GameDirector
            }
        }
        else
        {
            Debug.Log("The door is locked. You need a key to open it.");
        }
    }
}
