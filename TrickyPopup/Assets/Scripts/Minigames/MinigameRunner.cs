using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MinigameRunner : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] Minigame[] allGames;
    [SerializeField] TMP_Text banner;

    readonly List<Minigame> bag = new();
    readonly HashSet<Minigame> seen = new();
    Minigame last;

    public int Losses { get; private set; }

    public IEnumerator RunBurst(int glitchNumber, int count)
    {
        Losses = 0;
        float difficulty = (glitchNumber - 1) / 3f;
        for (int i = 0; i < count; i++)
        {
            HideAll();
            var g = Draw(glitchNumber);
            last = g; seen.Add(g);
            StartCoroutine(FlashBanner(g.label, g.grace + 0.3f));
            g.Begin(MouseTracker.World(cam), difficulty);
            yield return new WaitUntil(() => g.Finished);
            if (!g.Won) Losses++;
        }
    }

    public void HideAll() { foreach (var g in allGames) g.gameObject.SetActive(false); }

    Minigame Draw(int glitch)
    {
        var pool = new List<Minigame>();
        foreach (var g in allGames) if (g.unlockGlitch <= glitch) pool.Add(g);

        var fresh = pool.FindAll(g => g.unlockGlitch == glitch && !seen.Contains(g));
        if (fresh.Count > 0) return fresh[Random.Range(0, fresh.Count)];

        if (bag.Count == 0) bag.AddRange(pool);
        var candidates = bag.FindAll(g => g != last);
        if (candidates.Count == 0)
        {
            bag.Clear(); bag.AddRange(pool);
            candidates = pool.FindAll(g => g != last);
        }
        var pick = candidates[Random.Range(0, candidates.Count)];
        // var pick = candidates[0];
        bag.Remove(pick);
        return pick;
    }

    IEnumerator FlashBanner(string text, float seconds)
    {
        banner.text = text;
        yield return new WaitForSeconds(seconds);
        banner.text = "";
    }
}