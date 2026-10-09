using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Invisible grid for desktop icons. Put this on IconLayer (a full-screen RectTransform).
/// Cell (0,0) is the top-left; columns go right, rows go down.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class DesktopGrid : MonoBehaviour
{
    public Vector2 cellSize = new Vector2(100f, 110f);
    public Vector2 padding = new Vector2(16f, 16f);
    [Tooltip("Pixels kept free at the bottom of the screen for the taskbar.")]
    public float bottomReserve = 40f;

    readonly Dictionary<DesktopIcon, Vector2Int> occupied = new Dictionary<DesktopIcon, Vector2Int>();
    RectTransform rt;

    RectTransform Rt
    {
        get
        {
            if (rt == null) rt = (RectTransform)transform;
            return rt;
        }
    }

    public int Columns => Mathf.Max(1, Mathf.FloorToInt((Rt.rect.width - padding.x * 2f) / cellSize.x));
    public int Rows => Mathf.Max(1, Mathf.FloorToInt((Rt.rect.height - padding.y * 2f - bottomReserve) / cellSize.y));

    public Vector3 CellToLocal(Vector2Int cell)
    {
        Rect r = Rt.rect;
        float x = r.xMin + padding.x + cellSize.x * (cell.x + 0.5f);
        float y = r.yMax - padding.y - cellSize.y * (cell.y + 0.5f);
        return new Vector3(x, y, 0f);
    }

    public Vector2Int LocalToCell(Vector3 local)
    {
        Rect r = Rt.rect;
        int cx = Mathf.FloorToInt((local.x - r.xMin - padding.x) / cellSize.x);
        int cy = Mathf.FloorToInt((r.yMax - padding.y - local.y) / cellSize.y);
        return ClampCell(new Vector2Int(cx, cy));
    }

    public Vector2Int ClampCell(Vector2Int c)
    {
        return new Vector2Int(Mathf.Clamp(c.x, 0, Columns - 1), Mathf.Clamp(c.y, 0, Rows - 1));
    }

    public bool IsFree(Vector2Int cell, DesktopIcon ignore)
    {
        foreach (var kv in occupied)
            if (kv.Key != ignore && kv.Value == cell) return false;
        return true;
    }

    public Vector2Int FindNearestFree(Vector2Int target, DesktopIcon ignore)
    {
        if (IsFree(target, ignore)) return target;

        Vector2Int best = target;
        float bestDist = float.MaxValue;
        for (int x = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++)
            {
                var c = new Vector2Int(x, y);
                if (!IsFree(c, ignore)) continue;
                float d = (c - target).sqrMagnitude;
                if (d < bestDist)
                {
                    bestDist = d;
                    best = c;
                }
            }
        }
        return best;
    }

    public void Place(DesktopIcon icon, Vector2Int cell) { occupied[icon] = cell; }
    public void Remove(DesktopIcon icon) { occupied.Remove(icon); }

    // Keeps icons glued to the top-left if the screen size changes (e.g. WebGL fullscreen).
    void OnRectTransformDimensionsChange()
    {
        if (occupied.Count == 0) return;
        foreach (var icon in new List<DesktopIcon>(occupied.Keys))
            icon.Reflow();
    }
}
