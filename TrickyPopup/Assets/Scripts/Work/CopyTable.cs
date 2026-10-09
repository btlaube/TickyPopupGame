using TMPro;
using UnityEngine;

/// <summary>
/// Spreadsheet table. Clicking it (via HoverAction) copies the value cells to the game clipboard.
/// Flag: totals_copied.
/// </summary>
[RequireComponent(typeof(HoverAction))]
public class CopyTable : MonoBehaviour
{
    [Tooltip("The VALUE cells in reading order. Must match the Paste table's cell order.")]
    [SerializeField] TMP_Text[] valueCells;
    [SerializeField] string clipboardKind = "totals";
    [SerializeField] string copiedLabel = "COPIED!";

    HoverAction hover;

    void Awake()
    {
        hover = GetComponent<HoverAction>();
        hover.onActivate.AddListener(Copy);
    }

    void Copy()
    {
        var data = new string[valueCells.Length];
        for (int i = 0; i < valueCells.Length; i++) data[i] = valueCells[i].text;

        GameClipboard.Set(clipboardKind, data);
        hover.Flash(copiedLabel, 0.7f);
        GameEvents.Raise("totals_copied");
    }
}
