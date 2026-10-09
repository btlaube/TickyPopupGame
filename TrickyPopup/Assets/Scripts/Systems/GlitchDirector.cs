using System.Collections;
using UnityEngine;


[System.Serializable]
public class Glitch
{
    public string windowOpenFlag;
    public string forcedFlag;
}

public class GlitchDirector : MonoBehaviour
{
    public static GlitchDirector I { get; private set; }

    // [Header("Glitch Sequence")]
    // public Glitch[] glitches;

    [SerializeField] MinigameRunner runner;
    [SerializeField] GlitchTransition transition;
    [SerializeField] GameObject desktopRoot;
    [SerializeField] float[] transitionTimes = { 1.2f, 0.9f, 0.6f, 0.4f };
    [SerializeField] float cooldown = 8f;
    [SerializeField] float rollChancePerSecond = 0.3f;
    readonly int[] burstSizes = { 1, 1, 1, 1 };

    public int Done { get; private set; }
    public bool BlockRandom;        // set true while dragging or typing
    public bool DownloadReady;

    bool running;
    int windowOpenFor, forcedPending;
    float cooldownUntil, nextRoll;

    void Awake() => I = this;

    public void BeginDownload(float seconds) => StartCoroutine(DownloadTimer(seconds));
    IEnumerator DownloadTimer(float s) { yield return new WaitForSeconds(s); DownloadReady = true; }

    public void OpenWindow(int n) { if (n > windowOpenFor) windowOpenFor = n; }
    public void Force(int n) { OpenWindow(n); if (n == Done + 1) forcedPending = n; }

    void Update()
    {
        if (running || !DownloadReady || Done >= 4) return;
        int next = Done + 1;

        if (forcedPending == next) { StartCoroutine(Run(next)); return; }

        if (BlockRandom || windowOpenFor < next || Time.time < cooldownUntil) return;
        if (MouseTracker.IdleSeconds > 0.5f || Time.time < nextRoll) return;
        nextRoll = Time.time + 1f;
        if (Random.value < rollChancePerSecond) StartCoroutine(Run(next));
    }

    IEnumerator Run(int n)
    {
        Debug.Log($"[GlitchDirector] Run {n}");
        running = true; forcedPending = 0;

        yield return transition.Expand(transitionTimes[n - 1]);
        // desktopRoot.SetActive(false);
        yield return transition.Clear(0.15f);

        yield return runner.RunBurst(n, burstSizes[n - 1]);

        yield return transition.Expand(0.2f);
        runner.HideAll();
        // desktopRoot.SetActive(true);
        yield return transition.Clear(0.25f);

        for (int i = 0; i < runner.Losses; i++) GameClock.I.Penalty();

        Done = n;
        cooldownUntil = Time.time + cooldown;
        running = false;
    }
}