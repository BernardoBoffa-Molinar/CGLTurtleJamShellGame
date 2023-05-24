using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{

    private static ShopManager instance;
    public GameObject PlayerGO;
    public PlayersStackController PlayerSController;
    public HealhSystemInterface PlayerHP;
    public bool PlayerIsInRange = false;
    public bool StoreOpen =false;
    public ShellStackGameManager GameManager;
    public GameObject ShopBackground;
    public List<Sprite> CharacterImages;
    
    public GameObject[] BuyOptionsArray;
    public GameObject ShopUIMenu;
    public float distance = 0;
    // Start is called before the first frame update
    void Start()
    {
        // Check if an instance already exists
        if (instance == null)
        {
            // Set the instance to this object
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (PlayerGO == null)
            {
                PlayerGO = FindObjectOfType<ShellStackGameManager>().gameObject;
            }

            if (PlayerSController == null)
            {
                PlayerSController = PlayerGO.GetComponent<PlayersStackController>();

            }

            if (PlayerHP == null)
            {
                PlayerHP = PlayerGO.GetComponent<HealhSystemInterface>();
            }

            //BuyOptionsArray = PlayerGO.GetComponent<ShellStackGameManager>().PlayerBuyOptionsUI;
            Debug.Log("PlayerUI Buttons" + BuyOptionsArray.Length);
           

            PlayerIsInRange = false;
            StoreOpen = false;
            Debug.Log("options " + BuyOptionsArray.Length);
            GameManager = PlayerGO.GetComponent<ShellStackGameManager>();
            //ShopUIMenu = gameObject.GetComponentInChildren<Canvas>().gameObject;
            ShopUIMenu.SetActive(StoreOpen);
            CreateNewShop();
        }
        else
        {
            // Destroy this object if another instance exists
            Destroy(gameObject);
        }

        
      
    }

    // Update is called once per frame
    void Update()
    {
      if(GameManager == null)
        {
            GameManager = FindObjectOfType<ShellStackGameManager>();
        }

        if (!GameManager.ShellGameOver)
        {
            distance = Vector3.Distance(GameManager.transform.position, gameObject.transform.position); 

            ShopBackground.SetActive(distance < 10);
            if (Input.GetKeyDown(KeyCode.Space) && distance< 10)
            {


                if (!GameManager.IsPaused && !StoreOpen)
                {

                    //GameManager.TogglePause();
                    StoreOpen = true;
                    GameManager.PlaySoundInManager("ShopSFX");
                    GameManager.ShopOpen = StoreOpen;
                    GameManager.ShopMenu.SetActive(StoreOpen);
                    ShopUIMenu.SetActive(StoreOpen);
                 
                }
                else
                {
                    //GameManager.TogglePause();
                    StoreOpen = false;
                    GameManager.ShopOpen = StoreOpen;
                    GameManager.ShopMenu.SetActive(StoreOpen);
                    ShopUIMenu.SetActive(StoreOpen);
                }
            }
        }
       




    }

    public  void CreateBuyOption(UpgradeShopIconBase Base)
    {
        int AnimalToSet = Random.Range(1, 4);
        int UpgradeToSet = Random.Range(1, 4);
        //Debug.Log("Animal :" + AnimalToSet + " Power:" + UpgradeToSet);
        string Des = GetDefinitionFromUpgradeIndexs(AnimalToSet, UpgradeToSet);
        //Debug.Log("Description: " + Des);
        Sprite img = GetImgByAnimal(AnimalToSet);
        Base.SetUpIcon(img, AnimalToSet, UpgradeToSet, GetDefinitionFromUpgradeIndexs(AnimalToSet, UpgradeToSet));

    }


    public void CreateNewShop()
    {
        //Debug.Log("Total:"+BuyOptionsArray.Length);

        for(int i = 0; i< BuyOptionsArray.Length; i++)
        {
            if (BuyOptionsArray[i].GetComponent<UpgradeShopIconBase>())
            {
                CreateBuyOption(BuyOptionsArray[i].GetComponent<UpgradeShopIconBase>());
            }
        }

       GameManager.UpdateTopUi();
    }

    Sprite GetImgByAnimal(int Animal)
    {
        return CharacterImages[Animal - 1];
    }

    public void RerollShop()
    {
        if(GameManager.SnailsCount - 10 > 0)
        {
            GameManager.SnailsCount -= 10;
            CreateNewShop();
        }
       
    }


    public void BuyTime()
    {
        if (GameManager.SnailsCount - 10 > 0)
        {
            GameManager.SnailsCount -= 10;
            GameManager.GetMoreTime(30f);
        }
    }



    public string GetDefinitionFromUpgradeIndexs(int AnimalID, int UpgradeId)
    {
        string Description ="";
        string Extra ="";
  
        float Ftemp = 0;

        if(PlayerGO == null)
        {
            PlayerGO = FindObjectOfType<ShellStackGameManager>().gameObject;
        }

        if (PlayerSController == null) {
            PlayerSController = PlayerGO.GetComponent<PlayersStackController>(); 
        
        }
        
        if(PlayerHP == null)
        {
            PlayerHP = PlayerGO.GetComponent<HealhSystemInterface>();
        }
        

        switch (AnimalID)
        {
            //TURTLE
            case 1:
                switch (UpgradeId)
                {
                    case 1:
                        Ftemp = PlayerHP.maxHealth;
                        Extra = Ftemp + " => "+(Ftemp + 10);
                        Description = "More Health\n" +Extra;
                       
                        break;
                    case 2:
                        Ftemp = PlayerSController.TurtleMovementBaseSpeed;
                        Extra = Ftemp + " => " + (Ftemp + 5.0f);
                        Description = "More Speed\n" + Extra;

                        break;
                    case 3:
                        Ftemp = PlayerSController.TurtleDamage;
                        Extra = Ftemp.ToString("F0") + " => " + (Ftemp + 5.0f).ToString("F0");
                        Description = "More Damage\n" + Extra;
                        break;
                }
                break;
            //CRAB
            case 2:
                switch (UpgradeId)
                {
                    case 1:
                        Ftemp = PlayerSController.CrabArea;
                        Extra = Ftemp.ToString("F1") + "x => " + (Ftemp + 0.25f).ToString("F1")+"x";
                        Description = "More AOE\n"+Extra;
                        break;
                    case 2:
                        Ftemp = PlayerSController.CrabRotationSpeed;
                        Extra = Ftemp.ToString("F0") + " => " + (Ftemp + 5.0f).ToString("F0");
                        Description = "More Speed\n" + Extra;
                        break;
                    case 3:
                        Ftemp = PlayerSController.CrabDamage;
                        Extra = Ftemp.ToString("F0") + " => " + (Ftemp + 5.0f).ToString("F0");
                        Description = "More Damage\n" + Extra;
                        break;
                }

                break;
            //BIRD
            case 3:
                switch (UpgradeId)
                {
                    case 1:
                        Ftemp = PlayerSController.shootCooldown;
                        Extra = Ftemp.ToString("F1") + "s => " + (Ftemp * 0.9f).ToString("F1") +"s";
                        Description = "More Fire Rate\n" + Extra;

                        break;
                    case 2:
                        Ftemp = PlayerSController.BirdThrowSpeed;
                        Extra = Ftemp.ToString("F0") + " => " + (Ftemp + 5.0f).ToString("F0");
                        Description = "More Egg Speed\n" + Extra;
                   
                        break;
                    case 3:
                        Ftemp = PlayerSController.BirdDamage;
                        Extra = Ftemp.ToString("F0") + " => " + (Ftemp + 5.0f).ToString("F0");
                        Description = "More Egg Damage\n" + Extra;
                        break;
                }
                break;
        }
        return Description;
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == PlayerGO)
        {
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
