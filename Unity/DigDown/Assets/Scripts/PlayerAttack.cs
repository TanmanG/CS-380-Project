using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Il2Cpp;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAttack : MonoBehaviour
{

    public SmallEnemyAi aSEA;
    public enemyAi aEA;
    public BigEnemyAi aBEA;

    public ItemPickup aIP; //-reference variable to the ItemPickup class
    public PlayerMovement aPM; //-reference variable to the ItemPickup class - important, keep watch if this works well
    public DetectShots aDS; //-reference variable to the DetectShot class

    [SerializeField] private float attackCoolDown;
    private Animator anime;

    public GameObject ENEMY;
    public GameObject SHOTGUN; //game object variable
    public GameObject SHOTGUN_AUDIO_SOURCE; //**
    public Animator shotgunAnimator; //component variable
    public GameObject PLAYER;


    public AudioSource shotgunAudioSource;//component variable
    public AudioClip explosionAudioClip;//component varibale - assign the audio clip you want to play to the explosionAudioClip field in the Inspector

    private PlayerMovement playerMovement; //-reference variable to the ItemPickup class
    private float coolDownTimer = Mathf.Infinity;
    public Transform attackPoint;
    public float attackRange = .5f;
    
    public LayerMask enemyLayers;

    
    public BoxCollider2D shotgunRangeCollider; //HERE
    public Collider2D[] enemiesInRangeArr;//HERE

    public Vector2 shotgunColliderCenPos;//HERE
    public Quaternion shotgunColliderRotation;//HERE
    public float angleInDegrees;//HERE
    public Vector2 shotgunCollidersize; //HERE

    //oo GOAL: make a rectangle for the OverLapBox, then make it visible.
    public Vector2 centerPosition;//oo
    public float rotation; //oo

    public float distance;
    public float closestDistance;
    public GameObject closestEnemy;
    //public Vector3 playerCenterPosition;

    public int playerHP = 30;
    

    // Awake is called before the first frame update
    private void Awake()
    {

        closestDistance = 500;
        ENEMY = GameObject.Find("enemy1");

        SHOTGUN = GameObject.Find("Shotgun");//gained access to the SHOTGUN gameobject, and from there I had access to its components, hence shotgunAnimator = SHOTGUN.GetComponent<Animator>();
        PLAYER = GameObject.Find("New Dwarf");

        shotgunAnimator = SHOTGUN.GetComponent<Animator>();
        anime = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        SHOTGUN_AUDIO_SOURCE = GameObject.Find("shotgunAudioObject"); //**
        shotgunAudioSource = SHOTGUN_AUDIO_SOURCE.GetComponent<AudioSource>();//**
        shotgunAudioSource.clip = explosionAudioClip;//


        shotgunRangeCollider = SHOTGUN.GetComponent<BoxCollider2D>(); //HERE
        shotgunCollidersize = shotgunRangeCollider.size; //we obtained the size of the shotgun's 2D box collider.

        Debug.Log("The shotgun's 2D box collider size is " + shotgunCollidersize); //CORRECT SIZE GETS OUTPUTTED.
        Debug.Log("The shotgun's 2D box collider position is " + shotgunColliderCenPos);
    }
    // Update registers user input
    public void Update() //- was private
    {
        if (playerHP <= 0)
        {
            //go to game over screen, change the screen/scene
            //SceneManager.LoadSceneAsync("Dead screen");
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
        //PROBLEM: one right click with my wireless mouse acts as if it clicked 10+ times.
        //Doesn't occur with mousepad right click.
        //If problem persists, change shotgun firing input to be Input.GetKeyDown(KeyCode.K).
        //Problem probably doesn't occur with the Mining right click.

        if (Input.GetKeyDown(KeyCode.K) && aIP.ammoShotgun > 0 && aPM.holdShotgun) //OG input //Input.GetKeyDown(KeyCode.K)
        {
            shotgunAnimator.Play("Shotgun Firing", 0, 0f);//**Everytime the condition is met above, the shoot animation will restart. SUCCESS
            shotgunAudioSource.Play();//play the shotgun shoot audio sound
            aIP.ammoShotgun = aIP.ammoShotgun - 1;//subtract one bullet from gun ammo
            Debug.Log("ammo value is " + aIP.ammoShotgun);


            shotgunColliderCenPos = shotgunRangeCollider.bounds.center; //need to obtain the CURRENT center position of the shotgun box collider.
            shotgunColliderRotation = shotgunRangeCollider.transform.rotation; //need to obtain the CURRENT rotation of the shotgun box collider.
            angleInDegrees = shotgunColliderRotation.eulerAngles.z; //need to obtain the CURRENT angle of the shotgun Box Collider

            enemiesInRangeArr = Physics2D.OverlapBoxAll(shotgunColliderCenPos, shotgunCollidersize/4, angleInDegrees);

            foreach (Collider2D collider in enemiesInRangeArr) //this is working a little better now.
            {
                if (collider.CompareTag("Enemy"))
                {
                    Debug.Log("The collider.gameObject.transform.position object was " + collider.gameObject.name); //should be an enemy.
                    distance = Vector2.Distance(PLAYER.transform.position, collider.gameObject.transform.position); //obtain distance between the player and the collided enemy object

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = collider.gameObject;
                    }
                    Debug.Log("Enemies within the shotgun box collider when FIRED are " + collider.gameObject.name); //Only print the objects names that have the Enemy tag.
                }
            }
            closestDistance = 500; //reset the closest distance value.
            ////////////////////////////////////////////
            ///instead of destroying the game object, the shotgun will now subtract 3 HP from singular enemies

            if (closestEnemy != null)
            {
                //*** if the game object doesnt have the enemyAi script,
                //check if it has the enemyAi2 script, if it doesn't, check if it has the enemyAi3 script
                //if (closestEnemy.GetComponent<SmallEnemyAi>() != null) //if closestEnemy has the SmallEnemyAi script...
                //{
                //    aSEA = closestEnemy.GetComponent<SmallEnemyAi>();
                //    aSEA.HP = aSEA.HP - 3;
                //}
                //else if (closestEnemy.GetComponent<enemyAi>() != null) //if closestEnemy has the enemyAi script (medium)...
                //{
                //    aEA = closestEnemy.GetComponent<enemyAi>();
                //    aEA.HP = aEA.HP - 3;
                //}
                //else if (closestEnemy.GetComponent<BigEnemyAi>() != null) //if closestEnemy has the BigEnemyAi script...
                //{
                //    aBEA = closestEnemy.GetComponent<BigEnemyAi>();
                //    aBEA.HP = aBEA.HP - 3;
                //}

                ////////
                if (closestEnemy.GetComponent<enemyAi>() != null)
                {
                    aEA = closestEnemy.GetComponent<enemyAi>();
                    aEA.HP = aEA.HP - 3;
                }
                ////////

                //aEA = closestEnemy.GetComponent<enemyAi>(); //want to access the enemyAi script for that specific enemy at this point in time
                //aEA.HP = aEA.HP - 3;
                //-Destroy(closestEnemy);
                ////////////////////////////////////////////
                enemiesInRangeArr = new Collider2D[0]; //empty out the array for reuse.
                                                       //Debug.Log("enemiesInRangeArr contents are " + enemiesInRangeArr[0] + ", " + enemiesInRangeArr[1]); //Why does this not get run???
            }
        }
        else if (Input.GetKeyDown(KeyCode.K) && aIP.ammoShotgun <= 0 && aPM.holdShotgun) //OG input //Input.GetKeyDown(KeyCode.K)
        {
            aPM.holdShotgun = false;
            anime.SetBool("HoldShotgun", aPM.holdShotgun);
            SHOTGUN.SetActive(false);
            Debug.Log("No ammo!");
        }
        //checks for a left mouse button input along with other restrictions if needed
        if (Input.GetMouseButton(0) && coolDownTimer > attackCoolDown && playerMovement.canAttack() && aPM.holdShotgun == false) {
            Attack();
        }
        coolDownTimer += Time.deltaTime;
    }
    //handles the Attack input
    private void Attack(){
        //starts the attack animation
        anime.SetTrigger("Attack");
        //checks if we hit an enemy
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        //resets our attack cooldown
        coolDownTimer = 0;
        //sends a console update for each enemy we hit
        foreach(Collider2D enemy in hitEnemies){ //for each ...., add it to the array/list
            Debug.Log("We hit " + enemy.name);
        }
    }
    //helps visualize the attack point in unity
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
