using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class playerCreationScene : MonoBehaviour
{
    public gameManager gameManagerScript;
    public void Get3Players()
    {
        PlayerPrefs.SetInt("players", 3);
    }
    public void Get4Players()
    {
        PlayerPrefs.SetInt("players", 4);
    }
    public void Get5Players()
    {
        PlayerPrefs.SetInt("players", 5);
    }
}
