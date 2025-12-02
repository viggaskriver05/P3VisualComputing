using UnityEngine;
using UnityEngine.Video;

public class PeepDoor : MonoBehaviour
{
    public GameObject pressEUI;      // The "Press E to peep" text
    public VideoPlayer videoPlayer;  // VideoPlayer on Main Camera
    public GameObject videoCanvas;   // Optional: black overlay canvas

    private bool playerInRange = false;

    void Start()
    {
        // Make sure the hint is hidden at game start
        if (pressEUI != null)
            pressEUI.SetActive(false);

        // Make sure video overlay is hidden at start
        if (videoCanvas != null)
            videoCanvas.SetActive(false);

        // Register callback when video finishes
        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            pressEUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            pressEUI.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            pressEUI.SetActive(false);

            if (videoCanvas != null)
                videoCanvas.SetActive(true);

            videoPlayer.Play();
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        if (videoCanvas != null)
            videoCanvas.SetActive(false);

        videoPlayer.Stop();
    }
}


