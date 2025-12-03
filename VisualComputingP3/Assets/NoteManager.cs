using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public Transform positionA;
    public Transform positionB;
    public float moveDuration = 3.0f;

    private Vector3 startPos;
    private float elapsedTime = 0f;
    private bool isMoving = false;

    void Start()
    {
        if (positionA == null || positionB == null)
        {
            Debug.LogError("NoteManager: positionA or positionB not assigned!");
            return;
        }

        startPos = transform.position;
        StartMoving();
    }

    void Update()
    {
        if (!isMoving) return;

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / moveDuration);
        transform.position = Vector3.Lerp(startPos, positionB.position, t);

        if (t >= 1f)
        {
            isMoving = false;
            elapsedTime = 0f;
        }
    }

    public void StartMoving()
    {
        isMoving = true;
        elapsedTime = 0f;
        startPos = transform.position;
    }
}
