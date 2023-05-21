using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class UpgradeShopIconBase : MonoBehaviour
{
   // public string Description;
    public int UpgradeFunctionIndex =1;
    public int CharacterIndex =1;
    public Image UpgradeIconImage;
    public Image BackgroundImage;
    public TMP_Text DescriptionText;
    public TMP_Text CostText;

    public ShellStackGameManager GameManager;

    public int Maxcost = 30;
    public int Mincost = 10;
    public int ActualCost =0;

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



    public void SetUpIcon(Sprite icon, int Animal, int Power, string text)
    {
        //Debug.Log("Description: " + text);
        gameObject.SetActive(true);
        
        UpgradeIconImage.sprite = icon;
  
        CharacterIndex = Animal;
        UpgradeFunctionIndex = Power;
        ActualCost = Random.Range(Mincost, Maxcost);

        switch (Power)
        {
            case 1:
                BackgroundImage.color = Color.green;
                break;
            case 2:
                BackgroundImage.color = Color.cyan;
                break;
            case 3:
                BackgroundImage.color = Color.magenta;
                break;
        }

        CostText.text = ActualCost.ToString();
        DescriptionText.SetText(text);

    }


    public void OnBuyClick()
    {
       if(GameManager.SnailsCount >= ActualCost)
       {
            GameManager.SnailsCount -= ActualCost;
            GameManager.PowerUp(CharacterIndex, UpgradeFunctionIndex);
            gameObject.SetActive(false);
       }

    }
}
