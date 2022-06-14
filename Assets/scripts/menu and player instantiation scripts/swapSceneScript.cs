using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class swapSceneScript : MonoBehaviour
{

    public void swap_to_main_menu_scene()
    {
        SceneManager.LoadScene(0);
    }
    public void swap_to_player_selection_scene()
    {
        SceneManager.LoadScene(1);
    }
    public void swap_to_game_scene()
    {
        SceneManager.LoadScene(2);
    }
    public void swap_to_information_scene()
    {
        SceneManager.LoadScene(3);
    }
    public void swap_to_game_over_scene()
    {
        SceneManager.LoadScene(4);
    }
    public void quit_game()
    {
        Application.Quit();
    }
    public void mute_toggle(bool muted)
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
