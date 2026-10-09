using TMPro;
using UnityEngine;

/// <summary>
/// The highlighted table in the report. Hover shows PASTE (once the totals are copied);
/// clicking writes the copied values into the cells.
/// Flag: totals_pasted.
/// </summary>
[RequireComponent(typeof(HoverAction))]
public class PasteTable : MonoBehaviour
{
    [Tooltip("The VALUE cells in reading order. Must match the Copy table's cell order.")]
    [SerializeField] TMP_Text[] valueCells;
    [SerializeField] string clipboardKind = "totals";
    [SerializeField] Color pastedColor = new Color(0.10f, 0.50f, 0.15f, 1f);
    [Tooltip("The pulsing yellow glow behind the table; hidden after pasting.")]
    [SerializeField] GameObject pulseGlow;
    [SerializeField] ReportDocument report;

    public bool Pasted { get; private set; }

    HoverAction hover;

    void Awake()
    {
        hover = GetComponent<HoverAction>();
        hover.CanActivate = () => GameClipboard.Has(clipboardKind);
        hover.onActivate.AddListener(Paste);
    }

    void Paste()
    {
        if (Pasted) return;
        var data = GameClipboard.Data;
        for (int i = 0; i < valueCells.Length && i < data.Length; i++)
        {
            valueCells[i].text = data[i];
            valueCells[i].color = pastedColor;
        }

        Pasted = true;
        hover.Interactable = false;
        if (pulseGlow != null) pulseGlow.SetActive(false);

        if (report != null) report.OnTotalsPasted();
        GameEvents.Raise("totals_pasted");
    }
}
