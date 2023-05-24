using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealhSystemInterface : MonoBehaviour
{
    public bool IsPlayer;

    public int shield = 0;
    public float ImmuneframeTimer = 0f;
    public float ImmuneframeDuration = 0.2f;
    public bool TookDamage = false;

    public float maxHealth ;
    public float currentHealth;
    public PlayersStackController PlayerController;
    public ShellStackGameManager GameController;
    public SpriteRenderer SP;
    public float EnemySpeed = 10f;
    float targetAngle;
    public int EnemyIndex = 0;
    Vector2 MovementDirection;
    public bool isSnaillDrop;

    private void Start()
    {
        
        if (IsPlayer)
        {
            GameController = FindObjectOfType<ShellStackGameManager>();
            PlayerController = GetComponent<PlayersStackController>();
            currentHealth = maxHealth;
            TookDamage = false;
        }
        else
        {
            GameController = FindObjectOfType<ShellStackGameManager>();
            PlayerController = FindObjectOfType<PlayersStackController>();
            SP = GetComponent<SpriteRenderer>();
            maxHealth *= GameController.StageDifficulty;
            TookDamage = false;
            currentHealth = maxHealth;
           
        }
    }

    private void Update()
    {
        if (IsPlayer)
        {
            if (currentHealth > 0)
            {
                if (TookDamage)
                {
                    ImmuneframeTimer += Time.deltaTime;

                    if (SP)
                    {
                        if (SP.color == Color.black)
                        {
                            SP.color = Color.red;
                        }
                        else if (SP.color == Color.red)
                        {
                            SP.color = Color.black;
                        }
                    }



                    if (ImmuneframeTimer >= ImmuneframeDuration)
                    {
                        TookDamage = false;
                        ImmuneframeTimer = 0f;
                        if (SP)
                        {
                            SP.color = Color.white;
                        }
                    }

                }
            }
            else
            {
                Die();
            }
        }
        else if(!isSnaillDrop)
        {
            if(currentHealth > 0 )
            {

                if (TookDamage)
                {
                    Vector3 MoveAwayFromPlayer = -1*(PlayerController.gameObject.transform.position - transform.position);

                    Vector2 MovementDirection = new Vector2(MoveAwayFromPlayer.x, MoveAwayFromPlayer.y);
                    MovementDirection.Normalize();
                    ImmuneframeTimer += Time.deltaTime;

                    GetComponent<Rigidbody2D>().velocity = MovementDirection * EnemySpeed *2;
                    if (gameObject.GetComponent<SpriteRenderer>().color == Color.black)
                    {
                        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                    }
                    else if (gameObject.GetComponent<SpriteRenderer>().color == Color.red)
                    {
                        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                    }
                    else if(gameObject.GetComponent<SpriteRenderer>().color == Color.white)
                    {
                        gameObject.GetComponent<SpriteRenderer>().color = Color.black;
                    }

                    if (ImmuneframeTimer >= ImmuneframeDuration)
                    {
                        TookDamage = false;
                        ImmuneframeTimer = 0f;
                        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                    }

                }
                else
                {
                    Vector3 MoveToPlayer = PlayerController.gameObject.transform.position - transform.position;

                    MovementDirection = new Vector2(MoveToPlayer.x, MoveToPlayer.y);
                    MovementDirection.Normalize();


                    targetAngle = Mathf.Atan2(MovementDirection.y, MovementDirection.x) * Mathf.Rad2Deg;
                    Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle - 90);


                    gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, targetRotation, EnemySpeed * Time.deltaTime);

                    if (!GameController.IsPaused && !GameController.ShopOpen)
                    {
                        SP.flipX = MovementDirection.x < 0 ? true : false;
                        GetComponent<Rigidbody2D>().velocity = MovementDirection * EnemySpeed;


                    }
                    else
                    {
                        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    }
                }

            }
            else
            {
                if (Vector3.Distance(gameObject.transform.position, GameController.transform.position) > 50)
                {
                    Destroy(gameObject);
                };

            }


        }
        else
        {
            if (TookDamage)
            {
                ImmuneframeTimer += Time.deltaTime;
                if (gameObject.GetComponent<SpriteRenderer>().color == Color.black)
                {
                    gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                }
                else if (gameObject.GetComponent<SpriteRenderer>().color == Color.yellow)
                {
                    gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                }
                else if (gameObject.GetComponent<SpriteRenderer>().color == Color.white)
                {
                    gameObject.GetComponent<SpriteRenderer>().color = Color.black;
                }

                if (ImmuneframeTimer >= ImmuneframeDuration)
                {
                    TookDamage = false;
                    ImmuneframeTimer = 0f;
                    gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                }

            }
        }
      
    }

    public void TakeDamage(float damageAmount)
    {
        if (shield <=0)
        {
            currentHealth -= (int)damageAmount;
            //Debug.Log(gameObject.name +" has "+ currentHealth + " and took  damage : " + damageAmount);

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
            }
        }
        else { 
        
            shield -= (int)Mathf.Floor(damageAmount);

            if (shield < 0)
            {
                TakeDamage(shield);
                shield = 0;
            }

        }



    }

    public void Heal(int healAmount)
    {
      
        currentHealth += healAmount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
       
    }


    public void GainShield( int shieldToGain)
    {
        shield += shieldToGain;

    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    private void Die()
    {
        // Perform death logic here
        // ...

        if (!IsPlayer)
        {
            if (!isSnaillDrop)
            {
                GameController.SnailsCount += (2 + Random.Range(1, 5));
                Destroy(gameObject);
            }
            else
            {
                GameController.SnailsCount += 25;
                Destroy(gameObject);
            }
        }
        
        if(IsPlayer)
        {
            GameController.ShellGameOver = true;
            GameController.WinGame();
        }

        
        

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (IsPlayer)
        {
            //Player HealthSystem
            if (collision.gameObject.CompareTag("Enemy"))
            {
                if (!TookDamage)
                {
                    TakeDamage(10);
                    TookDamage = true;
                    SP.color = Color.black;
                }

            }


        }
        else
        {
            if (!TookDamage)
            {
                //Enemy HealthSystem
                HandleEnemyCollisionsWithPlayer(collision);
            }


        }

    }



    private void OnTriggerStay2D(Collider2D collision)
    {


        if (IsPlayer)
        {
            //Player HealthSystem
            if (collision.gameObject.CompareTag("Enemy"))
            {
                if (!TookDamage)
                {
                    TakeDamage(10);
                    TookDamage = true;
                    SP.color = Color.black;
                }

            }


        }
        else
        {
            if (!TookDamage)
            {
                //Enemy HealthSystem
                HandleEnemyCollisionsWithPlayer(collision);
            }

        
        }

    }



    public void HandleEnemyCollisionsWithPlayer(Collider2D otherCollider2D) { 
    
            // Check if the collision involves objects with specific tags
            if (otherCollider2D.gameObject.CompareTag("EggProjectile") ){
                if (otherCollider2D.gameObject.GetComponent<EggProjectile>())
                {
                    TakeDamage(otherCollider2D.gameObject.GetComponent<EggProjectile>().EggDamage);
                    TookDamage = true;
                    GameController.PlaySoundInManager("EggCrack");
                    gameObject.GetComponent<SpriteRenderer>().color = Color.black;
                    ImmuneframeDuration = 0.25f;
                }
            }
            else if (otherCollider2D.gameObject.CompareTag("CrabAttackArea")){
                TakeDamage(PlayerController.CrabDamage);
                GameController.PlaySoundInManager("CrabSlash");
                TookDamage = true;
                gameObject.GetComponent<SpriteRenderer>().color = Color.black;
                ImmuneframeDuration = 0.5f;

            }
            else if(otherCollider2D.gameObject.CompareTag("Player") )
            {
                // Handle collision with the enemy
                // ...
                TakeDamage(PlayerController.TurtleDamage);
                GameController.PlaySoundInManager("TurtleHit");
                TookDamage = true;
                gameObject.GetComponent<SpriteRenderer>().color = Color.black;
                ImmuneframeDuration = 1f;

            }
    }
}
