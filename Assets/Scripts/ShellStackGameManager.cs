using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ShellStackGameManager : MonoBehaviour
{
    public GameObject GroundGO;
    public Material[] GroundMaterials;
    public float StageDifficulty = 1;
    public int StageLevel =1;
    public int StageResources = 0;
    public int SnailsCount = 100;
    private static ShellStackGameManager instance;
    public float GameTimer = 600f;
    public bool IsPaused;
    public bool ShellGameOver;
    public bool PlayerIsDeath;
    public PlayersStackController PlayerController;
    public HealhSystemInterface HealhSystem;

    public AudioManager AudioManager;


    //UI Objects
    public TMP_Text timerText;
    public TMP_Text levelText;

    public TMP_Text SnaillScoreText;
    public TMP_Text StageResourcesScoreText;
    public TMP_Text HPText;
    public Slider HPSlider;
    public GameObject PauseMenu;

    //Gameover 
    public GameObject GameOverMenu;
    bool Winner = false;
    public TMP_Text GameOverMessage;

    //EnemySpawener Controll
    [SerializeField]
    public List<GameObject> EnemiesPrefab;
    public float SpawnDistance = 50f;
    public float SpawnTimer =0f;
    public float SpawnColdown = 5f;
    public int EnemiesSpawnAmmount = 2;

    //EnemySpawener Controll
    [SerializeField]
    public List<GameObject> CollectablesPrefab;

    public float CollectSpawnMaxDistance = 50f;
    public float CollectSpawnMinDistance = 25f;
    public float CollectSpawnTimer = 0f;
    public float CollectSpawnColdown = 10f;
    public int CollectableStageIndex = 0;
    public int CollectablesAmmounts = 3;

    //Shop Objects
    public ShopManager SMScript;
    public GameObject ShopMenu;
    public GameObject ShopGameObject;
    public bool ShopOpen;
    public float ShopTimer = 0f;
    public float ShopDuration = 30f;
    public float ShopCooldown = 30f;
    public Vector3 ShopSpawnPosition;



    public void ReloadGame()
    {
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Destroy(SMScript.gameObject);
        Destroy(gameObject);
    }

    public void GotoMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
         Destroy(SMScript.gameObject);
        Destroy(gameObject);
    }

    void MoveShopPosition()
    {
        ShopTimer = 0f;
        ShopGameObject.SetActive(true);
        float ToSpawnDirX = Random.Range(-1.0f, 1.0f);
        float ToSpawnDirY = Random.Range(-1.0f, 1.0f);
        float distance = 30f;
        ShopSpawnPosition = gameObject.transform.position + new Vector3(ToSpawnDirX, ToSpawnDirY, 0).normalized * distance + Vector3.forward;
        ShopGameObject.transform.position = ShopSpawnPosition;
        SMScript.CreateNewShop();

    }

    void ShopController()
    {

      

        ShopTimer += Time.deltaTime;
        if(ShopTimer < ShopDuration)
        {
            ShopGameObject.SetActive(true);
        }
        else
        {

            ShopGameObject.SetActive(false);

            if(ShopTimer > ShopDuration + ShopCooldown)
            {
                MoveShopPosition();
            }

        }
    }

    void SpawnCollectables()
    {
        for (int i = 0; i < CollectablesAmmounts; i++)
        {
            int CollectProbably = Random.Range(0, 100);
            int CollectToSpawn = 0;
            if (CollectProbably > 90)
            {
                CollectToSpawn = CollectablesPrefab.Count-1;
            }
            else
            {
                CollectToSpawn = Random.Range(0, CollectablesPrefab.Count - 1);

            }
                     
            float ToSpawnDirX = Random.Range(-1.0f, 1.0f);
            float ToSpawnDirY = Random.Range(-1.0f, 1.0f);
            float distance = Random.Range(CollectSpawnMinDistance, CollectSpawnMaxDistance);
            Vector3 SpawnPosition = gameObject.transform.position + new Vector3(ToSpawnDirX, ToSpawnDirY, 0).normalized * distance + Vector3.forward *2f;
            Instantiate(CollectablesPrefab[CollectToSpawn], SpawnPosition, Quaternion.identity);

        }
    }

    void CollectablesSpawnnerLogic()
    {
        CollectSpawnTimer += Time.deltaTime;
        if(CollectSpawnTimer>= CollectSpawnColdown)
        {

            SpawnCollectables();
            CollectSpawnTimer = 0f;
        }


    }
    
    
    
    void StageControll()
    {
        if (StageResources >= 50)
        {
            StageLevel++;
            StageResources = 0;
            if (GroundGO)
            {
                GroundGO.transform.position = gameObject.transform.position + Vector3.back;
                GroundGO.GetComponent<Renderer>().material = GroundMaterials[StageLevel - 1];

            }
            else
            {
                GroundGO =  GameObject.FindGameObjectWithTag("LevelGround");
               GroundGO.transform.position = gameObject.transform.position + Vector3.back;
                GroundGO.GetComponent<Renderer>().material = GroundMaterials[StageLevel - 1];

            }
            StageDifficulty += 0.25f;
            if(StageLevel ==GroundMaterials.Length +1)
            {
                Winner = true;
                WinGame();
            }
        }
    }

    public void WinGame()
    {
        UpdateTopUi();
        ShellGameOver = true;

        if (Winner)
        {
            GameOverMessage.text = "You Win :)";
        }
        else
        {
            GameOverMessage.text = "You Lose, Try Again :)";
        }

    }

    void EnemiesSpawnerLogic()
    {
        SpawnTimer += Time.deltaTime;
        if(SpawnTimer>= SpawnColdown)
        {
            SpawnEnemyWave();
            SpawnTimer = 0f;
        }

    }

    void SpawnEnemyWave()
    {
        for(int i = 0; i < EnemiesSpawnAmmount; i++)
        {
            
            int EnemyToSpawn = Random.Range(0, EnemiesPrefab.Count);
            float EnemyToSpawnDirX = Random.Range(-1.0f, 1.0f);
            float EnemyToSpawnDirY = Random.Range(-1.0f, 1.0f);
            Vector3 SpawnPosition = gameObject.transform.position + new Vector3(EnemyToSpawnDirX, EnemyToSpawnDirY, 0).normalized * SpawnDistance;
            Instantiate(EnemiesPrefab[EnemyToSpawn], SpawnPosition, Quaternion.identity);

        }


    }

    private void Awake()
    {
        // Check if an instance already exists
        if (instance == null)
        {
            // Set the instance to this object
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy this object if another instance exists
            Destroy(gameObject);
        }
    }

    public void UpdateTopUi()
    {
        timerText.text = "Timer: " + GameTimer.ToString("F0");
        levelText.text = "Level " + StageLevel.ToString();

        SnaillScoreText.text = SnailsCount.ToString();
        StageResourcesScoreText.text = StageResources.ToString("F0") + "/50";
        HPText.text = "HP: " + HealhSystem.currentHealth + " / " + HealhSystem.maxHealth;
        HPSlider.value =  (float)HealhSystem.currentHealth / (float)HealhSystem.maxHealth;
    }

    // Example method
    public void StartGame()
    {
        // Logic for starting the game
        // ...
        GameOverMenu.SetActive(false);
        ShopMenu.SetActive(false);
        PauseMenu.SetActive( false);
        HPSlider.value = HealhSystem.currentHealth / HealhSystem.maxHealth;
        ShellGameOver = false;
        GroundGO = GameObject.FindGameObjectWithTag("LevelGround");
        gameObject.transform.position = Vector3.zero + Vector3.forward;
        StageDifficulty = 1f;
        SnailsCount = 0;
       // StageResources = 0;
        IsPaused = false;
        ShopOpen = false;
        AudioManager = FindObjectOfType<AudioManager>();
        PlaySoundInManager("MainMusic");
        PlaySoundInManager("WavesSFX");


        levelText.text = "Level " + 1 + (StageDifficulty-1)/0.25f;
        PlayerController = GetComponent<PlayersStackController>();
        HealhSystem = GetComponent<HealhSystemInterface>();
        SMScript = FindObjectOfType<ShopManager>();
        ShopGameObject = SMScript.gameObject;
        if (PlayerController == null)
        {
            Debug.LogError("PlayerController component not found!");
        }
        SpawnCollectables();
        SpawnEnemyWave();
    }

    // Example method
    public void EndGame()
    {
        // Logic for ending the game    
        // ...
        GameOverMenu.SetActive(true);
        ShopMenu.SetActive(false);
        PauseMenu.SetActive(false);
    }

   

    public void PlaySoundInManager(string Sname)
    {
        if (AudioManager)
        {
            AudioManager.PlaySound(Sname);
        }
       
    }


