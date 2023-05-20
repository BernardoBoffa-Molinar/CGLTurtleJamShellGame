using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : Collectable
{
    public float CollectableTimer;
    public ShellStackGameManager Manager;
    public float SpeedRotation = 40f;
    public int HealAmmount = 20;

    public override void Collect(GameObject CollidingObject)
    {
        Manager.HealhSystem.Heal(HealAmmount);

        Destroy(this.gameObject);

    }

    // Start is called before the first frame update
    void Start()
    {
        ColorChange = false;
        Manager = FindObjectOfType<ShellStackGameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        CollectableTimer += Time.deltaTime;

        
        if (ColorChange)
        {
            float THue = CollectableTimer % 1f;
            gameObject.GetComponent<SpriteRenderer>().color = Color.Lerp(gameObject.GetComponent<SpriteRenderer>().color, Color.HSVToRGB(THue, 1, 1), SpeedRotation * Time.deltaTime);
        }
        else
        {
            Quaternion targetRotation = Quaternion.Euler(0f, CollectableTimer *Mathf.Rad2Deg,0f);
            gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, targetRotation, SpeedRotation * Time.deltaTime);
        
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.GetComponent<PlayersStackController>())
        {
            Collect(collision.gameObject);
        }


    }

}
