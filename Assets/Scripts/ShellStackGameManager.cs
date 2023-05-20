using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShellStackGameManager : MonoBehaviour
{
    public int StageResources = 0;
    public int SnailsCount = 0;
    private static ShellStackGameManager instance;
    public float GameTimer = 600f;
    public bool IsPaused;
    public bool ShellGameOver;
    public bool PlayerIsDeath;
    public PlayersStackController PlayerController;
    public HealhSystemInterface HealhSystem;


    //UI Objects
    public TMP_Text timerText;
    public TMP_Text SnaillScoreText;
    public TMP_Text StageResourcesScoreText;
    public TMP_Text HPText;
    public Slider HPSlider;
    public GameObject PauseMenu;

    public GameObject GameOverMenu;

    public GameObject ShopMenu;
 

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

    public float CollectSpawnMaxDistance = 60f;
    public float CollectSpawnMinDistance = 30f;
    public float CollectSpawnTimer = 0f;
    public float CollectSpawnColdown = 10f;
    public int CollectableStageIndex = 0;
    public int CollectablesAmmounts = 3;

    void SpawnCollectables()
    {
        for (int i = 0; i < CollectablesAmmounts; i++)
        {
            int CollectProbably = Random.Range(0, 100);
            int CollectToSpawn = 0;
            if (CollectProbably > 75)
            {
                CollectToSpawn = CollectablesPrefab.Count-1;
            }
            else
            {

            }
                     
            float ToSpawnDirX = Random.Range(-1.0f, 1.0f);
            float ToSpawnDirY = Random.Range(-1.0f, 1.0f);
            float distance = Random.Range(CollectSpawnMinDistance, CollectSpawnMaxDistance);
            Vector3 SpawnPosition = gameObject.transform.position + new Vector3(ToSpawnDirX, ToSpawnDirY, 0).normalized * distance;
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

    private void UpdateTimerText()
    {
        timerText.text = "Timer: " + GameTimer.ToString("F0");
        SnaillScoreText.text = SnailsCount.ToString();
        StageResourcesScoreText.text = StageResources.ToString("F0");
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
        PlayerController = GetComponent<PlayersStackController>();
        HealhSystem = GetComponent<HealhSystemInterface>();
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
        if (!IsPaused && !ShellGameOver)
        {
            GameTimer -= Time.deltaTime;
            if(GameTimer <= 0  || PlayerIsDeath)
            {
                ShellGameOver = true;
            }

            EnemiesSpawnerLogic();

            UpdateTimerText();
            CollectablesSpawnnerLogic();



        }


        

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        


        if (ShellGameOver)
        {
            EndGame();
        }

        
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




    }
}
