using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// "Hover to highlight, click to act." Put on a table (it needs a raycast-target Image).
/// While hovered, an overlay with a big label (COPY / PASTE) is shown in the middle.
/// Click runs onActivate. The overlay and its label must have Raycast Target OFF.
/// </summary>
public class HoverAction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] GameObject overlay;
    [SerializeField] TMP_Text overlayLabel;
    [SerializeField] string actionLabel = "COPY";
    [Tooltip("Shown instead when the action isn't available yet. Leave empty to show nothing.")]
    [SerializeField] string blockedLabel = "";
    public UnityEvent onActivate;

    public bool Interactable { get; set; } = true;
    /// <summary>Optional gate set from code. If it returns false the click does nothing.</summary>
    public Func<bool> CanActivate { get; set; }

    bool hovering;
    string tempText;
    float tempUntil;

    void OnEnable()
    {
        hovering = false;
        Refresh();
    }

    void OnDisable() { hovering = false; }

    public void OnPointerEnter(PointerEventData e) { hovering = true; Refresh(); }
    public void OnPointerExit(PointerEventData e) { hovering = false; Refresh(); }

    public void OnPointerClick(PointerEventData e)
    {
        if (e.button != PointerEventData.InputButton.Left) return;
        if (!Interactable || !IsReady()) return;
        onActivate.Invoke();
        Refresh();
    }

    /// <summary>Show a message in the overlay for a moment (e.g. "COPIED!").</summary>
    public void Flash(string text, float seconds)
    {
        tempText = text;
        tempUntil = Time.unscaledTime + seconds;
        Refresh();
        if (isActiveAndEnabled) StartCoroutine(RefreshLater(seconds));
    }

    IEnumerator RefreshLater(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Refresh();
    }

    public void Refresh()
    {
        bool ready = IsReady();
        bool flashing = Time.unscaledTime < tempUntil;
        bool show = hovering && Interactable && (ready || flashing || !string.IsNullOrEmpty(blockedLabel));

        if (overlay != null) overlay.SetActive(show);
        if (show && overlayLabel != null)
            overlayLabel.text = flashing ? tempText : (ready ? actionLabel : blockedLabel);
    }

    bool IsReady() => CanActivate == null || CanActivate();
}
