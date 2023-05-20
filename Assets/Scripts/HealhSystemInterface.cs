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
    private int currentHealth;
    public PlayersStackController PlayerController;

    private void Start()
    {
        currentHealth = maxHealth;
        if (IsPlayer)
        {
            PlayerController = GetComponent<PlayersStackController>();
        }
    }

    private void Update()
    {
        if (IsPlayer)
        {
            if (TookDamage)
            {
                ImmuneframeTimer += Time.deltaTime;
                if (ImmuneframeTimer >= ImmuneframeDuration)
                {
                    TookDamage = false;
                    ImmuneframeTimer = 0f;
                }

            }
        }
      
    }

    public void TakeDamage(float damageAmount)
    {
        if (shield <=0)
        {
            currentHealth -= (int)Mathf.Floor(damageAmount);
            Debug.Log(gameObject.name +"has "+ currentHealth + " and took  damage : " + damageAmount);

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
    }


    private void OnCollisionEnter(Collision collision)
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
                }

            }

            
        }
        else
        {

            //Enemy HealthSystem
            HandleEnemyCollisionsWithPlayer(collision.gameObject);
           
        }
         
    }

    public void HandlePlayerCollisions()
    {


    }




    public void HandleEnemyCollisionsWithPlayer(GameObject playerweapon) { 
    
            // Check if the collision involves objects with specific tags
            if (playerweapon.gameObject.CompareTag("EggProjectile")){
                if (playerweapon.gameObject.GetComponent<EggProjectile>())
                {
                    TakeDamage(playerweapon.gameObject.GetComponent<EggProjectile>().EggDamage);
                }
            }

            if (playerweapon.gameObject.CompareTag("CrabArms")){
                TakeDamage(playerweapon.gameObject.GetComponent<PlayersStackController>().CrabDamage);

            }
            else if(playerweapon.gameObject.CompareTag("TurtleBody"))
            {
                // Handle collision with the enemy
                // ...
                TakeDamage(playerweapon.gameObject.GetComponent<PlayersStackController>().TurtleDamage);

            }
    }
}
