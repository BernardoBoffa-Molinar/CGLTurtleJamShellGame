using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersStackController : MonoBehaviour
{

    public ShellStackGameManager GameManager;

    //TURTLE MECHANICS
    public GameObject TurtleBody;
    [SerializeField]
    public Vector2 TurtleMovement;
    public float TurtleMovementSpeed = 10;
    public float TurtleMovementBaseSpeed = 20;
    public float TurtleMovementDrag = 15;
    public float TurtleDamage = 10;

    //CRAB MECHANICS
    public GameObject CrabBody;
    [SerializeField]
    public Vector2 CrabAttackDirection;
    public float CrabRotationSpeed = 10;
    public float CrabDamage = 10;

    //CHICKEN MECHANICS
    [SerializeField]
    public Vector2 ChickenAimDirection;
    public float ChickenThrowSpeed = 20;
    public float ChickenDamage = 10;
    public Transform ShootingPoint;
    public GameObject ChickenEggProjectile;
    public GameObject ChickenBody;
    public bool canShoot = true;
    public float shootTimer = 0;
    float shootCooldown = 1;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GetComponent<ShellStackGameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.IsPaused)
        {
            TurtleControll(Time.deltaTime);
            CrabControll(Time.deltaTime);
            ChickenControll(Time.deltaTime);
        }
        else
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        }
      
    }



    void TurtleControll(float dt)
    {
        //Change Input System Later Maybe
        TurtleMovement = new Vector2( -1f*Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));


        if (Input.GetKey(KeyCode.LeftShift))
        {
            if(TurtleMovementSpeed == TurtleMovementBaseSpeed)
            {
                TurtleMovementSpeed = TurtleMovementBaseSpeed *2;
            }
            else
            {
                TurtleMovementSpeed -= Time.deltaTime * TurtleMovementDrag;
                if(TurtleMovementSpeed < 20)
                {
                    TurtleMovementSpeed = TurtleMovementBaseSpeed;
                }
            }
            
        }
        else
        {
            TurtleMovementSpeed = TurtleMovementBaseSpeed;
        }




        if(TurtleMovement.magnitude > 0.1f)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = TurtleMovement * TurtleMovementSpeed;

            float targetAngle = Mathf.Atan2(TurtleMovement.y, TurtleMovement.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0f,0f, targetAngle);
            TurtleBody.transform.rotation = Quaternion.Lerp(TurtleBody.transform.rotation, targetRotation, TurtleMovementSpeed * Time.deltaTime);

        }
        else
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        }


    }



    void CrabControll(float dt)
    {

        //Change Input System Later Maybe
        CrabAttackDirection = new Vector2(Input.GetAxis("Debug Vertical"), Input.GetAxis("Debug Horizontal"));

        if (CrabAttackDirection.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(CrabAttackDirection.y, CrabAttackDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
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
        //Debug.Log(direction);
        ChickenAimDirection = new Vector2(direction.x,direction.y);

        float targetAngle = Mathf.Atan2(ChickenAimDirection.y, ChickenAimDirection.x) * Mathf.Rad2Deg -90f;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
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
        Vector3 SpawnEggPosition = ChickenBody.transform.position + new Vector3( ChickenAimDirection.x,ChickenAimDirection.y,0f)* 0.25f;

        GameObject EggProjectile = Instantiate(ChickenEggProjectile, SpawnEggPosition, Quaternion.identity);
        EggProjectile.GetComponent<Rigidbody2D>().velocity =  -1 * ChickenAimDirection * ChickenThrowSpeed;
        EggProjectile.GetComponent<EggProjectile>().SetDamage(ChickenDamage);
    }

}
