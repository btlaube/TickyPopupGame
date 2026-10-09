using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The report window's toolbar logic: shows the file name (with * when unsaved),
/// and a Save button that only works once the totals have been pasted.
/// Flag: report_saved.
/// </summary>
public class ReportDocument : MonoBehaviour
{
    [SerializeField] string fileName = "Q3_Quarterly_Report.docx";
    [SerializeField] TMP_Text docNameText;
    [SerializeField] TMP_Text statusText;
    [SerializeField] Button saveButton;

    public bool TotalsPasted { get; private set; }
    public bool Saved { get; private set; }

    void Awake()
    {
        if (saveButton != null) saveButton.onClick.AddListener(Save);
        Refresh();
    }

    public void OnTotalsPasted()
    {
        TotalsPasted = true;
        Saved = false;
        Refresh();
    }

    public void Save()
    {
        if (!TotalsPasted || Saved) return;
        Saved = true;
        Refresh();
        GameEvents.Raise("report_saved");
    }

    void Refresh()
    {
        bool dirty = TotalsPasted && !Saved;
        if (docNameText != null) docNameText.text = dirty ? fileName + " *" : fileName;
        if (saveButton != null) saveButton.interactable = dirty;
        if (statusText != null)
            statusText.text = Saved ? "All changes saved."
                            : TotalsPasted ? "Unsaved changes."
                            : "All changes saved.";
    }
}
