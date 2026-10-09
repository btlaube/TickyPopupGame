using UnityEngine;

public class GameSceneTransitions : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        LevelLoader.instance.LoadScene(0);
    }
}
