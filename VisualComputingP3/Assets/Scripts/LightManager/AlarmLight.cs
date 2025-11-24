using UnityEngine;

[RequireComponent(typeof(Light))]
public class AlarmLight : MonoBehaviour
{
    [Header("Pulse Settings")]
    public float pulseSpeed = 2f;      // Hvor hurtigt lyset pulserer
    public float minIntensity = 0f;    // Laveste lysniveau
    public float maxIntensity = 5f;    // Højeste lysniveau

    [Header("Color Settings")]
    public Color alarmColor = Color.red;

    [Header("Optional: Emissive Mesh")]
    public Renderer emissiveRenderer;
    public string emissionProperty = "_EmissionColor";

    private Light _light;
    private float _time;

    void Awake()
    {
        _light = GetComponent<Light>();
        _light.color = alarmColor;

        if (emissiveRenderer != null)
        {
            emissiveRenderer.material.EnableKeyword("_EMISSION");
        }
    }

    void Update()
    {
        _time += Time.deltaTime;

        // Sinuskurve → pulserende effekt
        float t = (Mathf.Sin(_time * pulseSpeed) + 1f) / 2f;
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, t);

        // Sæt lysets intensity
        _light.intensity = intensity;

        // Synkroniser emissive mesh
        if (emissiveRenderer != null)
        {
            emissiveRenderer.material.SetColor(emissionProperty, alarmColor * intensity);
        }
    }
}
