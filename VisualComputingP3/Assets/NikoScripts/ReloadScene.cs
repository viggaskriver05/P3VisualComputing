using UnityEngine;
using UnityEngine.SceneManagement; // Add this line

public class ReloadScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // This method is called when the collider enters a trigger
    private void OnTriggerEnter(Collider other)
    {
        // Reload when the player (or object with tag "boxRE") enters the trigger
        if (other.CompareTag("Player") || other.CompareTag("boxRE"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
