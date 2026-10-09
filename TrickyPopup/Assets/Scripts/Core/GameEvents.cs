using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tiny global "flag" bus. Anything can call GameEvents.Raise("some_flag").
/// Objectives (and later the GlitchDirector) listen for flags.
/// Each flag only fires the FIRST time it is raised, and is remembered afterwards.
/// </summary>
public static class GameEvents
{
    public static event Action<string> FlagRaised;

    /// <summary>Prints every NEW flag to the Console as "[Flag] name" (editor / dev builds only).</summary>
    public static bool LogFlags = true;

    static readonly HashSet<string> raised = new HashSet<string>();

    // Keeps statics clean if "Enter Play Mode Options" (no domain reload) is enabled.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        raised.Clear();
        FlagRaised = null;
    }

    public static bool Has(string flag) => !string.IsNullOrEmpty(flag) && raised.Contains(flag);

    public static void Raise(string flag)
    {
        if (string.IsNullOrEmpty(flag)) return;
        if (!raised.Add(flag)) return;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (LogFlags) Debug.Log("[Flag] " + flag);
#endif
        FlagRaised?.Invoke(flag);
    }

    /// <summary>Call when restarting a run without reloading the scene.</summary>
    public static void ResetAll() => raised.Clear();
}
