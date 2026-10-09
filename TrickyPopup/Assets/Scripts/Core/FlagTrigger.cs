using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// "When flag X is raised, wait a moment, then do something (UnityEvent)."
/// Reusable glue: deliver the ending email, show the sequel popup, start a glitch, etc.
/// </summary>
public class FlagTrigger : MonoBehaviour
{
    [SerializeField] string flag;
    [SerializeField] float delay = 0f;
    [SerializeField] bool fireOnce = true;
    public UnityEvent onTriggered;

    bool fired;

    void OnEnable() { GameEvents.FlagRaised += OnFlag; }
    void OnDisable() { GameEvents.FlagRaised -= OnFlag; }

    void OnFlag(string raisedFlag)
    {
        if (raisedFlag != flag) return;
        if (fireOnce && fired) return;
        fired = true;
        StartCoroutine(Fire());
    }

    IEnumerator Fire()
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);
        onTriggered.Invoke();
    }
}
