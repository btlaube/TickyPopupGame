using UnityEngine;
using UnityEngine.EventSystems;

public class PopupCloseButton : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerUpHandler
{
    [SerializeField] private PopupTrick popupTrick;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (popupTrick != null)
            popupTrick.SetHiddenDownloadIcon(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (popupTrick != null)
            popupTrick.SetHiddenDownloadIcon(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (popupTrick != null)
            popupTrick.OnCloseButtonPointerUp();
    }
}