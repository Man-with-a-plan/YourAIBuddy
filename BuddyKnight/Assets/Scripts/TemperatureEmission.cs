using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class TemperatureEmission : MonoBehaviour
{
    [Header("Timing")]
    [Tooltip("Seconds for a full cold->hot (or hot->cold) change.")]
    public float stateChangeDuration = 5f;

    [Tooltip("Continuously heat up and cool down on its own.")]
    public bool autoOscillate = true;
    
    [Header("Emission")]
    [ColorUsage(false, true)] public Color coldEmission = Color.black;
    [ColorUsage(false, true)] public Color hotEmission = new Color(1f, 0.25f, 0.05f);
    public float minIntensity = 0f;
    public float maxIntensity = 4f;   // >1 for HDR/bloom punch
    public bool isHot;
    [Range(0f, 1f)] public float temperature = 0f;   // 0 cold, 1 hot

    private float _target = 1f;
    private Material _material;
    private static readonly int EmissionId = Shader.PropertyToID("_EmissionColor");

    void Awake()
    {
        isHot = false;
        _material = GetComponent<Renderer>().material; // auto-creates an instance
        _material.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        if (autoOscillate)
        {
            // Rate that covers the full 0<->1 range in stateChangeDuration seconds.
            float step = (stateChangeDuration > 0f ? 1f / stateChangeDuration : 1f) * Time.deltaTime;
            temperature = Mathf.MoveTowards(temperature, _target, step);
            if (Mathf.Approximately(temperature, _target))
            {
                isHot = !isHot;   // flip the state
                _target = 1f - _target;   // flip the goal -> ping-pong
            }
            
        }

        Color baseColor = Color.Lerp(coldEmission, hotEmission, temperature);
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, temperature);
        _material.SetColor(EmissionId, baseColor * intensity);
    }

    // Call from other scripts to drive it externally instead of oscillating.
    public void SetTemperature(float t) => temperature = Mathf.Clamp01(t);
}