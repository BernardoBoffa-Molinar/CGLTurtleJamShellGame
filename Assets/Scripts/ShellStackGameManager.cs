using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellStackGameManager : MonoBehaviour
{
    private static ShellStackGameManager instance;
    public float GameTimer = 600f;
    public bool IsPaused;
    public bool ShellGameOver;
    public bool PlayerIsDeath;
    public PlayersStackController PlayerController;
    public HealhSystemInterface HealhSystem;

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



    // Example method
    public void StartGame()
    {
        // Logic for starting the game
        // ...

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


            if (Input.GetAxis("Escape") > 0f)
            {
                IsPaused = true;
            }
        }


        if (IsPaused)
        {

        }


        if (ShellGameOver)
        {

        }
        
    }

    public static ShellStackGameManager Instance
    {
        get { return instance; }
    }



}
