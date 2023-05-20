using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggProjectile : MonoBehaviour
{

    public float EggDamage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDamage(float damageToSet)
    {
        EggDamage = damageToSet;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {




            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
      
    }
}