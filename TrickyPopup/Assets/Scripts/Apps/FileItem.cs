using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// A file inside a window (e.g. the shared folder). Double-click opens a window,
/// click-and-drag carries a ghost copy of the file that can be dropped on a SubmissionTray.
/// </summary>
public class FileItem : MonoBehaviour,
    IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string fileId = "quarterly_report";
    public string displayName = "Q3_Quarterly_Report.docx";
    public TMP_Text nameText;

    [Tooltip("Turn off to stop the file being dragged (e.g. until the report has been saved).")]
    public bool draggable = true;

    [Header("Double-click")]
    public DesktopWindow windowToOpen;
    public UnityEvent onOpen;
    [SerializeField] float doubleClickTime = 0.35f;

    RectTransform ghost;
    float lastClickTime = -10f;

    void Awake()
    {
        if (nameText != null) nameText.text = displayName;
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (e.button != PointerEventData.InputButton.Left) return;

        float now = Time.unscaledTime;
        if (now - lastClickTime <= doubleClickTime)
        {
            lastClickTime = -10f;
            if (windowToOpen != null) windowToOpen.Open();
            onOpen.Invoke();
        }
        else
        {
            lastClickTime = now;
        }
    }

    public void OnBeginDrag(PointerEventData e)
    {
        if (!draggable || e.button != PointerEventData.InputButton.Left) return;

        // Clone this item onto the drag layer so it floats above every window.
        var go = Instantiate(gameObject, DesktopManager.I.dragLayer);
        Destroy(go.GetComponent<FileItem>());

        if (!go.TryGetComponent(out CanvasGroup cg)) cg = go.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;   // so the drop lands on whatever is UNDER the ghost
        cg.alpha = 0.85f;

        ghost = (RectTransform)go.transform;
        ghost.anchorMin = ghost.anchorMax = ghost.pivot = new Vector2(0.5f, 0.5f);
        ghost.sizeDelta = ((RectTransform)transform).rect.size;
        ghost.localScale = Vector3.one;
        MoveGhost(e);
    }

    public void OnDrag(PointerEventData e)
    {
        if (ghost != null) MoveGhost(e);
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (ghost == null) return;
        Destroy(ghost.gameObject);
        ghost = null;
    }

    void MoveGhost(PointerEventData e)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                DesktopManager.I.dragLayer, e.position, e.pressEventCamera, out var local))
            ghost.localPosition = new Vector3(local.x, local.y, 0f);
    }
}
