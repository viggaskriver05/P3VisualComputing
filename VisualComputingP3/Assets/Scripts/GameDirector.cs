using UnityEngine;

public class GameDirector : MonoBehaviour
{
    //  STATE 
    public enum GameState { Intro, FacialScan, Pills, DoorScan, Crack1, Done }
    public GameState state = GameState.Intro;

    // FLAGS 
    bool didFacialScan = false;
    bool tookPills = false;
    bool doorOpened = false;
    bool crack1Triggered = false;

    void Start()
    {
        Debug.Log("Game start — welcome to the Rehabilitation Center.");
        state = GameState.Intro;
    }

    void Update()
    {
        //  INTRO 
        if (state == GameState.Intro)
        {
            state = GameState.FacialScan;
            Debug.Log("Please perform your daily facial scan.");
        }
        //  FACIAL SCAN 
        else if (state == GameState.FacialScan && didFacialScan)
        {
            Debug.Log("Facial scan complete. Noted.");
            state = GameState.Pills;
            Debug.Log("Please take your daily pills.");
        }
        //  PILLS 
        else if (state == GameState.Pills && tookPills)
        {
            Debug.Log("Pills taken. Proceed to the door scanner.");
            state = GameState.DoorScan;
        }
        //  DOOR SCAN 
        else if (state == GameState.DoorScan && doorOpened)
        {
            Debug.Log("Door opened. Proceed...");
            state = GameState.Crack1;
        }
        //  CRACK 1 
        else if (state == GameState.Crack1 && crack1Triggered)
        {
            Debug.Log("Whisper… Lights dim… Level complete!");
            state = GameState.Done;
        }
        //  DONE 
        else if (state == GameState.Done)
        {
            // ready for next level / scene change
        }
    }

    //  PUBLIC HOOKS 

    // Facial scanner result (e.g., "smile"/"frown")
    public void OnFacialScan(string result)
    {
        if (state != GameState.FacialScan) return;
        didFacialScan = true;
        Debug.Log("AI: Noted. (" + result + ")");
    }

    // Pills taken (used by PickupItem)
    public void OnPillsTaken()
    {
        if (state != GameState.Pills) return;
        tookPills = true;
        Debug.Log("AI: Pills taken.");
    }

    // Convenience alias so your existing PickupItem can call TakePills()
    public void TakePills()
    {
        OnPillsTaken();
    }

    // Door accepted / opened (used by Door.cs)
    public void OnDoorScanAccepted()
    {
        if (state != GameState.DoorScan) return;
        doorOpened = true;
        Debug.Log("AI: Access granted.");
    }

    // CRACK 1 trigger (call from a trigger zone / timeline cue)
    public void TriggerCrack1()
    {
        if (state != GameState.Crack1) return;
        crack1Triggered = true;
        Debug.Log("AI: Apologies. Minor interference. Increasing intensity...");
    }
}




