using UnityEngine;

public class FadeTransition : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeDuration = 1f;

    void Start() => FadeOut();

    public void FadeIn()
    {
        StopAllCoroutines();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        StartCoroutine(Fade(0f, 1f));
    }

    public void FadeOut()
    {
        StopAllCoroutines();
        canvasGroup.alpha = 1f;
        StartCoroutine(Fade(1f, 0f));
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    System.Collections.IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}
