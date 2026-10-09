using UnityEngine;

public class CursorFollower : MonoBehaviour
{
    [SerializeField] RectTransform canvasRect;
    [SerializeField] Camera cam;
    RectTransform rt;

    void Awake()
    {
        rt = (RectTransform)transform;

        // Hides the hardware cursor
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, MouseTracker.ScreenPos, cam, out var local);
        rt.anchoredPosition = local;
    }
}