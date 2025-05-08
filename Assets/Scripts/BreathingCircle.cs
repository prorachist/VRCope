using UnityEngine;
using UnityEngine.UI;      // for RectTransform (Image still comes from UI)
using TMPro;               // for TMP_Text

public class BreathingCircle : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The inner circle Image whose scale we animate")]
    public RectTransform innerCircle;
    [Tooltip("TextMeshProUGUI that displays 'Inhale' or 'Exhale'")]
    public TMP_Text breathingText;

    [Header("Timings (seconds)")]
    public float inhaleDuration = 4f;
    public float exhaleDuration = 4f;

    [Header("Scale Range")]
    [Tooltip("Inner circle scale at end of exhale")]
    public float minScale = 0.3f;
    [Tooltip("Inner circle scale at end of inhale")]
    public float maxScale = 1.0f;

    private float cycleTime;
    private float timer;

    void Start()
    {
        cycleTime = inhaleDuration + exhaleDuration;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = timer % cycleTime;

        if (t < inhaleDuration)
        {
            // Inhale phase
            float p = t / inhaleDuration;
            float s = Mathf.Lerp(minScale, maxScale, p);
            innerCircle.localScale = Vector3.one * s;
            breathingText.text = "Inhale";
        }
        else
        {
            // Exhale phase
            float p = (t - inhaleDuration) / exhaleDuration;
            float s = Mathf.Lerp(maxScale, minScale, p);
            innerCircle.localScale = Vector3.one * s;
            breathingText.text = "Exhale";
        }
    }
}
