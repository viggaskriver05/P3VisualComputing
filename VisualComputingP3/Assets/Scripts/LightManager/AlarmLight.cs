using UnityEngine;

[RequireComponent(typeof(Light))]
public class AlarmLight : MonoBehaviour
{
    [Header("Pulse Settings")]
    public float pulseSpeed = 2f;      // Hvor hurtigt lyset pulserer
    public float minIntensity = 0f;    // Laveste lysniveau
    public float maxIntensity = 5f;    // Højeste lysniveau
    [SerializeField] EmotionOscListener listener;


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
        listener = UnityEngine.Object.FindFirstObjectByType<EmotionOscListener>();
    }

    void Update()
    {
        _time += Time.deltaTime;

        var dominant = listener != null ? listener.DominantEmotion : "unknown";
        var sentiment = listener != null ? listener.SentimentEmotion : "neutral";
        // Juster pulseSpeed og farve baseret på emotion
        if (listener != null)
        {
            if (sentiment == "negative") 
            {
                alarmColor = Color.Lerp(new Color(0.6f, 0f, 0f), new Color(1f, 0f, 2f), 0.2f);
                pulseSpeed = 6f;
                _light.range = 10f;
            }
            else if (sentiment == "positive") 
            {
                alarmColor = Color.Lerp(new Color(0.6f, 0f, 0f), new Color(2f, 2f, 0f), 0.2f);
                pulseSpeed = 2f;
                _light.range = 2f;
            }
            else 
            {
                alarmColor = new Color(0.6f, 0f, 0f); 
                pulseSpeed = 4f;
                _light.range = 6f;

            }
            _light.color = alarmColor;
        }

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