using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CardScript : MonoBehaviour
{
    // defines all the variables used on the card prefabs
    public string face;
    public string suit;
    public string iconSuit;
    public string iconFace;
    public Text iconFaceTextLeft;
    public Text iconSuitTextRight;
    public Text iconSuitTextLeft;
    public Text iconFaceTextRight;
    //sets the card prefab with values defined in list on DeckScript
    public void update_card(string _face, string _suit, string _iconSuit, string _iconFace)
    {
        face = _face;
        suit = _suit;      
        iconFace = _iconFace;
        iconSuit = _iconSuit;

        iconFaceTextLeft.text = iconFace;
        iconFaceTextRight.text = iconFace;
        iconSuitTextLeft.text = iconSuit;
        iconSuitTextRight.text = iconSuit;
    }
}

