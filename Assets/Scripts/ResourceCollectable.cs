using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCollectable : Collectable
{
   public float CollectableTimer;
   public ShellStackGameManager Manager;

    public override void Collect(GameObject CollidingObject)
    {
        Manager.StageResources++;

        Destroy(this.gameObject);

    }

    // Start is called before the first frame update
    void Start()
    {
        Manager = FindObjectOfType<ShellStackGameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        CollectableTimer += Time.deltaTime;
        Quaternion targetRotation = Quaternion.Euler(0f, CollectableTimer *Mathf.Rad2Deg,0f);
        gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, targetRotation, 20 *Time.deltaTime);

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.GetComponent<PlayersStackController>())
        {
            Collect(collision.gameObject);
        }


    }


}
