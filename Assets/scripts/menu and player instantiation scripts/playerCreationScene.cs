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
        gameManagerScript.playerNumInput = 3;
        print(gameManagerScript.playerNumInput);
    }
    public void Get4Players()
    {
        gameManagerScript.playerNumInput = 4;
    }
    public void Get5Players()
    {
        gameManagerScript.playerNumInput = 5;
    }
}
