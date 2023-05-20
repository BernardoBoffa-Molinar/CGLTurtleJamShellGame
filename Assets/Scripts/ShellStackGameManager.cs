using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        timerText.text = "Timer: " + GameTimer.ToString("F2");
        SnaillScoreText.text = SnailsCount.ToString();
        StageResourcesScoreText.text = StageResources.ToString("F0");
        HPText.text = "HP: " + HealhSystem.currentHealth + " / " + HealhSystem.maxHealth;
    }

    // Example method
    public void StartGame()
    {
        // Logic for starting the game
        // ...

        ShellGameOver = false;
        PlayerController = GetComponent<PlayersStackController>();
        HealhSystem = GetComponent<HealhSystemInterface>();
        if (PlayerController == null)
        {
            Debug.LogError("PlayerController component not found!");
        }
    }

    // Example method
    public void EndGame()
    {
        // Logic for ending the game
        // ...
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

            UpdateTimerText();


            
        }


        

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        


        if (ShellGameOver)
        {
           
        }
        
    }

    public static ShellStackGameManager Instance
    {
        get { return instance; }
    }

    private void TogglePause()
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
    }


}
