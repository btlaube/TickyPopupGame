using UnityEngine;

public class MainMenuTransitions : MonoBehaviour
{
    public void StartGame()
    {
        LevelLoader.instance.LoadScene(1);
    }
}
