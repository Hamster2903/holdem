using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    //make list of players
    public DeckScript deckScript;
    public PlayerClassScript playerClassScript;
    public Button raiseButton;
    public Button callButton;
    public Button foldButton;
    public int littleBlindBetValue;
    public int bigBlindBetValue;
    public int callValue;
    public int raiseValue;
    public int roundValue;
    public List<PlayerClassScript> players;
    void Start()
    {
        deckScript.Generate();
        deckScript.Shuffle();
        deckScript.DealToFlop();
        DealToHands();
    }
   
    //pre-flop, little and big blinds bet, moves around table until everyone has acted
    //store and update round value based on community cards
    //deal to turn on the second round, players re-bet/call/fold/raise
    //deal to river on the third round, players re-bet/call/fold/raise
    //showdown, the card hands are evualated and the winner is determined
    //
    public void DealToHands()
    {
        //askjs dck script for two crds for each player
        //refrences deck script, refrences players through list, adds two cards to players
    }
    
    public void GeneratePlayers(int numPlayers =2)
    {
        for (int i = 0; i < numPlayers; i++)
        {
            PlayerClassScript newPlayer = new PlayerClassScript(0,0,false,"TestPlayer",0,false,false);
            players.Add(newPlayer);
        }
        Console.WriteLine(numPlayers);
    }

}
