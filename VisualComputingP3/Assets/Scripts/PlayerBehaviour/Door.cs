using UnityEngine;
using System.Collections;
using static UnityEngine.Object;

public class Door : InteractionScript
{

    public bool isUnlocked = false;
    public bool needsKey = true;
    public Transform LDoor;
    public Transform RDoor;
    public float openWidth;
    public float openSpeed = 0.5f;
    public bool scannerUnlocked = false;
    public bool keyAndScanNeeded = true;

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
        lDoorOpenPos = lDoorClosedPos + new Vector3(0, 0, -openWidth);
        rDoorOpenPos = rDoorClosedPos + new Vector3(0, 0, openWidth);
    }

    public void unlockWithScanner()
    {
        scannerUnlocked = true;
        TryAutoOpen();
    }

    private void TryAutoOpen()
    {
        if (!keyAndScanNeeded && scannerUnlocked)
        {
            StartCoroutine(OpenNow());
        }
        else if (keyAndScanNeeded && scannerUnlocked && isUnlocked)
        {
            StartCoroutine(OpenNow());
        }
    }

    void Update()
    {

    }

    public override void Interact()
    {
        PlayerInteraction player = FindAnyObjectByType<PlayerInteraction>();

        if (player.isHoldingKey)
        {
            isUnlocked = true;
            TryAutoOpen();
            Debug.Log("Door " + (isOpen ? "opened." : "closed."));
        }
        else
        {
            Debug.Log("The door is locked. You need a key to open it.");
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
