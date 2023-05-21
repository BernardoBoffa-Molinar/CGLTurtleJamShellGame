using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public GameObject PlayerGO;
    public bool PlayerIsInRange;
    public bool StoreOpen;
    public ShellStackGameManager GameManager;
    public GameObject ShopBackground;
    public List<Sprite> CharacterImages;
    
    public GameObject[] BuyOptionsArray;
    public UpgradeShopIconBase BuyOption1;
    public UpgradeShopIconBase BuyOption2;
    public UpgradeShopIconBase BuyOption3;

    // Start is called before the first frame update
    void Awake()
    {
        PlayerGO = GameObject.FindGameObjectWithTag("Player");
        GameObject[] Temp = GameObject.FindGameObjectsWithTag("UIShopIcon");
        Debug.Log("Temp"+Temp.Length);
        BuyOptionsArray = Temp;
        Debug.Log("options "+BuyOptionsArray.Length);
      
      
        GameManager = FindObjectOfType<ShellStackGameManager>();
        CreateNewShop();
        CreateNewShop();
        CreateNewShop();
    }

    // Update is called once per frame
    void Update()
    {
        ShopBackground.SetActive(PlayerIsInRange);


        if (Input.GetKey(KeyCode.Space) ){

            if (!GameManager.IsPaused && !StoreOpen)
            {

                //GameManager.TogglePause();
                StoreOpen = true;
                GameManager.ShopOpen = StoreOpen;
                GameManager.ShopMenu.SetActive(StoreOpen);
            }
            else
            {
                //GameManager.TogglePause();
                StoreOpen = false;
                GameManager.ShopOpen = StoreOpen;
                GameManager.ShopMenu.SetActive(StoreOpen);
            }
        }




    }



    public void CreateNewShop()
    {
        for(int i = 0; i< BuyOptionsArray.Length; i++)
        {
            int AnimalToSet = Random.Range(1, 4);
            int UpgradeToSet = Random.Range(1, 4);
            Debug.Log("Animal :" + AnimalToSet + " Power:" + UpgradeToSet);
            string Des = GetDefinitionFromUpgradeIndexs(AnimalToSet, UpgradeToSet);
            Debug.Log("Description: " + Des);
            Sprite img = GetImgByAnimal(AnimalToSet);

            Debug.Log(img.name);
            
            BuyOptionsArray[i].GetComponent<UpgradeShopIconBase>().SetUpIcon(img, AnimalToSet, UpgradeToSet, Des);
        }

    }

    Sprite GetImgByAnimal(int Animal)
    {
        return CharacterImages[Animal - 1];
    }


    public string GetDefinitionFromUpgradeIndexs(int AnimalID, int UpgradeId)
    {
        string Description ="";
       

        switch (AnimalID)
        {
            //TURTLE
            case 1:
                switch (UpgradeId)
                {
                    case 1:

                        Description = "More Health";
                       
                        break;
                    case 2:
                        Description = "More Speed";
                       
                        break;
                    case 3:
                        Description = "More Damage";
                        break;
                }
                break;
            //CRAB
            case 2:
                switch (UpgradeId)
                {
                    case 1:
                        Description = "More AOE";
                       
                        break;
                    case 2:
                        Description = "More Speed";
                       
                        break;
                    case 3:
                        Description = "More Damage";
                        break;
                }

                break;
            //BIRD
            case 3:
                switch (UpgradeId)
                {
                    case 1:
                        Description = "More Fire Rate";
                        
                        break;
                    case 2:
                        Description = "More Egg Speed";
                   
                        break;
                    case 3:
                        Description = "More Egg Damage";
                        break;
                }
                break;
        }
        return Description;
    }

    UpgradeShopIconBase CreateBuyOption()
    {
        UpgradeShopIconBase temp = new UpgradeShopIconBase();


        return temp;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayersStackController>() || collision.gameObject.GetComponent<ShellStackGameManager>())
        {
            if (PlayerGO == null)
            {
                PlayerGO = collision.gameObject;
            }
            PlayerIsInRange = true;


        }

    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        
        if(collision.gameObject == PlayerGO)
        {
            PlayerIsInRange = false;
        }



    }

}
