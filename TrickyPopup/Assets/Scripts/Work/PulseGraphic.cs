using UnityEngine;
using UnityEngine.UI;

/// <summary>Gently pulses an Image's alpha. Used for the glow around the table you need to paste into.</summary>
[RequireComponent(typeof(Graphic))]
public class PulseGraphic : MonoBehaviour
{
    [SerializeField] float speed = 3f;
    [SerializeField, Range(0f, 1f)] float minAlpha = 0.25f;
    [SerializeField, Range(0f, 1f)] float maxAlpha = 0.85f;

    Graphic g;

    void Awake() { g = GetComponent<Graphic>(); }

    void Update()
    {
        float t = (Mathf.Sin(Time.unscaledTime * speed) + 1f) * 0.5f;
        Color c = g.color;
        c.a = Mathf.Lerp(minAlpha, maxAlpha, t);
        g.color = c;
    }
}
