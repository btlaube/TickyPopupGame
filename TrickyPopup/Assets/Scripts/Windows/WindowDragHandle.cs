using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Drags the whole window by its header and keeps it on-screen (above the taskbar).
/// Added automatically by DesktopWindow if the header doesn't already have one.
/// The on-screen limit uses the root canvas, so it works however WindowLayer is sized.
/// </summary>
public class WindowDragHandle : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField] DesktopWindow window;

    Vector2 grabOffset;
    readonly Vector3[] corners = new Vector3[4];

    void Awake()
    {
        if (window == null) window = GetComponentInParent<DesktopWindow>();
    }

    RectTransform ParentRect => (RectTransform)window.transform.parent;

    public void OnBeginDrag(PointerEventData e)
    {
        if (window == null) return;
        window.Focus();
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(ParentRect, e.position, e.pressEventCamera, out var local))
            grabOffset = (Vector2)window.Rect.localPosition - local;
    }

    public void OnDrag(PointerEventData e)
    {
        if (window == null) return;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(ParentRect, e.position, e.pressEventCamera, out var local))
            return;

        Vector3 lp = window.Rect.localPosition;
        window.Rect.localPosition = new Vector3(local.x + grabOffset.x, local.y + grabOffset.y, lp.z);
        KeepOnScreen();
    }

    void KeepOnScreen()
    {
        var dm = DesktopManager.I;
        if (dm == null || dm.CanvasRect == null) return;
        RectTransform canvasRect = dm.CanvasRect;

        window.Rect.GetWorldCorners(corners);
        Vector2 bl = canvasRect.InverseTransformPoint(corners[0]);
        Vector2 tr = canvasRect.InverseTransformPoint(corners[2]);
        Rect area = canvasRect.rect;
        float minY = area.yMin + dm.TaskbarHeight;

        Vector2 shift = Vector2.zero;

        if (tr.x - bl.x >= area.width) shift.x = area.xMin - bl.x;
        else if (bl.x < area.xMin) shift.x = area.xMin - bl.x;
        else if (tr.x > area.xMax) shift.x = area.xMax - tr.x;

        if (bl.y < minY) shift.y = minY - bl.y;
        else if (tr.y > area.yMax) shift.y = area.yMax - tr.y;
        if (tr.y + shift.y > area.yMax) shift.y = area.yMax - tr.y;   // header must stay reachable

        if (shift == Vector2.zero) return;
        window.Rect.position += canvasRect.TransformVector(new Vector3(shift.x, shift.y, 0f));
    }
}
