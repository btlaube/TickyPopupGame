using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bottom bar. Shows one button per open window (including minimized ones).
/// The clock is just a TMP text assigned to GameClock.
/// </summary>
public class Taskbar : MonoBehaviour
{
    [SerializeField] TaskbarButton buttonPrefab;
    [SerializeField] Transform buttonContainer;

    readonly Dictionary<DesktopWindow, TaskbarButton> buttons = new Dictionary<DesktopWindow, TaskbarButton>();

    public void Register(DesktopWindow w)
    {
        if (buttons.ContainsKey(w))
        {
            Refresh();
            return;
        }
        if (buttonPrefab == null || buttonContainer == null)
        {
            Debug.LogWarning("Taskbar: assign 'Button Prefab' and 'Button Container' to get taskbar buttons.", this);
            return;
        }
        var b = Instantiate(buttonPrefab, buttonContainer);
        b.Init(w);
        buttons.Add(w, b);
        Refresh();
    }

    public void Unregister(DesktopWindow w)
    {
        if (buttons.TryGetValue(w, out var b))
        {
            if (b != null) Destroy(b.gameObject);
            buttons.Remove(w);
        }
        Refresh();
    }

    public void Refresh()
    {
        var focused = DesktopManager.I != null ? DesktopManager.I.Focused : null;
        foreach (var kv in buttons)
            if (kv.Value != null) kv.Value.SetState(kv.Key.IsMinimized, kv.Key == focused);
    }
}
