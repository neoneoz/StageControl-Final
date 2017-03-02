using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour { // Can be singleton

    public float Gold;
    public float GoldIncomeRate = 1;
    public Text goldtext; 
	// Use this for initialization
	void Start () {
        Gold = 500;
        goldtext.text = ((int)Gold).ToString();
	}
	
	// Update is called once per frame
	void Update ()
    {
        Gold += GoldIncomeRate * Time.deltaTime;
        goldtext.text = ((int)Gold).ToString();
	}

    public bool AddPlayerGold(int amount)
    {
        if (goldtext)
        {
            Gold += amount;
            goldtext.text = ((int)Gold).ToString();
        }
        else
            return false;
        return true;
    }

    public bool SpendPlayerGold(int amount)
    {
        if(Gold < amount)
            return false;

        Gold -= amount;
        goldtext.text = Gold.ToString();

        return true;
    }

    public bool CheckPlayerGold(int amount)
    {
        if (Gold < amount)
            return false;

        return true;


    }

    public int GetGold()
    {
        return (int)Gold;
    }
}
