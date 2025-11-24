using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlickeringLight : MonoBehaviour
{
    [Header("Base Light Settings")]
    public float baseIntensity = 1.5f;          // Normal lysstyrke
    public float flickerIntensityMultiplier = 0.2f; // Hvor mørkt det bliver under et flicker (0 = helt slukket)

    [Header("Flicker Pattern")]
    public float minTimeBetweenBursts = 2f;     // Min tid mellem flicker-bursts
    public float maxTimeBetweenBursts = 6f;     // Max tid mellem flicker-bursts

    public int minFlashesInBurst = 2;          // Minimum antal små blink i et burst
    public int maxFlashesInBurst = 6;          // Maks antal små blink i et burst

    public float minOffTime = 0.03f;           // Hvor kort tid lyset kan være “slukket”
    public float maxOffTime = 0.12f;
    public float minOnTime = 0.03f;            // Hvor kort tid lyset er “på” mellem off-blink
    public float maxOnTime = 0.1f;

    [Header("Optional: Emissive Material")]
    public Renderer emissiveRenderer;          // Fx loftpanelet med lysrør
    public string emissionProperty = "_EmissionColor";
    public Color emissionColor = Color.white;

    private Light _light;
    private float _targetBaseIntensity;

    void Awake()
    {
        _light = GetComponent<Light>();
        _targetBaseIntensity = baseIntensity;

        // Sæt start-intensity
        if (_light != null)
            _light.intensity = baseIntensity;

        // Sørg for at emission er aktiveret, hvis vi har et material
        if (emissiveRenderer != null)
        {
            emissiveRenderer.material.EnableKeyword("_EMISSION");
            emissiveRenderer.material.SetColor(emissionProperty, emissionColor * baseIntensity);
        }
    }

    void OnEnable()
    {
        StartCoroutine(FlickerRoutine());
    }

    IEnumerator FlickerRoutine()
    {
        while (true)
        {
            // Vent mellem bursts
            float waitTime = Random.Range(minTimeBetweenBursts, maxTimeBetweenBursts);
            yield return new WaitForSeconds(waitTime);

            int flashes = Random.Range(minFlashesInBurst, maxFlashesInBurst + 1);

            for (int i = 0; i < flashes; i++)
            {
                // SLUK / svag belysning
                SetLightIntensity(baseIntensity * flickerIntensityMultiplier);
                yield return new WaitForSeconds(Random.Range(minOffTime, maxOffTime));

                // TÆND igen (evtl. lidt varierende for at føles levende)
                float jitter = Random.Range(-0.1f, 0.1f);
                SetLightIntensity(Mathf.Max(0f, baseIntensity + jitter));
                yield return new WaitForSeconds(Random.Range(minOnTime, maxOnTime));
            }

            // Til sidst tilbage til normal styrke
            SetLightIntensity(baseIntensity);
        }
    }

    private void SetLightIntensity(float intensity)
    {
        if (_light != null)
            _light.intensity = intensity;

        if (emissiveRenderer != null)
        {
            emissiveRenderer.material.SetColor(emissionProperty, emissionColor * intensity);
        }
    }
}
