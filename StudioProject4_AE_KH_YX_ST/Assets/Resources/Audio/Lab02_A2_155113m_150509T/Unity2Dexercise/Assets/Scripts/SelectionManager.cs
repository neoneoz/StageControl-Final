using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectionManager : MonoBehaviour {

    private int RogueAttack, WarriorAttack, MageAttack;
    private int RogueDefence, WarriorDefence, MageDefence;
    private int RoguePower, WarriorPower, MagePower;
    private string RogueText, WarriorText, MageText;

    public GameObject GlassBow, WolfBlade, StoneBlade, BattleAxe, DarkBow, SkullAxe, Dagger, KnightHelmet, VikingHelmet, ThiefArmor, ThiefHood, KnightArmor, VikingArmor, ThiefBoots, CommonBoots;

    private string easyText, normalText, hardText;

    private string All, Weapons, Armors;

    public Text debugText;

	// Use this for initialization
	void Start () {
        //Initialise the stats
        RogueAttack = 180; WarriorAttack = 70; MageAttack = 10;
        RogueDefence = 70; WarriorDefence = 180; MageDefence = 60;
        RoguePower = 10; WarriorPower = 10; MagePower = 200;

        //Set the Strings
        RogueText = "Rogue"; WarriorText = "Warrior"; MageText = "Mage";
        easyText = "Easy"; normalText = "Normal"; hardText = "Hard";

        GameObject.Find("ClassText").GetComponent<Text>().text = WarriorText;

        //Don't display these GameObjects at the start
        GameObject.Find("RogueChara").GetComponent<Image>().enabled = false;
        GameObject.Find("MageChara").GetComponent<Image>().enabled = false;

        changeClass("WarriorChara");
        changeDifficulty("NormalSelection");
        displayLegend("Legend");
        changeFilter("AllItems");

	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void changeClass(string className)
    {
        if (className != "RogueChara")
        {
            GameObject.Find("RogueChara").GetComponent<Image>().enabled = false;
        }
        else
        {
            GameObject.Find("RogueChara").GetComponent<Image>().enabled = true;
            GameObject.Find("StatsValues").GetComponent<Text>().text = RogueAttack.ToString() + "\n" + RogueDefence.ToString() + "\n" + RoguePower.ToString();
            GameObject.Find("ClassText").GetComponent<Text>().text = RogueText;
        }

        if (className != "MageChara")
        {
            GameObject.Find("MageChara").GetComponent<Image>().enabled = false;
        }

        else
        {
            GameObject.Find("MageChara").GetComponent<Image>().enabled = true;
            GameObject.Find("StatsValues").GetComponent<Text>().text = MageAttack.ToString() + "\n" + MageDefence.ToString() + "\n" + MagePower.ToString();
            GameObject.Find("ClassText").GetComponent<Text>().text = MageText;

        }

        if (className != "WarriorChara")
        {
            GameObject.Find("WarriorChara").GetComponent<Image>().enabled = false;
        }

        else
        {
            GameObject.Find("WarriorChara").GetComponent<Image>().enabled = true;
            GameObject.Find("StatsValues").GetComponent<Text>().text = WarriorAttack.ToString() + "\n" + WarriorDefence.ToString() + "\n" + WarriorPower.ToString();
            GameObject.Find("ClassText").GetComponent<Text>().text = WarriorText;

        }
    }

    public void changeDifficulty(string difficulty)
    {
        if (difficulty != "EasySelection")
            GameObject.Find("EasySelection").GetComponent<Image>().enabled = false;
        else
        {
            GameObject.Find("EasySelection").GetComponent<Image>().enabled = true;
            GameObject.Find("DifficultyText").GetComponent<Text>().text = easyText;
        }

        if (difficulty != "NormalSelection")
            GameObject.Find("NormalSelection").GetComponent<Image>().enabled = false;
        else
        {
            GameObject.Find("NormalSelection").GetComponent<Image>().enabled = true;
            GameObject.Find("DifficultyText").GetComponent<Text>().text = normalText;
        }

        if (difficulty != "HardSelection")
            GameObject.Find("HardSelection").GetComponent<Image>().enabled = false;
        else
        {
            GameObject.Find("HardSelection").GetComponent<Image>().enabled = true;
            GameObject.Find("DifficultyText").GetComponent<Text>().text = hardText;
        }
    }

    public void changeFilter(string filterType)
    {
        if (filterType == "AllItems")
        {
            BattleAxe.SetActive(true);
            DarkBow.SetActive(true);
            SkullAxe.SetActive(true);
            Dagger.SetActive(true);

            KnightHelmet.SetActive(true);
            VikingHelmet.SetActive(true);
            ThiefHood.SetActive(true);
            KnightArmor.SetActive(true);
            VikingArmor.SetActive(true);
            ThiefBoots.SetActive(true);
        }

        else if (filterType == "WeaponsOnly")
        {
            BattleAxe.SetActive(true);
            DarkBow.SetActive(true);
            SkullAxe.SetActive(true);
            Dagger.SetActive(true);

            KnightHelmet.SetActive(false);
            VikingHelmet.SetActive(false);
            ThiefHood.SetActive(false);
            KnightArmor.SetActive(false);
            VikingArmor.SetActive(false);
            ThiefBoots.SetActive(false);
        }

        else if (filterType == "ArmorOnly")
        {
            BattleAxe.SetActive(false);
            DarkBow.SetActive(false);
            SkullAxe.SetActive(false);
            Dagger.SetActive(false);

            KnightHelmet.SetActive(true);
            VikingHelmet.SetActive(true);
            ThiefHood.SetActive(true);
            KnightArmor.SetActive(true);
            VikingArmor.SetActive(true);
            ThiefBoots.SetActive(true);
        }
    }

    public void shopFilter(string filterType)
    {
        if (filterType == "AllItems")
        {
            GlassBow.SetActive(true);
            WolfBlade.SetActive(true);
            StoneBlade.SetActive(true);
            BattleAxe.SetActive(true);
            DarkBow.SetActive(true);

            KnightHelmet.SetActive(true);
            VikingHelmet.SetActive(true);
            ThiefArmor.SetActive(true);
            ThiefHood.SetActive(true);
            KnightArmor.SetActive(true);
            ThiefBoots.SetActive(true);
            CommonBoots.SetActive(true);
        }

        else if (filterType == "WeaponsOnly")
        {
            GlassBow.SetActive(true);
            WolfBlade.SetActive(true);
            StoneBlade.SetActive(true);
            BattleAxe.SetActive(true);
            DarkBow.SetActive(true);

            KnightHelmet.SetActive(false);
            VikingHelmet.SetActive(false);
            ThiefArmor.SetActive(false);
            ThiefHood.SetActive(false);
            KnightArmor.SetActive(false);
            ThiefBoots.SetActive(false);
            CommonBoots.SetActive(false);
        }

        else if (filterType == "ArmorOnly")
        {
            GlassBow.SetActive(false);
            WolfBlade.SetActive(false);
            StoneBlade.SetActive(false);
            BattleAxe.SetActive(false);
            DarkBow.SetActive(false);

            KnightHelmet.SetActive(true);
            VikingHelmet.SetActive(true);
            ThiefArmor.SetActive(true);
            ThiefHood.SetActive(true);
            KnightArmor.SetActive(true);
            ThiefBoots.SetActive(true);
            CommonBoots.SetActive(true);
        }
    }

    public void displayLegend(string legendGO)
    {
        GameObject.Find(legendGO).GetComponent<Image>().enabled = !GameObject.Find(legendGO).GetComponent<Image>().enabled;
    }

    public void tapAnywhere(string tap)
    {
        GameObject.Find(tap).GetComponent<Image>().enabled = false;
    }
}
