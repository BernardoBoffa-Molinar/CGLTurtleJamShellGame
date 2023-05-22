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
    public float CrabArea = 1f;
    public GameObject CrabAreaAttack;

    //Bird MECHANICS
    [SerializeField]
    public Vector2 BirdAimDirection;
    public float BirdThrowSpeed = 45;
    public float BirdDamage = 10;
    public Transform ShootingPoint;
    public GameObject BirdEggProjectile;
    public GameObject BirdBody;
    public bool canShoot = true;
    public bool canMove = true;
    public float shootTimer = 0;
    public float shootCooldown = 1;

    public Transform pointA;
    public Transform pointB;
    public float duration = 2f;
    public string fire2Axis = "Fire2";

    private float startTime;
    private Vector3 initialLocalPosition;
    private bool InAir = false;
    public GameObject EggAimSprite;

    public Vector3 FlyDirection;

    public Sprite InAirBirdSprite;
    public Sprite BirdSprite;



    // Start is called before the first frame update
    void Start()
    {
        GameManager = GetComponent<ShellStackGameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.IsPaused && !GameManager.ShopOpen && !GameManager.ShellGameOver)
        {
            TurtleControll(Time.deltaTime);
            CrabControll(Time.deltaTime);
            BirdControll(Time.deltaTime);
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


        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Joystick1Button4))
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
        //Debug.Log(CrabAttackDirection);

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


    void BirdAim()
    {

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10;

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePosition.z = BirdBody.transform.position.z;
        Vector3 direction = worldPosition - transform.position;
        direction.Normalize();
        //Debug.Log(direction);
        BirdAimDirection = new Vector2(direction.x, direction.y);

        float targetAngle = Mathf.Atan2(BirdAimDirection.y, BirdAimDirection.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
        BirdBody.transform.rotation = Quaternion.Lerp(BirdBody.transform.rotation, targetRotation, BirdThrowSpeed * Time.deltaTime);

    }

    void BirdControll(float dt)
    {
        
       if((Input.GetMouseButtonDown(0) || Input.GetAxis("Fire1")> 0.1f) && canShoot && !InAir)
        {
            canShoot = false;
            BirdThrowEgg();
        }


        if (( Input.GetAxis("Fire2") > 0.1f))
        {
            canShoot = false;
            if (!InAir)
            {
                InAir = true;
                BirdBody.GetComponent<SpriteRenderer>().sprite = InAirBirdSprite;
            }
            BirdFly();
        }
        else
        {
            if (InAir)
            {
                BirdFlyBack();
            }
         
        }


        if (!InAir)
        {
            BirdAim();
         
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

        EggAimSprite.SetActive(!InAir);

    }



    void BirdThrowEgg()
    {
        GameManager.PlaySoundInManager("EggShoot");
        Vector3 SpawnEggPosition = BirdBody.transform.position + new Vector3( BirdAimDirection.x,BirdAimDirection.y,0f)* 0.25f;

        GameObject EggProjectile = Instantiate(BirdEggProjectile, SpawnEggPosition, Quaternion.identity);
        EggProjectile.GetComponent<Rigidbody2D>().velocity =  -1 * BirdAimDirection.normalized * BirdThrowSpeed;
        EggProjectile.GetComponent<EggProjectile>().SetDamage(BirdDamage);
    }



    void BirdFly()
    {
        Vector3 localPosition = BirdBody.transform.localPosition;
        Vector3 worldPosition = transform.TransformPoint(localPosition);

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 0f;
        Vector3 targetWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector3 direction = targetWorldPosition - worldPosition;
        direction.Normalize();
        /*
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 1;

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3 localPosition = BirdBody.transform.InverseTransformPoint(worldPosition);

       // localPosition.z = BirdBody.transform.localPosition.z;
        Vector3 direction = localPosition - BirdBody.transform.localPosition;
        */
        BirdBody.transform.localPosition += new Vector3(direction.x, direction.y, 0f).normalized * BirdThrowSpeed * Time.deltaTime;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
        BirdBody.transform.rotation = Quaternion.Lerp(BirdBody.transform.rotation, targetRotation, BirdThrowSpeed/2 * Time.deltaTime);


    }

    void BirdFlyBack()
    {

       
        Vector3 flyBack = Vector3.zero - BirdBody.transform.localPosition;
        if(flyBack.magnitude < 1f)
        {
            canShoot = true;
        }

        BirdBody.transform.localPosition += new Vector3(flyBack.x, flyBack.y, 0f).normalized * BirdThrowSpeed/2* Time.deltaTime;
        
        if (Vector3.Distance( BirdBody.transform.localPosition , Vector3.zero) <1f)
        {
            InAir = false;
            BirdBody.GetComponent<SpriteRenderer>().sprite = BirdSprite;
        }
    }


  



}
