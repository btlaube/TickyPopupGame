
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchTransition : MonoBehaviour
{
    [Header("Wave")]
    [SerializeField] Camera targetCamera;
    [SerializeField] float maxRadius = 25f;
    [SerializeField] float ringThickness = 0.8f;

    [Header("Glitch Fragments")]
    [SerializeField] float fragmentsPerRadius = 7f;
    [SerializeField] Vector2 fragmentSize = new Vector2(0.12f, 0.12f);
    [SerializeField] Vector2 horizontalStretch = new Vector2(1f, 5f);
    [SerializeField] float positionJitter = 0.7f;
    [SerializeField] int sortingOrder = 100;

    [Header("Colors")]
    [SerializeField] Color cyan = new Color(0.1f, 1f, 1f);
    [SerializeField] Color magenta = new Color(1f, 0.1f, 0.8f);
    [SerializeField] Color white = Color.white;
    [SerializeField] Color black = Color.black;

    readonly List<SpriteRenderer> fragments = new();

    Sprite pixelSprite;
    Transform fragmentRoot;
    Coroutine activeRoutine;
    bool expanding;

    void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        CreatePixelSprite();

        GameObject root = new GameObject("Glitch Fragments");
        fragmentRoot = root.transform;
        fragmentRoot.SetParent(transform, false);
    }

    void CreatePixelSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.name = "Glitch Pixel";
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        pixelSprite = Sprite.Create(
            texture,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f),
            1f
        );
    }

    Vector3 GetCursorWorldPosition()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null)
            return transform.position;

        Vector3 mouse = MouseTracker.ScreenPos;

        // Preserve the effect object's world depth relative
        // to the camera when converting screen to world space.
        mouse.z = Mathf.Abs(
            targetCamera.transform.position.z - transform.position.z
        );

        Vector3 world = targetCamera.ScreenToWorldPoint(mouse);
        world.z = transform.position.z;

        return world;
    }

    public IEnumerator Expand(float seconds)
    {
        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        ClearImmediately();

        transform.position = GetCursorWorldPosition();

        expanding = true;
        activeRoutine = StartCoroutine(ExpandRoutine(seconds));

        yield return activeRoutine;

        activeRoutine = null;
        expanding = false;
    }

    IEnumerator ExpandRoutine(float seconds)
    {
        seconds = Mathf.Max(0.01f, seconds);

        float elapsed = 0f;
        float previousRadius = 0f;

        while (elapsed < seconds)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / seconds);
            float radius = Mathf.Lerp(0f, maxRadius, progress);

            float distanceTravelled = radius - previousRadius;

            int count = Mathf.CeilToInt(
                Mathf.Max(1f, distanceTravelled * fragmentsPerRadius
                    * Mathf.Max(radius, 1f))
            );

            // Keep spawn counts manageable during fast transitions.
            count = Mathf.Clamp(count, 1, 100);

            for (int i = 0; i < count; i++)
                SpawnFragment(radius);

            previousRadius = radius;

            yield return null;
        }

        // Fill out the final edge of the wave.
        for (int i = 0; i < 60; i++)
            SpawnFragment(maxRadius);

        expanding = false;
    }

    void SpawnFragment(float radius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);

        Vector2 direction = new Vector2(
            Mathf.Cos(angle),
            Mathf.Sin(angle)
        );

        float offset = Random.Range(
            -ringThickness,
            ringThickness
        );

        Vector2 position =
            direction * Mathf.Max(0f, radius + offset);

        position += Random.insideUnitCircle * positionJitter;

        GameObject piece = new GameObject("Glitch Fragment");
        piece.transform.SetParent(fragmentRoot, false);
        piece.transform.localPosition = new Vector3(
            position.x,
            position.y,
            0f
        );

        SpriteRenderer sr = piece.AddComponent<SpriteRenderer>();
        sr.sprite = pixelSprite;
        sr.sortingOrder = sortingOrder;

        float width = Random.Range(
            fragmentSize.x,
            fragmentSize.x * horizontalStretch.y
        );

        float height = Random.Range(
            fragmentSize.y * 0.5f,
            fragmentSize.y * 2f
        );

        // Most fragments are small blocks; some are long
        // horizontal digital tearing artifacts.
        if (Random.value < 0.65f)
            width = Random.Range(
                fragmentSize.x,
                fragmentSize.x * 2f
            );

        piece.transform.localScale = new Vector3(
            width, height, 1f
        );

        Color[] colors = { cyan, magenta, white, black };
        sr.color = colors[Random.Range(0, colors.Length)];

        fragments.Add(sr);
    }

    public IEnumerator Clear(float seconds)
    {
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
            activeRoutine = null;
        }

        expanding = false;

        seconds = Mathf.Max(0f, seconds);

        if (seconds <= 0f)
        {
            ClearImmediately();
            yield break;
        }

        List<SpriteRenderer> current =
            new List<SpriteRenderer>(fragments);

        float elapsed = 0f;

        while (elapsed < seconds)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsed / seconds);

            foreach (SpriteRenderer sr in current)
            {
                if (sr == null) continue;

                Color color = sr.color;
                color.a = alpha;
                sr.color = color;
            }

            yield return null;
        }

        ClearImmediately();
    }

    void ClearImmediately()
    {
        fragments.Clear();

        if (fragmentRoot == null)
            return;

        for (int i = fragmentRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(fragmentRoot.GetChild(i).gameObject);
        }
    }

    void OnDestroy()
    {
        if (pixelSprite != null)
        {
            Destroy(pixelSprite.texture);
            Destroy(pixelSprite);
        }
    }
}
