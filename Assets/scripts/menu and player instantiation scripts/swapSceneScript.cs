using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class swapSceneScript : MonoBehaviour
{

    public void SwapToMainMenuScene()
    {
        SceneManager.LoadScene(0);
    }
    public void SwapToPlayerCreationScene()
    {
        SceneManager.LoadScene(1);
    }
    public void SwapToGameScene()
    {
        SceneManager.LoadScene(2);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
