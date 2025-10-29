using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    public float minIntensity = 0.2f;
    public float maxIntensity = 1.5f;
    public float speed = 25f;          // larger = faster jitter
    public float dropChance = 0.03f;   // chance to “blink” off briefly
    public float dropDuration = 0.05f;

    Light L;
    float baseIntensity;
    float dropTimer;

    void Awake() { L = GetComponent<Light>(); baseIntensity = L.intensity; }

    void Update()
    {
        if (dropTimer > 0f) { dropTimer -= Time.deltaTime; L.intensity = 0f; return; }
        if (Random.value < dropChance * Time.deltaTime * 60f) dropTimer = dropDuration;

        float n = Mathf.PerlinNoise(Time.time * speed, 0.0f);
        L.intensity = Mathf.Lerp(minIntensity, maxIntensity, n);
    }

    void OnEnable()  { if (L == null) L = GetComponent<Light>(); L.intensity = baseIntensity; dropTimer = 0f; }
    void OnDisable() { if (L == null) return; L.intensity = baseIntensity; }
}

