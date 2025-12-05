using UnityEngine;
using System.Collections;
using static UnityEngine.Object;

public class Door : InteractionScript
{

    private bool isUnlocked = false;
    public bool needsKey = false;
    public Transform LDoor;
    public Transform RDoor;
    public float openWidth;
    public float openSpeed = 0.5f;
    public bool facialScanNeeded = true;
    public bool openRotated = false;

    private Vector3 lDoorClosedPos, lDoorOpenPos;
    private Vector3 rDoorClosedPos, rDoorOpenPos;
    private float lerpProgress = 1f; // 1 = at target, 0 = just started
    //public bool isMoving = false;
    private bool isOpen;
    //[SerializeField] private ColliderManager colliderManager;

    void Start()
    {
        //colliderManager = new ColliderManager();
        lDoorClosedPos = LDoor.position;
        rDoorClosedPos = RDoor.position;
        if (!openRotated)
        { 
            lDoorOpenPos = lDoorClosedPos + new Vector3(0, 0, -openWidth);
            rDoorOpenPos = rDoorClosedPos + new Vector3(0, 0, openWidth);
        }
        else
        {
            lDoorOpenPos = lDoorClosedPos + new Vector3(openWidth, 0, 0);
            rDoorOpenPos = rDoorClosedPos + new Vector3(-openWidth, 0, 0);
        }
    }


    public override void Interact()
    {
        PlayerInteraction player = FindAnyObjectByType<PlayerInteraction>();
        if (!facialScanNeeded)
        {
            if (needsKey)
            {
                if (player.isHoldingKey)
                {
                    isUnlocked = true;
                    StartCoroutine(OpenNow());
                    Debug.Log("Door " + (isOpen ? "opened." : "closed."));
                }
                else
                {
                    Debug.Log("The door is locked. You need a key to open it.");
                }
            }
            else
            {
                StartCoroutine(OpenNow());
                Debug.Log("Door " + (isOpen ? "opened." : "closed."));
            }
        }
    }

    public IEnumerator OpenNow()
    {
        Debug.Log("Door OpenNow called.");
        if (!isOpen)
        //if (!isOpen && colliderManager.IsFacialScanComplete())
        {
            Debug.Log("Starting door opening animation.");
            isOpen = true;
            lerpProgress = 0f;

            yield return new WaitForSeconds(1f);

            while (lerpProgress < 1f)
            {
                lerpProgress += Time.deltaTime * openSpeed;
                if (!openRotated)
                {
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
                    yield return null;
                }
                else
                {
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
                    yield return null;
                }
            }
        }
    }
}
