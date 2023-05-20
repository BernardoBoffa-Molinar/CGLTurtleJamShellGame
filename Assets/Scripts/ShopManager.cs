using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public ShellStackGameManager GameManager;

    public List<Image> CharacterImages;
    public List<GameObject> BuyOptions;
    public UpgradeShopIconBase BuyOption1;
    public UpgradeShopIconBase BuyOption2;
    public UpgradeShopIconBase BuyOption3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    void CreateNewShop()
    {
        BuyOption1 = CreateBuyOption();



    }

    UpgradeShopIconBase CreateBuyOption()
    {
        UpgradeShopIconBase temp = new UpgradeShopIconBase();


        return temp;
    }


}
