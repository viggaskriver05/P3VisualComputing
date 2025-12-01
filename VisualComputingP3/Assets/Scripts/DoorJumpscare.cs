using UnityEngine;
using System.Collections;
using UnityEngine.Video;

public class DoorJumpscare : MonoBehaviour
{
    public GameObject player;
    public GameObject JumpSPos;
    public IntroManager introManager;
    public VideoPlayer jumpscareVideo;
    public GameObject jumpscareCanvas;

    public IEnumerator Jumpscare()
    {
         float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 1.0f;
            player.transform.position = Vector3.Lerp(player.transform.position, JumpSPos.transform.position, t);

            yield return null;
        }
        StartCoroutine(introManager.EyesBlink());
        jumpscareCanvas.SetActive(true);
        jumpscareVideo.Prepare();
        yield return new WaitUntil(() => jumpscareVideo.isPrepared);
        jumpscareVideo.Play();
        yield return new WaitUntil(() => !jumpscareVideo.isPlaying);

        
        jumpscareCanvas.SetActive(false);
        yield return new WaitForSeconds(1.0f);
    }
}
