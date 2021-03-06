﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Deck_Detail : System.Object
{
    public int CardAmount;
    public CARD_TYPE CardType;
}

public class Deck : MonoBehaviour
{

    public List<Deck_Detail> CardsToInclude = null;
    public List<GameObject> Cards;
    public HandHandler handHandler;
    bool drawable = true, a_draw = true;

    // Use this for initialization
    void Start()
    {
        GenerateDeck();
        PlayAudio.instance.m_source.clip = PlayAudio.instance.m_drawCard;
        SceneData.sceneData.NewDeckButton.gameObject.SetActive(false);
        SceneData.sceneData.fireSPrite.gameObject.SetActive(false);
    }

    public void GenerateDeck()
    {
        if (CardsToInclude == null)
            return;

        Cards = new List<GameObject>();

        foreach (Deck_Detail detail in CardsToInclude)
        {
            GameObject CardinDatabase = null;
            SharedData.instance.CardDatabase.TryGetValue(detail.CardType, out CardinDatabase);
            if (CardinDatabase != null)
            {
                for (int i = 0; i < detail.CardAmount; ++i)
                {
                    GameObject newcard = Instantiate(CardinDatabase);
                    Cards.Add(newcard);
                    newcard.transform.SetParent(SceneData.sceneData.UI.transform);
                }
            }
        }

        ShuffleDeck();
    }

    // Update is called once per frame
    void Update()
    {
        Animator draw = gameObject.transform.GetChild(0).GetComponent<Animator>();
        if (draw.GetCurrentAnimatorStateInfo(0).IsName("Draw") && draw.GetCurrentAnimatorStateInfo(0).length >
          draw.GetCurrentAnimatorStateInfo(0).normalizedTime && !a_draw)//draw the card after animation
        {
            draw.SetBool("Isdraw", false);
            a_draw = true;
            addcard();

        }

        if (Cards.Count <= 0)
        {
            SceneData.sceneData.NewDeckButton.gameObject.SetActive(true);
            if (SceneData.sceneData.Player.GetGold() >= 500)
                SceneData.sceneData.fireSPrite.gameObject.SetActive(true);
            else
                SceneData.sceneData.fireSPrite.gameObject.SetActive(false);
        }


    }


    public void ShuffleDeck()
    {
        for (int i = 0; i < 50; ++i)
        {
            int randomIndex1 = Random.Range(0, Cards.Count);
            int randomIndex2 = Random.Range(0, Cards.Count);
            GameObject temp = Cards[randomIndex1];
            Cards[randomIndex1] = Cards[randomIndex2];
            Cards[randomIndex2] = temp;

        }
    }

    public void DrawCard()
    {

        // GameObject drawcard = gameObject.transform.GetChild(0).GetComponent<GameObject>();
        Animator draw = gameObject.transform.GetChild(0).GetComponent<Animator>();
        if (Cards.Count <= 30 && Cards.Count > 0 && drawable == true)
        {

            if (SceneData.sceneData.handhandler.handsize < 5 && a_draw) //&& draw.GetCurrentAnimatorStateInfo(0).IsName("Idle"))//can draw here
            {
                //gameObject.transform.GetChild(0).GetComponent<Animation>().
                PlayAudio.instance.m_source.clip = PlayAudio.instance.m_drawCard;
                PlayAudio.instance.m_source.volume = 0.5f;
                PlayAudio.instance.PlayOnce();
                draw.SetBool("Isdraw", true);
                a_draw = false;
            }
        }

        else if (Cards.Count <= 0)
        {
            if (SceneData.sceneData.handhandler.handsize < 5 && drawable == false && a_draw)
            {
                draw.SetBool("Isdraw", true);
                a_draw = false;
                drawable = false;
            }
        }
    }


    void addcard()
    {
        GameObject firstCard = Cards.ElementAt(0);
        SceneData.sceneData.handhandler.cardlist.Add(firstCard);
        SceneData.sceneData.handhandler.ResetCardPos();
        firstCard.GetComponent<UnitCards>().GenerateBuilding();
        Cards.Remove(firstCard);
        return;
    }

    public void RegenerateDeck()
    {
        if (Cards.Count <= 0)
        {
            if (SceneData.sceneData.Player.GetGold() >= 500)
            {

                SceneData.sceneData.Player.SpendPlayerGold(500);
                PlayAudio.instance.m_source.clip = PlayAudio.instance.m_newDeck;
                PlayAudio.instance.m_source.volume = 0.5f;
                PlayAudio.instance.PlayOnce();
                drawable = true;
                GenerateDeck();
                ShuffleDeck();
            }
        }
        SceneData.sceneData.NewDeckButton.gameObject.SetActive(false);
        SceneData.sceneData.fireSPrite.gameObject.SetActive(false);
        //else if (Cards.Count > 0)
        //SceneData.sceneData.NewDeckButton.enabled = false;
    }

}