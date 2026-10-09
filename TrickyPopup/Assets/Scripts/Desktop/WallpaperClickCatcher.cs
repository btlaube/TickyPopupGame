using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>Put on the Wallpaper image. Clicking empty desktop deselects the selected icon.</summary>
public class WallpaperClickCatcher : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData e)
    {
        if (DesktopManager.I != null) DesktopManager.I.ClearSelection();
    }
}
