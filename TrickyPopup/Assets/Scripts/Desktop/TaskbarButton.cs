using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>One taskbar entry. Click: restore if minimized, minimize if focused, else focus.</summary>
[RequireComponent(typeof(Button))]
public class TaskbarButton : MonoBehaviour
{
    [SerializeField] TMP_Text label;
    [SerializeField] Image icon;
    [SerializeField] Image background;
    [SerializeField] Color focusedColor = new Color(0.35f, 0.50f, 0.85f, 1f);
    [SerializeField] Color normalColor = new Color(0.28f, 0.28f, 0.34f, 1f);
    [SerializeField] Color minimizedColor = new Color(0.18f, 0.18f, 0.22f, 1f);

    DesktopWindow window;

    public void Init(DesktopWindow w)
    {
        window = w;
        if (label != null) label.text = w.title;
        if (icon != null)
        {
            icon.sprite = w.taskbarIcon;
            icon.enabled = w.taskbarIcon != null;
        }
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (window == null) return;
        if (window.IsMinimized) window.Restore();
        else if (DesktopManager.I.Focused == window) window.Minimize();
        else window.Focus();
    }

    public void SetState(bool minimized, bool focused)
    {
        if (background == null) return;
        background.color = focused ? focusedColor : (minimized ? minimizedColor : normalColor);
    }
}
