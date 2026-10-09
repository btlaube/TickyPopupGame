using UnityEngine;

public abstract class Minigame : MonoBehaviour
{
    [Header("Identity")]
    public string label = "CLICK!";
    public int unlockGlitch = 1;          // 1 = from G1, 2 = from G2, etc.

    [Header("Timing")]
    public float duration = 8f;
    public float grace = 0.4f;            // nothing can hurt you yet
    public bool idleCheck = false;        // backup rule for games that don't punish idleness
    public float idleLimit = 1f;

    public bool Finished { get; private set; }
    public bool Won { get; private set; }

    protected Vector2 startWorld;         // where the cursor was when the glitch hit
    protected float difficulty;           // 0..1, scales with glitch number
    protected float t;

    public void Begin(Vector2 start, float diff)
    {
        startWorld = start; difficulty = diff;
        t = 0; Finished = false; Won = false;
        MouseTracker.ResetIdle();
        gameObject.SetActive(true);
        OnBegin();
    }

    void Update()
    {
        if (Finished) return;
        t += Time.deltaTime;
        if (t < grace) return;
        Tick();
        if (Finished) return;
        if (idleCheck && MouseTracker.IdleSeconds > idleLimit) Lose();
        else if (t >= grace + duration) TimeUp();
    }

    protected abstract void OnBegin();
    protected abstract void Tick();
    protected virtual void TimeUp() => Lose();   // "survive" games override this to Win()

    protected void Win()  { if (!Finished) { Finished = true; Won = true; } }
    protected void Lose() { if (!Finished) { Finished = true; Won = false; } }
}