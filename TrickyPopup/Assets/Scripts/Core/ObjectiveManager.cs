using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Objective
{
    public string id;
    [TextArea(1, 3)] public string text;
    [Tooltip("The GameEvents flag that completes this objective, e.g. workapp_login")]
    public string completeOnFlag;
    [NonSerialized] public bool done;
}

[Serializable]
public class ObjectiveStage
{
    public string title;
    [TextArea(1, 3)] public string note;
    public Objective[] objectives;
}

/// <summary>
/// Drives the sticky note. Shows one stage at a time; objectives get struck out as their
/// flags are raised, and the next stage starts once every objective in the stage is done.
/// Flags raised BEFORE a stage starts still count (so doing steps out of order is fine).
/// </summary>
public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager I { get; private set; }

    [SerializeField] TMP_Text stickyText;
    [SerializeField] ObjectiveStage[] stages;
    [SerializeField] string doneColorHex = "#8C8C8C";
    [Tooltip("Seconds to leave a finished stage on screen before showing the next one.")]
    [SerializeField] float advanceDelay = 1f;
    [SerializeField] UnityEvent onStageStarted;
    [SerializeField] UnityEvent onAllComplete;

    [Header("Debug (component's ⋮ menu → Raise Debug Flag while playing)")]
    [SerializeField] string debugFlag;

    int stageIndex = -1;
    bool advancing;

    public bool AllComplete { get; private set; }
    public int StageIndex => stageIndex;

    void Awake() { I = this; }
    void OnEnable() { GameEvents.FlagRaised += OnFlag; }
    void OnDisable() { GameEvents.FlagRaised -= OnFlag; }
    void Start() { StartStage(0); }

    void StartStage(int index)
    {
        advancing = false;
        if (stages == null || index >= stages.Length)
        {
            AllComplete = true;
            Render();
            onAllComplete.Invoke();
            GameEvents.Raise("objectives_complete");
            return;
        }

        stageIndex = index;
        foreach (var o in stages[index].objectives)
            o.done = GameEvents.Has(o.completeOnFlag);

        Render();
        onStageStarted.Invoke();
        CheckStageComplete();
    }

    void OnFlag(string flag)
    {
        if (AllComplete || stageIndex < 0 || advancing) return;

        bool changed = false;
        foreach (var o in stages[stageIndex].objectives)
        {
            if (!o.done && o.completeOnFlag == flag)
            {
                o.done = true;
                changed = true;
            }
        }
        if (changed)
        {
            Render();
            CheckStageComplete();
        }
    }

    void CheckStageComplete()
    {
        if (advancing) return;
        foreach (var o in stages[stageIndex].objectives)
            if (!o.done) return;

        advancing = true;
        StartCoroutine(AdvanceAfterDelay());
    }

    IEnumerator AdvanceAfterDelay()
    {
        yield return new WaitForSeconds(advanceDelay);
        StartStage(stageIndex + 1);
    }

    void Render()
    {
        if (stickyText == null) return;

        if (AllComplete)
        {
            stickyText.text = "<b>All done!</b>\nNothing left on the list.";
            return;
        }

        var stage = stages[stageIndex];
        var sb = new StringBuilder();
        sb.Append("<b>").Append(stage.title).Append("</b>\n");
        if (!string.IsNullOrEmpty(stage.note))
            sb.Append("<i>").Append(stage.note).Append("</i>\n");
        sb.Append('\n');

        foreach (var o in stage.objectives)
        {
            if (o.done)
                sb.Append("<color=").Append(doneColorHex).Append("><s>[x] ").Append(o.text).Append("</s></color>\n");
            else
                sb.Append("[ ] ").Append(o.text).Append('\n');
        }
        stickyText.text = sb.ToString();
    }

    [ContextMenu("Raise Debug Flag")]
    void RaiseDebugFlag() { GameEvents.Raise(debugFlag); }
}
