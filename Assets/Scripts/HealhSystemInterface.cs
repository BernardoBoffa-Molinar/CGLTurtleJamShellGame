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

    public int maxHealth = 100;
    public int currentHealth;
    public PlayersStackController PlayerController;
    public ShellStackGameManager GameController;
    public SpriteRenderer SP;
    public float EnemySpeed = 10f;
    float targetAngle;

    private void Start()
    {
        currentHealth = maxHealth;
        if (IsPlayer)
        {
            PlayerController = GetComponent<PlayersStackController>();
            GameController = FindObjectOfType<ShellStackGameManager>();
            
        }
        else
        {
            GameController = FindObjectOfType<ShellStackGameManager>();
            PlayerController = FindObjectOfType<PlayersStackController>();
            SP = GetComponent<SpriteRenderer>();
        }
    }

    private void Update()
    {
        if (IsPlayer)
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
            if(currentHealth > 0 )
            {

                Vector3 MoveToPlayer = PlayerController.gameObject.transform.position - transform.position;

                Vector2 MovementDirection = new Vector2(MoveToPlayer.x, MoveToPlayer.y);
                MovementDirection.Normalize();

                if (TookDamage)
                {
                    ImmuneframeTimer += Time.deltaTime;
                    MovementDirection *= -1f;
                    if(gameObject.GetComponent<SpriteRenderer>().color == Color.black)
                    {
                        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                    }
                    else if (gameObject.GetComponent<SpriteRenderer>().color == Color.red)
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


                targetAngle = Mathf.Atan2(MovementDirection.y, MovementDirection.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
                gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, targetRotation, EnemySpeed * Time.deltaTime);

                if (!GameController.IsPaused)
                {
                    GetComponent<Rigidbody2D>().velocity = MovementDirection * EnemySpeed;
                }
                else
                {
                    GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                }
            }


        }
      
    }

    public void TakeDamage(float damageAmount)
    {
        if (shield <=0)
        {
            currentHealth -= (int)Mathf.Floor(damageAmount);
            Debug.Log(gameObject.name +" has "+ currentHealth + " and took  damage : " + damageAmount);

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
            GameController.SnailsCount++;
            Destroy(gameObject);
        }
        else
        {
            GameController.ShellGameOver = true;
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

            //Enemy HealthSystem
            HandleEnemyCollisionsWithPlayer(collision);
           
        }
         
    }

    public void HandlePlayerCollisions()
    {


    }




    public void HandleEnemyCollisionsWithPlayer(Collider2D playerweapon) { 
    
            // Check if the collision involves objects with specific tags
            if (playerweapon.gameObject.CompareTag("EggProjectile") ){
                if (playerweapon.gameObject.GetComponent<EggProjectile>())
                {
                    TakeDamage(playerweapon.gameObject.GetComponent<EggProjectile>().EggDamage);
                    TookDamage = true;
                    gameObject.GetComponent<SpriteRenderer>().color = Color.black;
                    ImmuneframeDuration = 0.25f;
                }
            }
            else if (playerweapon.gameObject.CompareTag("CrabArms")){
                TakeDamage(PlayerController.CrabDamage);
                TookDamage = true;
                gameObject.GetComponent<SpriteRenderer>().color = Color.black;
                ImmuneframeDuration = 0.5f;

            }
            else if(playerweapon.gameObject.CompareTag("TurtleBody") || playerweapon.gameObject.GetComponent<HealhSystemInterface>())
            {
                // Handle collision with the enemy
                // ...
                TakeDamage(PlayerController.TurtleDamage);
                TookDamage = true;
                gameObject.GetComponent<SpriteRenderer>().color = Color.black;
                ImmuneframeDuration = 1f;

            }
    }
}
