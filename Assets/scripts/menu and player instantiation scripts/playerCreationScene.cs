using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class playerCreationScene : MonoBehaviour
{
    public gameManager gameManagerScript;
    public void get_3_players()
    {
        PlayerPrefs.SetInt("players", 3);
    }
    public void get_4_players()
    {
        PlayerPrefs.SetInt("players", 4);
    }
    public void get_5_players()
    {
        PlayerPrefs.SetInt("players", 5);
    }
}
