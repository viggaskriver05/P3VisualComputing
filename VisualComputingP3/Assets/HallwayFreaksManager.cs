using UnityEngine;

public class HallwayFreaksManager : MonoBehaviour
{
    private bool hasTriggered = false;

    // Assign the GameObjects to activate for each P tag in the Inspector
    public GameObject p1Object;
    public GameObject p2Object;
    public GameObject p3Object;
    public GameObject p4Object;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        // Only react when the player (tag "Player") enters this trigger
        if (!other.CompareTag("Player")) return;
        if (hasTriggered) return;

        hasTriggered = true; // ensure this trigger only fires once

        switch (gameObject.tag)
        {
            case "P1":
                if (p1Object != null) p1Object.SetActive(true);
                HandleP1(other.gameObject);
                break;
            case "P2":
                if (p2Object != null) p2Object.SetActive(true);
                HandleP2(other.gameObject);
                break;
            case "P3":
                if (p3Object != null) p3Object.SetActive(true);
                HandleP3(other.gameObject);
                break;
            case "P4":
                if (p4Object != null) p4Object.SetActive(true);
                HandleP4(other.gameObject);
                break;
            default:
                break;
        }
    }

    void HandleP1(GameObject Player)
    {
        Debug.Log("Player entered P1 trigger on: " + gameObject.name);
    }

    void HandleP2(GameObject Player)
    {
        Debug.Log("Player entered P2 trigger on: " + gameObject.name);
    }

    void HandleP3(GameObject Player)
    {
        Debug.Log("Player entered P3 trigger on: " + gameObject.name);
    }

    void HandleP4(GameObject Player)
    {
        Debug.Log("Player entered P4 trigger on: " + gameObject.name);
    }
}
