using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersStackController : MonoBehaviour
{

    public float TurtleMovementSpeed = 10;
    public float CrabRotationSpeed = 10;
    public float ChickenThrowSpeed = 20;
    [SerializeField]
    public Vector2 TurtleMovement;
    [SerializeField]
    public Vector2 CrabAttackDirection;
    [SerializeField]
    public Vector2 ChickenAimDirection;
    public GameObject CrabBody;
    public GameObject TurtleBody;
    public GameObject ChickenBody;
    public Transform ShootingPoint;

    public GameObject ChickenEggProjectile;

    public bool canShoot = true;
    public float shootTimer = 0;
    float shootCooldown = 1;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TurtleControll(Time.deltaTime);
        CrabControll(Time.deltaTime);
        ChickenControll(Time.deltaTime);
    }



    void TurtleControll(float dt)
    {
        //Change Input System Later Maybe
        TurtleMovement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if(TurtleMovement.magnitude > 0.1f)
        {
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(TurtleMovement.x, 0, TurtleMovement.y)  * TurtleMovementSpeed;
            float targetAngle = Mathf.Atan2(TurtleMovement.x, TurtleMovement.y) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            TurtleBody.transform.rotation = Quaternion.Lerp(TurtleBody.transform.rotation, targetRotation, TurtleMovementSpeed * Time.deltaTime);

        }
        else
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }


    }



    void CrabControll(float dt)
    {

        //Change Input System Later Maybe
        CrabAttackDirection = new Vector2(Input.GetAxis("Debug Vertical"), Input.GetAxis("Debug Horizontal"));

        if (CrabAttackDirection.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(CrabAttackDirection.y, CrabAttackDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            CrabBody.transform.rotation = Quaternion.Lerp(CrabBody.transform.rotation, targetRotation, CrabRotationSpeed * Time.deltaTime);
        }
        else
        {
           // gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

    }




    void ChickenControll(float dt)
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10;

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector3 direction = worldPosition - transform.position;
        direction.Normalize();
        Debug.Log(direction);
        ChickenAimDirection = new Vector2(direction.z,direction.x);

        float targetAngle = Mathf.Atan2(ChickenAimDirection.y, ChickenAimDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
        ChickenBody.transform.rotation = Quaternion.Lerp(ChickenBody.transform.rotation, targetRotation, ChickenThrowSpeed * Time.deltaTime);

       if(Input.GetMouseButtonDown(0) && canShoot)
        {
            canShoot = false;
            ChickenThrowEgg();
        }

        if (!canShoot)
        {
            shootTimer += dt;
            if(shootCooldown <= shootTimer)
            {
                canShoot = true;
                shootTimer = 0;
            }

        }

    }


    void ChickenThrowEgg()
    {
        Vector3 SpawnEggPosition = ShootingPoint.position + new Vector3( ChickenAimDirection.y,0,ChickenAimDirection.x)* 0.25f;

        GameObject EggProjectile = Instantiate(ChickenEggProjectile, SpawnEggPosition, Quaternion.identity);
        EggProjectile.GetComponent<Rigidbody>().velocity = new Vector3(ChickenAimDirection.y, 0, ChickenAimDirection.x) * ChickenThrowSpeed;

    }

}
