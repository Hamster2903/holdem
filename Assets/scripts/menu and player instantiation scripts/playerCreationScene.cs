using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class playerCreationScene : MonoBehaviour
{
    public gameManager gameManagerScript;
    public Button get3PlayersButton;
    public Button get4PlayersButton;
    public Button get5PlayersButton;
    public void Get3Players()
    {
        gameManagerScript.GeneratePlayers(3, 1);
    }
    public void Get4Players()
    {
        gameManagerScript.GeneratePlayers(4, 1);
    }
    public void Get5Players()
    {
        gameManagerScript.GeneratePlayers(5, 1);
    }
}
