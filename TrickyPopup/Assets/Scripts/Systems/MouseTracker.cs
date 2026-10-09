using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class MouseTracker : MonoBehaviour
{
    public static Vector2 ScreenPos { get; private set; }
    public static Vector2 Delta { get; private set; }
    public static bool Down { get; private set; }
    public static bool Held { get; private set; }
    public static bool Up { get; private set; }
    public static float IdleSeconds => Time.unscaledTime - lastActive;
    static float lastActive;

    public static void ResetIdle() => lastActive = Time.unscaledTime;

    public static Vector2 World(Camera cam)
    {
        var p = new Vector3(ScreenPos.x, ScreenPos.y, Mathf.Abs(cam.transform.position.z));
        return cam.ScreenToWorldPoint(p);
    }

    void Update()
    {
        var m = Mouse.current;
        if (m == null) return;
        Vector2 p = m.position.ReadValue();
        Delta = p - ScreenPos;
        ScreenPos = p;
        Down = m.leftButton.wasPressedThisFrame;
        Held = m.leftButton.isPressed;
        Up = m.leftButton.wasReleasedThisFrame;
        if (Delta.sqrMagnitude > 1f || Down || Up) lastActive = Time.unscaledTime;
    }
}