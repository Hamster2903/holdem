using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    //make list of players
    public DeckScript deckScript;
    public Button raiseButton;
    public Button callButton;
    public Button foldButton;
    public int littleBlindBetValue;
    public int bigBlindBetValue;
    public int callValue;
    public int raiseValue;
    public int roundValue;
    public List<PlayerClassScript> players;
    public GameObject playerPrefab;
    public GameObject cardGroups;

    //make list of player positions
    public List<GameObject> playerPositions;
    void Start()
    {
        deckScript.Generate();
        deckScript.Shuffle();
        deckScript.DealToFlop();
        GeneratePlayers(5);
        GeneratePlayerObjects();
        DealToHands();
    }

    //pre-flop, little and big blinds bet, moves around table until everyone has acted
    //store and update round value based on community cards
    //deal to turn on the second round, players re-bet/call/fold/raise
    //deal to river on the third round, players re-bet/call/fold/raise
    //showdown, the card hands are evualated and the winner is determined
    public void GeneratePlayers(int numPlayers)
    {
        players.Clear();
        for (int i = 0; i < numPlayers; i++)
        {
            PlayerClassScript newPlayer = new PlayerClassScript("TestPlayer" + i, false, false);
            players.Add(newPlayer);
        }
        print(numPlayers);
    }
    public void DealToHands()
    {
        for (int i = 0; i < players.Count; i++)
        {
            GameObject cardToMove = deckScript.cards[0]; //defines the card about to be moved and then selects it from the top of the list
            deckScript.cards.Remove(cardToMove); //removes card from list
            players[i].cards.Add(cardToMove); //adds removed card to player list for each player.
            print(cardToMove);
        }
    }
    

    public void GeneratePlayerObjects()
    {
        //defines player position in objects as its position in the list
        GameObject playerPositionsParent = playerPositions[players.Count - 2];
        foreach (Transform item in playerPositionsParent.transform)//for each position in the list instantiates players
        {
            print("hello");
            GameObject newPlayer = Instantiate(playerPrefab);
            newPlayer.transform.SetParent(item.transform);
            newPlayer.transform.position = item.position;
            //instantiate card prefabs based on cards in their respective player list
            //how to access the specific card group 
        }

    }
    public void GenerateCardGroupObjects()
    {
        for (int i = 0; i < players.Count; i++)
        {
            //instantiate cards from list under the generated player game objects
        }
    }


}
