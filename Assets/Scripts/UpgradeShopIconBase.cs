using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum PlayerCharacters
{
    Turtle,
    Crab,
    Bird
}


public enum TurtleUpgrades
{
    Health, Speed, Damage
}

public enum CrabUpgrades
{
    AreaScale, RotationSpeed, Damage
}

public enum BirdUpgrades
{
    AttackSpeed, ThrowSpeed, Damage
}



public class UpgradeShopIconBase : MonoBehaviour
{
    public string Description;
    public int UpgradeFunctionIndex;
    public int CharacterIndex;
    public Image UpgradeIconImage;
    public TMP_Text DescriptionText;
    public TMP_Text NameText;

    public ShellStackGameManager GameManager;

    public int Maxcost = 50;
    public int Mincost = 20;
    public int ActualCost;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = FindObjectOfType<ShellStackGameManager>();
        ActualCost = Random.Range(Mincost, Maxcost);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnBuyClick()
    {
       if(GameManager.SnailsCount >= ActualCost)
        {
            GameManager.SnailsCount -= ActualCost;
            GameManager.PowerUp(CharacterIndex, UpgradeFunctionIndex);

        }

    }
}
