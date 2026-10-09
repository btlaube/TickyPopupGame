using UnityEngine;

/// <summary>The in-game clipboard. Copy on the spreadsheet fills it; paste on the report reads it.</summary>
public static class GameClipboard
{
    public static string Kind { get; private set; }
    public static string[] Data { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() { Kind = null; Data = null; }

    public static void Set(string kind, string[] data) { Kind = kind; Data = data; }
    public static bool Has(string kind) => Kind == kind && Data != null;
    public static void Clear() { Kind = null; Data = null; }
}
