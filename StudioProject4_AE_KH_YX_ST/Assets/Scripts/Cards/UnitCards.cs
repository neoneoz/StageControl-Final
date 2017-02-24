using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum CARD_TYPE
{
    BALLISTA_FACTORY,
    BUSTER_FACTORY,
    IRON_GOLEM_FACTORY,
    CLOCKWORK_FACTORY,
    RAILGUN_FACTORY,
    SPIDERTANK_FACTORY,
    MISSILE_SPELL,
    NEWDECK
}


public class UnitCards : MonoBehaviour
{
    public string buildingName;
    public string UnitType;
    public int goldValue;// gold needed
    public float Time;
    public string cardDescr;// String containing description of card
    public GameObject GOModel;
    RectTransform card;
    public Text nameText;
    public Text goldText;
    public Text timeText;
    public Text cardDescription;
    public CARD_TYPE cardType;
    /* Spell card only stuff */
    public float damage; // Damage that this spell card deals, need to limit it later
    public string effectiveUnitType; // Which unit is this spell effective against
    public bool is_spell = false;
    public int effectRadius = 1; // Radius this spell has an effect
    /**/

    // Use this for initialization
    void Start()
    {
        SetText();
        //take this out tlater
        if (GOModel)
            GOModel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }



    public void GenerateBuilding()
    {
        //create a building when drawn
        if (!is_spell)
            GOModel = Instantiate(GOModel);
    }

    public void ConstructBuilding()
    {

    }

    public void ResetCardPos()
    {
        SceneData.sceneData.handhandler.ResetCardPos();
    }

    public void SetText()
    {
        nameText.text = buildingName + "\n" + " Factory";
        goldText.text = goldValue.ToString();
        timeText.text = Time.ToString();
        cardDescription.text = cardDescr;
    }

}
