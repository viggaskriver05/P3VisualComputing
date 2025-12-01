using UnityEngine;

public class AIRingVisualizer : MonoBehaviour
{
    [Header("Ring Reference")]
    public Transform ring;         

    [Header("Pulse Settings")]
    public float pulseAmplitude = 0.15f; 
    public float pulseSpeed = 6f;       

    [Header("Smoothing")]
    public float returnSpeed = 10f;      

    bool isSpeaking;
    Vector3 baseScale;
    float t;

    void Awake()
    {
        if (ring == null)
            ring = transform; 

        baseScale = ring.localScale;
    }

    void Update()
    {
        if (isSpeaking)
        {
            t += Time.deltaTime * pulseSpeed;
            float factor = 1f + Mathf.Sin(t) * pulseAmplitude;
            ring.localScale = baseScale * factor;
        }
        else
        {
            t = 0f;
            ring.localScale = Vector3.Lerp(
                ring.localScale,
                baseScale,
                Time.deltaTime * returnSpeed
            );
        }
    }

    // Kald denne fra andre scripts
    public void SetSpeaking(bool speaking)
    {
        isSpeaking = speaking;
    }
}
