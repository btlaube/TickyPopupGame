using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// The "drop the report here" box inside WorkHub. Accepts one FileItem with the right fileId
/// (once the required flag - the report being saved - has been raised), then enables Submit.
/// Flags: report_in_tray, report_submitted.
/// </summary>
public class SubmissionTray : MonoBehaviour, IDropHandler
{
    [SerializeField] string acceptedFileId = "quarterly_report";
    [Tooltip("The report is only accepted after this flag exists. Leave empty to always accept.")]
    [SerializeField] string requiredFlag = "report_saved";
    [SerializeField] string wrongFileMessage = "That's not the quarterly report.";
    [SerializeField] string notReadyMessage = "This report still has the old totals. Update and save it first.";

    [SerializeField] TMP_Text statusText;
    [SerializeField] GameObject attachedVisual;
    [SerializeField] TMP_Text attachedName;
    [SerializeField] Button submitButton;
    public UnityEvent onSubmitted;

    public bool HasFile { get; private set; }
    public bool Submitted { get; private set; }

    void Awake()
    {
        if (submitButton != null) submitButton.onClick.AddListener(Submit);
        Refresh();
    }

    public void OnDrop(PointerEventData e)
    {
        if (HasFile || Submitted || e.pointerDrag == null) return;

        var file = e.pointerDrag.GetComponentInParent<FileItem>();
        if (file == null) return;

        if (file.fileId != acceptedFileId)
        {
            Say(wrongFileMessage);
            return;
        }

        if (!string.IsNullOrEmpty(requiredFlag) && !GameEvents.Has(requiredFlag))
        {
            Say(notReadyMessage);
            return;
        }

        HasFile = true;
        if (attachedName != null) attachedName.text = file.displayName;
        Refresh();
        GameEvents.Raise("report_in_tray");
    }

    void Submit()
    {
        if (!HasFile || Submitted) return;
        Submitted = true;
        Refresh();
        GameEvents.Raise("report_submitted");
        onSubmitted.Invoke();
    }

    void Say(string msg)
    {
        if (statusText != null) statusText.text = msg;
    }

    void Refresh()
    {
        if (attachedVisual != null) attachedVisual.SetActive(HasFile);
        if (submitButton != null) submitButton.interactable = HasFile && !Submitted;
        Say(Submitted ? "Submitted. Thank you!"
          : HasFile ? "Ready to submit."
          : "Drag the quarterly report here.");
    }
}
