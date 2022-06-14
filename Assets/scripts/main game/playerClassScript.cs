using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerClassScript : MonoBehaviour
{
    public int numOfChips = 1000;
    public int numOfChipsInPot = 0;
    public bool hasFolded = false;
    public bool hasRaised = false;
    public Text playerChipsText;
    public bool hasCalled = false;
    public bool isActive = false;
    public bool isAllIn = false;
    public int mostRecentBet;
    public string playerName = "";
    public Text playerNameText;
    public int valueOfCardsInHand = 0;
    public bool isLittleBlind;
    public bool isBigBlind;
    public bool isDealer;
    public List<GameObject> cards;
    public int playerId;
}
