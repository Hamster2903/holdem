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
    public void SwapToInformationScene()
    {
        SceneManager.LoadScene(3);
    }
    public void SwapToGameOverScene()
    {
        SceneManager.LoadScene(4);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void MuteToggle(bool muted)
    {
        if(muted)
        {
            AudioListener.volume = 0;
        }
        else
        {
            AudioListener.volume = 1;
        }
    }
}
