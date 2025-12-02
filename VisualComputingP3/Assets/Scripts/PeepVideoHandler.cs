using UnityEngine;
using UnityEngine.Video;

public class PeepVideoHandler : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // Stop the video once it reaches the end
        videoPlayer.Stop();
    }
}