// Start is called before the first frame update
    void Start()
    {
        StartGame();

    }

    // Update is called once per frame
    void Update()
    {

        // game in play
        if (!IsPaused && !ShellGameOver && !ShopOpen)
        {
            GameTimer -= Time.deltaTime;
            if(GameTimer <= 0  || PlayerIsDeath)
            {
                ShellGameOver = true;
                WinGame();
            }

            EnemiesSpawnerLogic();

            UpdateTopUi();
            CollectablesSpawnnerLogic();
            ShopController();

            StageControll();
        }

        if (IsPaused) {
            if (ShopOpen)
            {
                ShopMenu.SetActive(ShopOpen);
                PauseMenu.SetActive(false);
            }
            else
            {
                ShopMenu.SetActive(false);
                PauseMenu.SetActive(true);
            }
        }

    
 
        
        

        if (Input.GetKeyDown(KeyCode.Escape) && !ShopOpen)
        {
            TogglePause();
        }
        


        if (ShellGameOver)
        {
            EndGame();
        }

        
    }

    public void RerollShopClick()
    {
        SMScript.RerollShop();
    }

    public static ShellStackGameManager Instance
    {
        get { return instance; }
    }

    public void TogglePause()
    {
     

        if (IsPaused)
        {
            IsPaused = !IsPaused;

            if (ShopOpen)
            {
                ShopOpen = false;
                SMScript.StoreOpen = false;
            }
            // Pause the game
            //   Time.timeScale = 0f;
            Debug.Log("Game paused");
        }
        else
        {
            IsPaused = !IsPaused;
            // Resume the game
           // Time.timeScale = 1f;
            Debug.Log("Game resumed");
        }
        PauseMenu.SetActive(IsPaused);
    }

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


    public void PowerUp(int AnimalID, int UpgradeId)
    {

        switch (AnimalID)
        {
            //TURTLE
            case 1:
                switch (UpgradeId)
                {
                    case 1:
                        HealhSystem.maxHealth += 10;
                        HealhSystem.currentHealth += 5;
                        break;
                    case 2:
                        PlayerController.TurtleMovementBaseSpeed += 5;
                        break;
                    case 3:
                        PlayerController.TurtleDamage += 5;
                        break;
                }
                break;
            //CRAB
            case 2:
                switch (UpgradeId)
                {
                    case 1:
                        PlayerController.CrabArea += 0.25f;
                        break;
                    case 2:
                        PlayerController.CrabRotationSpeed += 5;
                        break;
                    case 3:
                        PlayerController.CrabDamage += 5;
                        break;
                }

                break;
            //BIRD
            case 3:
                switch (UpgradeId)
                {
                    case 1:
                        PlayerController.shootCooldown -=  PlayerController.shootCooldown * 0.1f;
                        break;
                    case 2:
                        PlayerController.BirdThrowSpeed += 5f;
                        break;
                    case 3:
                        PlayerController.BirdDamage += 5f;
                        break;
                }
                break;
        }

        UpdateTopUi();
    }
}
