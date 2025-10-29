using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    public float minIntensity = 0.8f;   // raised from 0.2 → smoother baseline
    public float maxIntensity = 1.2f;   // lowered from 1.5 → less dramatic
    public float speed = 5f;            // slower flicker movement
    public float dropChance = 0.005f;   // rarer full “blink”
    public float dropDuration = 0.05f;

    Light L;
    float baseIntensity;
    float dropTimer;

    void Awake()
    {
        L = GetComponent<Light>();
        baseIntensity = L.intensity;
    }

    void Update()
    {
        if (dropTimer > 0f)
        {
            dropTimer -= Time.deltaTime;
            L.intensity = 0f;
            return;
        }

        if (Random.value < dropChance * Time.deltaTime * 60f)
            dropTimer = dropDuration;

        float n = Mathf.PerlinNoise(Time.time * speed, 0.0f);
        L.intensity = Mathf.Lerp(minIntensity, maxIntensity, n);
    }

    void OnEnable()
    {
        if (L == null) L = GetComponent<Light>();
        L.intensity = baseIntensity;
        dropTimer = 0f;
    }

    void OnDisable()
    {
        if (L == null) return;
        L.intensity = baseIntensity;
    }
}

