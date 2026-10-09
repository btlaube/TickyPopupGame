using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance;
    public FadeTransition transition;

    [SerializeField] private float transitionTime = 1f;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
    }

    public void ResetScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadScene(int sceneToLoad) {
        StartCoroutine(LoadLevel(sceneToLoad));
    }

    IEnumerator LoadLevel(int levelIndex) {
        transition.FadeIn();

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);

        transition.FadeOut();
    }

    public void StartTransition()
    {
        transition.FadeIn();
    }

    public void EndTransition()
    {
        transition.FadeOut();
    }

    public void ActivateTransition(int transitionIndex)
    {
        StartCoroutine(Transition(transitionIndex));
    }

    IEnumerator Transition(int transitionIndex) {
        transition.FadeIn();

        yield return new WaitForSeconds(transitionTime);

        transition.FadeOut();
    }

    public void Quit() {
        Application.Quit();
    }
}
