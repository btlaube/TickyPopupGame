using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Central desktop hub: knows the layers, the taskbar, which window has focus,
/// and which icon is selected. Brings a window to the front when you click anywhere on it.
/// </summary>
public class DesktopManager : MonoBehaviour
{
    public static DesktopManager I { get; private set; }

    [Header("Scene references")]
    public DesktopGrid grid;
    public RectTransform windowLayer;
    public RectTransform dragLayer;
    public Taskbar taskbar;

    public DesktopWindow Focused { get; private set; }
    public Camera UICamera { get; private set; }
    /// <summary>The root canvas rect. Windows are kept inside this, whatever size WindowLayer is.</summary>
    public RectTransform CanvasRect { get; private set; }

    public float TaskbarHeight =>
        taskbar != null ? ((RectTransform)taskbar.transform).rect.height : 0f;

    DesktopIcon selectedIcon;

    void Awake()
    {
        I = this;
        if (windowLayer == null)
        {
            Debug.LogError("DesktopManager: Window Layer is not assigned.", this);
            return;
        }
        var canvas = windowLayer.GetComponentInParent<Canvas>();
        if (canvas != null) canvas = canvas.rootCanvas;
        UICamera = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            ? canvas.worldCamera : null;
        CanvasRect = canvas != null ? (RectTransform)canvas.transform : null;
    }

    void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null || windowLayer == null || !mouse.leftButton.wasPressedThisFrame) return;

        // Focus the top-most open window under the pointer. (Done here rather than with
        // pointer events because child buttons would swallow the click.)
        Vector2 p = mouse.position.ReadValue();
        for (int i = windowLayer.childCount - 1; i >= 0; i--)
        {
            var child = windowLayer.GetChild(i);
            if (!child.gameObject.activeInHierarchy) continue;
            var w = child.GetComponent<DesktopWindow>();
            if (w == null) continue;
            if (RectTransformUtility.RectangleContainsScreenPoint((RectTransform)child, p, UICamera))
            {
                w.Focus();
                return;
            }
        }
    }

    // ---- Icon selection ----
    public void SelectIcon(DesktopIcon icon)
    {
        if (selectedIcon == icon) return;
        if (selectedIcon != null) selectedIcon.SetSelected(false);
        selectedIcon = icon;
        if (selectedIcon != null) selectedIcon.SetSelected(true);
    }

    public void ClearSelection() => SelectIcon(null);

    // ---- Window focus ----
    public void SetFocused(DesktopWindow w)
    {
        Focused = w;
        RefreshFocusVisuals();
    }

    /// <summary>Called when a window closes or minimizes: focus the next window down.</summary>
    public void OnWindowStateChanged()
    {
        Focused = null;
        for (int i = windowLayer.childCount - 1; i >= 0; i--)
        {
            var w = windowLayer.GetChild(i).GetComponent<DesktopWindow>();
            if (w != null && w.IsOpen && !w.IsMinimized)
            {
                Focused = w;
                break;
            }
        }
        RefreshFocusVisuals();
    }

    void RefreshFocusVisuals()
    {
        for (int i = 0; i < windowLayer.childCount; i++)
        {
            var w = windowLayer.GetChild(i).GetComponent<DesktopWindow>();
            if (w != null) w.SetFocusVisual(w == Focused);
        }
        if (taskbar != null) taskbar.Refresh();
    }
}
