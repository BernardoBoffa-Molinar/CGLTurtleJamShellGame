using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCollectable : Collectable
{
   public float CollectableTimer;
   public ShellStackGameManager Manager;
    public float SpeedRotation;

    public override void Collect(GameObject CollidingObject)
    {
        Manager.StageResources++;
        Manager.PlaySoundInManager("Collectable");
        Destroy(this.gameObject);

    }


    // Start is called before the first frame update
    void Start()
    {
        CollectableTimer = Random.Range(0.1f, 1.0f);
        SpeedRotation = 5f;
        ColorChange = true;
        Manager = FindObjectOfType<ShellStackGameManager>();
        Destroy(gameObject, 10f);
    }

    // Update is called once per frame
    void Update()
    {

        if (!Manager.IsPaused && !Manager.ShopOpen)
        {
            CollectableTimer += Time.deltaTime;



            if (ColorChange)
            {
                float THue = Mathf.Abs(Mathf.Sin(CollectableTimer));
                gameObject.GetComponent<SpriteRenderer>().color = Color.Lerp(gameObject.GetComponent<SpriteRenderer>().color, Color.HSVToRGB(THue, 1, 1), (SpeedRotation / 2) * Time.deltaTime);
               
                
                Quaternion targetRotation = Quaternion.Euler(0f, Mathf.Sin(CollectableTimer)* Mathf.Rad2Deg,0f);
                gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, targetRotation, SpeedRotation * Time.deltaTime);
                if (CollectableTimer >= 9f)
                {
                    Vector3 scales = Vector3.one * Mathf.Abs(Mathf.Clamp01(10.0f - CollectableTimer)) * 3;

                    gameObject.transform.localScale = scales;
                }
            }
            else
            {

            }
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
