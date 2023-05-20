using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldColectable : MonoBehaviour, ICollectible
{
    public int ShieldAmmount = 10;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("TurtleBody"))
        {
            Debug.Log("Player Collision Here");
            Collect(other.gameObject);
        }
    }

    public void Collect( GameObject player)
    {
        // Implement specific logic for collecting a coin
        // ...
        
        player.GetComponent<HealhSystemInterface>().GainShield(ShieldAmmount);
        Destroy(gameObject);
    }

}