using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// A desktop icon: click to select, double-click to open, drag to move (snaps to the grid).
/// The root object needs an Image (raycast target ON, alpha 0) so it can receive pointer events.
/// </summary>
public class DesktopIcon : MonoBehaviour,
    IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("What happens on double-click")]
    public DesktopWindow windowToOpen;
    public UnityEvent onOpen;

    [Header("Grid")]
    public Vector2Int startCell;

    [Header("Visuals")]
    [SerializeField] Image background;
    [SerializeField] Color normalColor = new Color(1f, 1f, 1f, 0f);
    [SerializeField] Color selectedColor = new Color(0.35f, 0.55f, 1f, 0.35f);
    [SerializeField] float doubleClickTime = 0.35f;

    RectTransform rt;
    DesktopGrid grid;
    Vector2Int cell;
    Vector2 dragOffset;
    float lastClickTime = -10f;
    bool dragging;

    void Start()
    {
        rt = (RectTransform)transform;
        grid = DesktopManager.I.grid;
        Canvas.ForceUpdateCanvases();

        cell = grid.FindNearestFree(grid.ClampCell(startCell), this);
        grid.Place(this, cell);
        rt.localPosition = grid.CellToLocal(cell);
        SetSelected(false);
    }

    void OnDestroy()
    {
        if (grid != null) grid.Remove(this);
    }

    public void SetSelected(bool selected)
    {
        if (background != null) background.color = selected ? selectedColor : normalColor;
    }

    public void Open()
    {
        if (windowToOpen != null) windowToOpen.Open();
        onOpen.Invoke();
    }

    /// <summary>Called by the grid when the screen size changes.</summary>
    public void Reflow()
    {
        if (grid == null || rt == null) return;
        cell = grid.ClampCell(cell);
        rt.localPosition = grid.CellToLocal(cell);
    }

    // ---- Click / double-click ----
    public void OnPointerClick(PointerEventData e)
    {
        if (e.button != PointerEventData.InputButton.Left) return;

        DesktopManager.I.SelectIcon(this);

        float now = Time.unscaledTime;
        if (now - lastClickTime <= doubleClickTime)
        {
            lastClickTime = -10f;
            Open();
        }
        else
        {
            lastClickTime = now;
        }
    }

    // ---- Drag with grid snap ----
    public void OnBeginDrag(PointerEventData e)
    {
        if (e.button != PointerEventData.InputButton.Left) return;
        dragging = true;
        DesktopManager.I.SelectIcon(this);
        rt.SetAsLastSibling();

        var parent = (RectTransform)rt.parent;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, e.position, e.pressEventCamera, out var local))
            dragOffset = (Vector2)rt.localPosition - local;
    }

    public void OnDrag(PointerEventData e)
    {
        if (!dragging) return;
        var parent = (RectTransform)rt.parent;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, e.position, e.pressEventCamera, out var local))
            return;

        Vector2 p = local + dragOffset;
        Rect r = parent.rect;
        p.x = Mathf.Clamp(p.x, r.xMin, r.xMax);
        p.y = Mathf.Clamp(p.y, r.yMin, r.yMax);
        rt.localPosition = new Vector3(p.x, p.y, 0f);
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (!dragging) return;
        dragging = false;

        Vector2Int target = grid.LocalToCell(rt.localPosition);
        cell = grid.FindNearestFree(target, this);
        grid.Place(this, cell);
        rt.localPosition = grid.CellToLocal(cell);
    }
}
