using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryManager : MonoBehaviour {
    public bool startHidden = true;
    public float disappearSize = 0.1f;
    public float popSpeed = 1.0f;

    private bool shouldShow;
    private RectTransform recty;


    public string itemName, itemType; 
    public int itemAttack, itemDefence, itemPower, itemPrice;
    private int currentAttack, currentDefence, currentPower;

    

    public Text debugText;


	// Use this for initialization
	void Start () {
        currentAttack = 360;
        currentDefence = 180;
        currentPower = 80;

        recty = GetComponentInParent<RectTransform>();

        if (startHidden)
        {
            shouldShow = true;
            gameObject.SetActive(true);
            recty.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            shouldShow = false;
            gameObject.SetActive(false);
            recty.localScale = new Vector3(disappearSize, disappearSize, 1);
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        //float dt = Time.deltaTime;

        //if (shouldShow)
        //{
        //    if (recty.localScale.x < 1)
        //    {
        //        Vector3 nextSize = recty.localScale + new Vector3(popSpeed * dt, popSpeed * dt, 0);
        //        if (nextSize.x > 1)
        //        {
        //            nextSize.x = 1;
        //            nextSize.y = 1;
        //        }

        //        recty.localScale = nextSize;
        //    }
        //}
        //else
        //{
        //    //if more than de-spawn size, scale down
        //    if (recty.localScale.x > disappearSize)
        //    {
        //        Vector3 nextSize = recty.localScale - new Vector3(popSpeed * dt, popSpeed * dt, 0);
        //        if (nextSize.x < disappearSize)
        //        {
        //            nextSize.x = disappearSize;
        //            nextSize.y = disappearSize;
        //        }

        //        recty.localScale = nextSize;
        //    }
        //    else
        //    {
        //        gameObject.SetActive(false);
        //    }
        //}
	}

    public void Enter()
    {
        gameObject.SetActive(true);
        shouldShow = true;
    }

    public void Exit()
    {
        shouldShow = false;
    }

    public void Toggle()
    {
        shouldShow = !shouldShow;
        if (shouldShow)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }

    public void displayItemStats(string stats)
    {
        GameObject.Find("BattleAxe").GetComponent<Text>().text = itemAttack + "\n" + itemDefence + "\n" + itemPower;
    }

    public void GetStats()
    {
        if (!GameObject.Find("selectedItem"))
        {
            GameObject selectedItem = Instantiate(gameObject);
            selectedItem.name = "selectedItem";
        }
        else
        {
            Destroy(GameObject.Find("selectedItem"));
            GameObject selectedItem = Instantiate(gameObject);
            selectedItem.name = "selectedItem";
        }

        GameObject.Find("ItemStatsBox").transform.Find("ItemStatsDisplay").GetComponent<Text>().text = itemAttack + "\n" + itemDefence + "\n" + itemPower;
          
    }

    public void GetShopStats()
    {
         GameObject.Find("ItemStatsBox").transform.Find("ItemStatsDisplay").GetComponent<Text>().text = itemAttack + "\n" + itemDefence + "\n" + itemPower + "\n" + itemPrice;
    }

    public void Equip()
    {
        InventoryManager tempInventory = new InventoryManager();

        Sprite tempSprite = new Sprite();

        //debugText.text = GameObject.Find("EquippedPrimary").GetComponent<Image>().sprite.name;

        if (GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemType == "PrimaryWeapon")
        {
            tempSprite = GameObject.Find("EquippedPrimary").GetComponent<Image>().sprite;
            tempInventory.itemName = GameObject.Find("EquippedPrimary").GetComponent<InventoryManager>().itemName;
            tempInventory.itemType = GameObject.Find("EquippedPrimary").GetComponent<InventoryManager>().itemType;
            tempInventory.itemAttack = GameObject.Find("EquippedPrimary").GetComponent<InventoryManager>().itemAttack;
            tempInventory.itemDefence = GameObject.Find("EquippedPrimary").GetComponent<InventoryManager>().itemDefence;
            tempInventory.itemPower = GameObject.Find("EquippedPrimary").GetComponent<InventoryManager>().itemPower;

            GameObject.Find("EquippedPrimary").GetComponent<InventoryManager>().itemName = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemName;
            GameObject.Find("EquippedPrimary").GetComponent<InventoryManager>().itemType = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemType;
            GameObject.Find("EquippedPrimary").GetComponent<InventoryManager>().itemAttack = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemAttack;
            GameObject.Find("EquippedPrimary").GetComponent<InventoryManager>().itemDefence = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemDefence;
            GameObject.Find("EquippedPrimary").GetComponent<InventoryManager>().itemPower = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemPower;
            GameObject.Find("EquippedPrimary").GetComponent<Image>().sprite = GameObject.Find("selectedItem").GetComponent<Image>().sprite;
        }

        else if (GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemType == "SecondaryWeapon")
        {
            tempSprite = GameObject.Find("EquippedSecondary").GetComponent<Image>().sprite;
            tempInventory.itemName = GameObject.Find("EquippedSecondary").GetComponent<InventoryManager>().itemName;
            tempInventory.itemType = GameObject.Find("EquippedSecondary").GetComponent<InventoryManager>().itemType;
            tempInventory.itemAttack = GameObject.Find("EquippedSecondary").GetComponent<InventoryManager>().itemAttack;
            tempInventory.itemDefence = GameObject.Find("EquippedSecondary").GetComponent<InventoryManager>().itemDefence;
            tempInventory.itemPower = GameObject.Find("EquippedSecondary").GetComponent<InventoryManager>().itemPower;


            GameObject.Find("EquippedSecondary").GetComponent<InventoryManager>().itemName = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemName;
            GameObject.Find("EquippedSecondary").GetComponent<InventoryManager>().itemType = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemType;
            GameObject.Find("EquippedSecondary").GetComponent<InventoryManager>().itemAttack = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemAttack;
            GameObject.Find("EquippedSecondary").GetComponent<InventoryManager>().itemDefence = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemDefence;
            GameObject.Find("EquippedSecondary").GetComponent<InventoryManager>().itemPower = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemPower;
            GameObject.Find("EquippedSecondary").GetComponent<Image>().sprite = GameObject.Find("selectedItem").GetComponent<Image>().sprite;
        }

        else if (GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemType == "HeadArmor")
        {
            tempSprite = GameObject.Find("EquippedHelmet").GetComponent<Image>().sprite;
            tempInventory.itemName = GameObject.Find("EquippedHelmet").GetComponent<InventoryManager>().itemName;
            tempInventory.itemType = GameObject.Find("EquippedHelmet").GetComponent<InventoryManager>().itemType;
            tempInventory.itemAttack = GameObject.Find("EquippedHelmet").GetComponent<InventoryManager>().itemAttack;
            tempInventory.itemDefence = GameObject.Find("EquippedHelmet").GetComponent<InventoryManager>().itemDefence;
            tempInventory.itemPower = GameObject.Find("EquippedHelmet").GetComponent<InventoryManager>().itemPower;


            GameObject.Find("EquippedHelmet").GetComponent<InventoryManager>().itemName = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemName;
            GameObject.Find("EquippedHelmet").GetComponent<InventoryManager>().itemType = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemType;
            GameObject.Find("EquippedHelmet").GetComponent<InventoryManager>().itemAttack = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemAttack;
            GameObject.Find("EquippedHelmet").GetComponent<InventoryManager>().itemDefence = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemDefence;
            GameObject.Find("EquippedHelmet").GetComponent<InventoryManager>().itemPower = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemPower;
            GameObject.Find("EquippedHelmet").GetComponent<Image>().sprite = GameObject.Find("selectedItem").GetComponent<Image>().sprite;
        }

        else if (GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemType == "BodyArmor")
        {
            tempSprite = GameObject.Find("EquippedArmor").GetComponent<Image>().sprite;
            tempInventory.itemName = GameObject.Find("EquippedArmor").GetComponent<InventoryManager>().itemName;
            tempInventory.itemType = GameObject.Find("EquippedArmor").GetComponent<InventoryManager>().itemType;
            tempInventory.itemAttack = GameObject.Find("EquippedArmor").GetComponent<InventoryManager>().itemAttack;
            tempInventory.itemDefence = GameObject.Find("EquippedArmor").GetComponent<InventoryManager>().itemDefence;
            tempInventory.itemPower = GameObject.Find("EquippedArmor").GetComponent<InventoryManager>().itemPower;


            GameObject.Find("EquippedArmor").GetComponent<InventoryManager>().itemName = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemName;
            GameObject.Find("EquippedArmor").GetComponent<InventoryManager>().itemType = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemType;
            GameObject.Find("EquippedArmor").GetComponent<InventoryManager>().itemAttack = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemAttack;
            GameObject.Find("EquippedArmor").GetComponent<InventoryManager>().itemDefence = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemDefence;
            GameObject.Find("EquippedArmor").GetComponent<InventoryManager>().itemPower = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemPower;
            GameObject.Find("EquippedArmor").GetComponent<Image>().sprite = GameObject.Find("selectedItem").GetComponent<Image>().sprite;
        }

        else if (GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemType == "LegArmor")
        {
            tempSprite = GameObject.Find("EquippedBoots").GetComponent<Image>().sprite;
            tempInventory.itemName = GameObject.Find("EquippedBoots").GetComponent<InventoryManager>().itemName;
            tempInventory.itemType = GameObject.Find("EquippedBoots").GetComponent<InventoryManager>().itemType;
            tempInventory.itemAttack = GameObject.Find("EquippedBoots").GetComponent<InventoryManager>().itemAttack;
            tempInventory.itemDefence = GameObject.Find("EquippedBoots").GetComponent<InventoryManager>().itemDefence;
            tempInventory.itemPower = GameObject.Find("EquippedBoots").GetComponent<InventoryManager>().itemPower;

            GameObject.Find("EquippedBoots").GetComponent<InventoryManager>().itemName = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemName;
            GameObject.Find("EquippedBoots").GetComponent<InventoryManager>().itemType = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemType;
            GameObject.Find("EquippedBoots").GetComponent<InventoryManager>().itemAttack = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemAttack;
            GameObject.Find("EquippedBoots").GetComponent<InventoryManager>().itemDefence = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemDefence;
            GameObject.Find("EquippedBoots").GetComponent<InventoryManager>().itemPower = GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemPower;
            GameObject.Find("EquippedBoots").GetComponent<Image>().sprite = GameObject.Find("selectedItem").GetComponent<Image>().sprite;
        }

        GameObject.Find("ItemsBack").transform.Find("ItemsFrontAll").transform.Find(GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemName).GetComponent<InventoryManager>().itemAttack = tempInventory.itemAttack;
        GameObject.Find("ItemsBack").transform.Find("ItemsFrontAll").transform.Find(GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemName).GetComponent<InventoryManager>().itemDefence = tempInventory.itemDefence;
        GameObject.Find("ItemsBack").transform.Find("ItemsFrontAll").transform.Find(GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemName).GetComponent<InventoryManager>().itemPower = tempInventory.itemPower;
        GameObject.Find("ItemsBack").transform.Find("ItemsFrontAll").transform.Find(GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemName).GetComponent<InventoryManager>().itemType = tempInventory.itemType;
        GameObject.Find("ItemsBack").transform.Find("ItemsFrontAll").transform.Find(GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemName).GetComponent<Image>().sprite = tempSprite;
        GameObject.Find("ItemsBack").transform.Find("ItemsFrontAll").transform.Find(GameObject.Find("selectedItem").GetComponent<InventoryManager>().itemName).GetComponent<InventoryManager>().itemName = tempInventory.itemName;

    }

}
