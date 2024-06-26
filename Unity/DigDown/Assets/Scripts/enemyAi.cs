using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AI;

public class enemyAi : MonoBehaviour
{

    public Transform target;

    public float followSpeed = 5f;



    public CircleCollider2D enemyCircleCol;
    public BoxCollider2D enemyBoxCol;



    public PlayerAttack aPA;
    //enemy movement and health 
    public float speed = 2f;
    public float stoppingDistance = 10f;
    public float distanceToPlayer = 3f;
    public float jump = 5f;
    public float HP = 3;
    public float attackRange = 1.3f;
    public float attackDamage = 6f;
    public float knockBackForce = 1f;
    public float coolDownTimer = 3f;
    public float lastTimeAttack;



    private Vector3 dir = Vector3.left;
    private Rigidbody2D body;
    private Animator anime;
    private bool isAttacking = false;
    //--private bool isJumping = false;
    //--private bool isDead = false;

    public LayerMask groundLayer;
    //public LayerMask enemyLayers;

    public GameObject obstacle; //future rename to objects
    public GameObject enemy;
    public GameObject player;


    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;

    public bool followPlayer = false;
    public bool isGrounded;
    //public bool isJumping;
    void Start()
    {
        enemyCircleCol = GetComponent<CircleCollider2D>();
        enemyBoxCol = GetComponent<BoxCollider2D>();

        player = GameObject.FindGameObjectWithTag("Player"); //- original was "player
        rb = GetComponent<Rigidbody2D>();
        groundLayer = LayerMask.GetMask("Ground");
        //obstacle = GameObject.FindGameObjectWithTag("obstacle");
        //enemy = GameObject.FindGameObjectWithTag("Enemy");
        spriteRenderer = GetComponent<SpriteRenderer>();

    }
    //flip sprite
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anime = GetComponent<Animator>();
    }
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (target != null)
        {
            Vector2 direction = target.position - transform.position;
            direction.Normalize();

            if (direction.x > 0) {
                spriteRenderer.flipX = false; // Face right
            } else if (direction.x < 0) {
                spriteRenderer.flipX = true; // Face left
            }


            //if (horizontalInput > 0.01f)
            //{
            //    dir = Vector3.left;
            //}
            //else if (horizontalInput < -0.01f)
            //{
            //    dir = Vector3.right;
            //}
            transform.position += (Vector3)direction * followSpeed * Time.deltaTime;

        }




        ////flip sprite
        //float horizontalInput = Input.GetAxis("Horizontal");
        //body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
        //if (horizontalInput > 0.01f)
        //{
        //    dir = Vector3.right;
        //}
        //else if (horizontalInput < -0.01f)
        //{
        //    dir = Vector3.left;
        //}

        //isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, groundLayer);
        //lastTimeAttack = -coolDownTimer;

        ////if player is detected, then follow the player
        //float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        //if (distanceToPlayer <= stoppingDistance)
        //{
        //    followPlayer = true;
        //}
        //else
        //{
        //    followPlayer = false;
        //}

        //if (followPlayer)
        //{
        //    Vector3 direction = (player.transform.position - transform.position).normalized;
        //    transform.position += speed * Time.deltaTime * direction;
        //    //test flip
        //    if (direction.x > 0)
        //    {
        //        spriteRenderer.flipX = false;
        //        GetComponent<CircleCollider2D>().offset = new Vector2(0.2f, GetComponent<CircleCollider2D>().offset.y);

        //    }
        //    else if (direction.x < 0)
        //    {
        //        spriteRenderer.flipX = true;
        //        GetComponent<CircleCollider2D>().offset = new Vector2(-0.2f, GetComponent<CircleCollider2D>().offset.y);
        //    }
        //}

        ////if the player is within the circle collider, attack
        //if (Time.time - lastTimeAttack >= coolDownTimer)
        //{
        //Debug.Log("distance to player value is " + distanceToPlayer);

        //}
        //if the player damages the enemy to HP = 0, die
        if (HP < 1)
        {
            enemyCircleCol.enabled = false;
            enemyBoxCol.enabled = false;
            Die();
        }

        //--anime.SetBool("IsWalking", Mathf.Abs(horizontalInput) > 0.01f);
        anime.SetBool("Attack", isAttacking);
        //--anime.SetBool("IsJumping", isJumping);
        //--anime.SetBool("IsDead", isDead);

    }
    void Jump()
    {
        rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
        //--isJumping = true;

    }

    //if come into contact with another enemy, turn around
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
            //--isJumping = false;
        }
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Collision has been made with another enemy");
            if (dir == Vector3.left)
            {
                dir = Vector3.right;
            }
            else if (dir == Vector3.right)
            {
                dir = Vector3.left;
            }
        }
        //if circle collider detects obstacle, jump
        if (collision.CompareTag("obstacle") && isGrounded)
        {
            Jump();
            isGrounded = false;
        }


        //--
        if (collision.gameObject.name == "New Dwarf") //OG //collision.CompareTag("Player")
        {
            //--
            Debug.Log("damage has been dealt to player");
            aPA.playerHP = aPA.playerHP - 30;
            //--
            isAttacking = true;
            anime.SetBool("Attack", isAttacking);
            Attack();
            lastTimeAttack = Time.time;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "New Dwarf")
        {
            isAttacking = false;
            anime.SetBool("Attack", isAttacking);
        }
    }

    void Attack()
    {
        anime.SetBool("Attack", isAttacking);
        Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;
        player.GetComponent<Rigidbody2D>().AddForce(knockbackDirection * knockBackForce, ForceMode2D.Impulse);
    }

    void Die()
    {
        anime.SetTrigger("die trig");
        //Destroy(gameObject, anime.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject, 1.37f);
    }

    bool IsWalking()
    {
        return true;
    }
}