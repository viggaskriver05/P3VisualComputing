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

    public GameObject objectToDeactivate;
    public float deactivateDelay = 35f;

    public float fadeDuration = 2.0f;
    public float flutterSpeed = 0.5f;
    public float sitUpSpeed = 1.0f;
    public Vector3 sitUpPosition = new Vector3(0, 1, 0);
    private Vector3 startingPosition;
    private Quaternion startingRotation;
    private bool skipIntro = false;
    private Coroutine deactivateCoroutine;

    void Start()
    {
        movementScript.enabled = false;
        fadeCanvas.alpha = 0f;
        startingPosition = playerTransform.position;
        startingRotation = playerTransform.rotation;
        StartCoroutine(PlayIntroVideo());
        deactivateCoroutine = StartCoroutine(DeactivateAfterDelay(objectToDeactivate, deactivateDelay));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !skipIntro)
        {
            skipIntro = true;
            
            // Stop the deactivate coroutine and deactivate immediately
            if (deactivateCoroutine != null) StopCoroutine(deactivateCoroutine);
            if (objectToDeactivate != null) objectToDeactivate.SetActive(false);

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
        yield return new WaitForSeconds(2f);
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

    public IEnumerator EyesBlink()
    {
        yield return StartCoroutine(FadeToBlack(0.1f, 1f, 1f));
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(FadeToBlack(1f, 0f, 1f));
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
            playerTransform.rotation = Quaternion.Slerp(startingRotation, Quaternion.Euler(0, 180, 0), t / sitUpSpeed);

            yield return null;
        }

        playerTransform.position = end;
        movementScript.enabled = true;
    }

    IEnumerator DeactivateAfterDelay(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (target != null) target.SetActive(false);
    }
}
