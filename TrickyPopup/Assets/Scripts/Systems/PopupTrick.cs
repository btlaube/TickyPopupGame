using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopupTrick : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button downloadButton;

    [Header("Icons")]
    [SerializeField] private Image hiddenDownloadIcon;
    [SerializeField] private Image downloadingIcon;

    [Header("Timing")]
    [SerializeField] private float hiddenDownloadSeconds = 4.5f;
    [SerializeField] private float closeDelay = 0.5f;

    [SerializeField] private DesktopWindow desktopWindow;
    private bool downloadStarted;
    private Coroutine iconCoroutine;

    private void Awake()
    {
        // desktopWindow = GetComponent<DesktopWindow>();

        if (hiddenDownloadIcon != null)
            hiddenDownloadIcon.gameObject.SetActive(false);

        if (downloadingIcon != null)
            downloadingIcon.gameObject.SetActive(false);

        if (downloadButton != null)
            downloadButton.onClick.AddListener(StartDownloadAndClose);
    }

    // Called directly by the X button's pointer handler.
    public void OnCloseButtonPointerDown()
    {
        if (downloadStarted)
            return;

        if (hiddenDownloadIcon != null)
            hiddenDownloadIcon.gameObject.SetActive(true);
    }

    // Called when the pointer is released on the X button.
    public void OnCloseButtonPointerUp()
    {
        StartDownloadAndClose();
    }

    public void StartDownloadAndClose()
    {
        if (downloadStarted)
            return;

        downloadStarted = true;

        GameEvents.Raise("popup_clicked");

        if (downloadingIcon != null)
            downloadingIcon.gameObject.SetActive(true);

        if (iconCoroutine != null)
            StopCoroutine(iconCoroutine);

        iconCoroutine = StartCoroutine(HideDownloadingIcon());

        GlitchDirector.I.BeginDownload(hiddenDownloadSeconds);

        StartCoroutine(CloseWindowAfterDelay());
    }

    private IEnumerator HideDownloadingIcon()
    {
        yield return new WaitForSeconds(hiddenDownloadSeconds);

        if (downloadingIcon != null)
            downloadingIcon.gameObject.SetActive(false);

        iconCoroutine = null;
    }

    private IEnumerator CloseWindowAfterDelay()
    {
        yield return new WaitForSeconds(closeDelay);

        if (desktopWindow != null)
            desktopWindow.Close();
        else
            Debug.LogWarning("PopupTrick: DesktopWindow component not found.", this);
    }

    private void OnDestroy()
    {
        if (downloadButton != null)
            downloadButton.onClick.RemoveListener(StartDownloadAndClose);
    }

    public void SetHiddenDownloadIcon(bool visible)
    {
        if (hiddenDownloadIcon != null)
            hiddenDownloadIcon.gameObject.SetActive(visible);
    }
}