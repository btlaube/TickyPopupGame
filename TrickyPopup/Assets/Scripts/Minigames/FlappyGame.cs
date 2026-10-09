using System.Collections.Generic;
using UnityEngine;

public class FlappyGame : Minigame
{
    [SerializeField] Transform bird;
    [SerializeField] GameObject pipePrefab;
    [SerializeField] float gravity = -22f, flapSpeed = 8f;
    [SerializeField] int pipesToWin = 3;

    class Pipe { public Transform t; public bool counted; }
    readonly List<Pipe> pipes = new();
    const float BirdX = -4f;
    float vy, scroll, gapHalf, spawnTimer;
    int passed;

    protected override void OnBegin()
    {
        foreach (var p in pipes) Destroy(p.t.gameObject);
        pipes.Clear();
        passed = 0; vy = 0; spawnTimer = 2f;
        gapHalf = Mathf.Lerp(1.9f, 1.3f, difficulty);
        scroll  = Mathf.Lerp(4.5f, 7f, difficulty);
        bird.position = new Vector3(BirdX, startWorld.y, 0);
        Spawn(7f, Mathf.Clamp(startWorld.y, -2.5f, 2.5f));   // first gap lines up with the cursor
    }

    protected override void Tick()
    {
        float dt = Time.deltaTime;
        if (MouseTracker.Down) vy = flapSpeed;
        vy += gravity * dt;
        bird.position += new Vector3(0, vy * dt, 0);
        if (Mathf.Abs(bird.position.y) > 5f) { Lose(); return; }

        spawnTimer -= dt;
        if (spawnTimer <= 0) { Spawn(10f, Random.Range(-2.5f, 2.5f)); spawnTimer = 2.2f; }

        foreach (var p in pipes)
        {
            p.t.position += Vector3.left * scroll * dt;
            float dx = Mathf.Abs(p.t.position.x - BirdX);
            if (dx < 0.9f && Mathf.Abs(bird.position.y - p.t.position.y) > gapHalf - 0.3f) { Lose(); return; }
            if (!p.counted && p.t.position.x < BirdX - 0.9f) { p.counted = true; passed++; }
        }
        if (passed >= pipesToWin) Win();
    }

    void Spawn(float x, float gapY)
    {
        var go = Instantiate(pipePrefab, transform);
        go.transform.position = new Vector3(x, gapY, 0);
        go.transform.Find("Top").localPosition    = new Vector3(0,  gapHalf + 5f, 0);
        go.transform.Find("Bottom").localPosition = new Vector3(0, -gapHalf - 5f, 0);
        pipes.Add(new Pipe { t = go.transform });
    }
}