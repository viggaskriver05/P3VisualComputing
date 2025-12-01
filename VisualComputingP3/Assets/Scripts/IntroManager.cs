using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class IntroManager : MonoBehaviour
{
    public VideoPlayer introVid;
    public CanvasGroup fadeCanvas;
    public Transform playerTransform;
    public GameObject VideoCanvas;
    public GameObject PointerCanvas;
    public PlayerMove movementScript;

    public float fadeDuration = 2.0f;
    public float flutterSpeed = 0.5f;
    public float sitUpSpeed = 1.0f;
    public Vector3 sitUpPosition = new Vector3(0, 1, 0);
    private Vector3 startingPosition;
    private bool skipIntro = false;

    void Start()
    {
        movementScript.enabled = false;
        fadeCanvas.alpha = 0f;
        startingPosition = playerTransform.position;
        StartCoroutine(PlayIntroVideo());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && !skipIntro)
        {
            skipIntro = true;

            //SoundManager.SpeechSource.Stop();
            SoundManager.Instance.Stop("oai_pre_video_sp");
            StartCoroutine(EyesFlutter());
            StartCoroutine(SitUp());
            SoundManager.Instance.PlaySpeechSFX("oai_welcome_sp");
            PointerCanvas.SetActive(true);
        }
    }


    IEnumerator PlayIntroVideo()
    {
        introVid.Prepare();
        yield return new WaitUntil(() => introVid.isPrepared);

        introVid.Play();
        SoundManager.Instance.PlaySpeechSFX("oai_pre_video_sp");

        yield return new WaitUntil(() => !introVid.isPlaying);
        if (skipIntro) yield break;
        yield return StartCoroutine(EyesFlutter());
        yield return StartCoroutine(SitUp());
        SoundManager.Instance.PlaySpeechSFX("oai_welcome_sp");
        PointerCanvas.SetActive(true);

    }

    IEnumerator FadeToBlack(float from, float to, float duration)
    {
        float t = 0f;
        fadeCanvas.alpha = from;

        while (t < duration)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        fadeCanvas.alpha = to;
    }

    public IEnumerator EyesFlutter()
    {
        VideoCanvas.SetActive(false);
        yield return StartCoroutine(FadeToBlack(1f, 0.5f, flutterSpeed));
        yield return StartCoroutine(FadeToBlack(0.5f, 1f, flutterSpeed));
        yield return StartCoroutine(FadeToBlack(1f, 0.2f, flutterSpeed));
        yield return StartCoroutine(FadeToBlack(0.2f, 1f, flutterSpeed));
        yield return StartCoroutine(FadeToBlack(1f, 0.2f, flutterSpeed));

    }
    IEnumerator SitUp()
    {
        float t = 0f;

        Vector3 start = startingPosition;
        Vector3 end = sitUpPosition;

        while (t < sitUpSpeed)
        {
            t += Time.deltaTime;
            playerTransform.position = Vector3.Lerp(start, end, t / sitUpSpeed);
            yield return null;
        }

        playerTransform.position = end;
        movementScript.enabled = true;
    }
}
