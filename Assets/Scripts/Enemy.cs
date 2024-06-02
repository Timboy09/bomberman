using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private GameManager GManager;
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private Transform player;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayerMask;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float distanceFromPlayer;
    [SerializeField] private float bombThrowHeight;
    [SerializeField] private int currentHealth;
    [SerializeField] private int pushBackValue = 5;
    [SerializeField] private int damageAmount;
    [SerializeField] private Slider healthSlider;

    private Animator enemyAnim;
    private CharacterController2D controller2D;
    private Rigidbody2D rb;
    private float horizontalMove = 0f;
    private float wallCheckRadius = 0.5f;
    private bool isTouchingWall = false;
    private bool shouldCheckWall = false;
    private int maxHealth = 100;

    private void Awake()
    {
        GManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        player = GManager.playerController.gameObject.transform;
        enemyAnim = this.GetComponent<Animator>();
        controller2D = this.GetComponent<CharacterController2D>();
        rb = this.GetComponent<Rigidbody2D>();
        healthSlider.gameObject.SetActive(false);
        currentHealth = maxHealth;
    }

    private void Update()
    {
        MoveTowardsPlayer();

        if(enemyType == EnemyType.Bald_Pirate)
        {
            if (shouldCheckWall)
            {
                isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayerMask);
            }
            else
            {
                isTouchingWall = false;
            }
        }
    }

    private void FixedUpdate()
    {
        switch (enemyType)
        {
            case EnemyType.Whale:
                controller2D.MoveEnemy(horizontalMove * Time.deltaTime, false);
                break;

            case EnemyType.Bald_Pirate:
                if (isTouchingWall)
                {
                    enemyAnim.SetBool("canJump", true);
                }
                controller2D.Move(horizontalMove * Time.deltaTime, isTouchingWall);
                isTouchingWall = false;
                break;
        }
    }

    public void OnLanding()
    {
        enemyAnim.SetBool("canJump", false);
    }

    private void MoveTowardsPlayer()
    {
        if(player != null)
        {
            if (Vector2.Distance(transform.position, player.position) < distanceFromPlayer)
            {
                float distX = Mathf.Abs(transform.position.x - player.position.x);

                if(distX < 0.1f)
                {
                    shouldCheckWall = false;
                    horizontalMove = 0f * moveSpeed;
                }
                else
                {
                    shouldCheckWall = true;
                    if (transform.position.x > player.position.x)
                    {
                        horizontalMove = -1f * moveSpeed;
                    }
                    else
                    {
                        horizontalMove = 1f * moveSpeed;
                    }
                }                
            }
            else
            {
                shouldCheckWall = false;
                horizontalMove = 0f * moveSpeed;
            }
            enemyAnim.SetFloat("speed", Mathf.Abs(horizontalMove));
        }        
    }

    private void OnTriggerEnter2D(Collider2D others)
    {
        if (others.tag == "Bomb Explosion")
        {
            currentHealth -= damageAmount;
            healthSlider.gameObject.SetActive(true);
            healthSlider.value = currentHealth;
            if (currentHealth > 0)
            {
                enemyAnim.SetBool("isHit", true);
                rb.velocity = new Vector2(Random.Range(pushBackValue, -pushBackValue), Random.Range(pushBackValue, -pushBackValue));
            }
            else
            {
                GManager.levelDoor.CheckLevelComplete(this.gameObject);
                enemyAnim.SetBool("isDead", true);
            }
        }

        if (others.tag == "Bomb Col")
        {
            switch (enemyType)
            {
                case EnemyType.Whale:
                    SwallowBomb(others.gameObject);
                    break;

                case EnemyType.Bald_Pirate:
                    KickBomb(others.gameObject);
                    break;
            }
        }

        if(others.tag == "Player")
        {
            GManager.playerController.TakeDamage(10f, others);
        }
    }

    private void SwallowBomb(GameObject bomb)
    {
        enemyAnim.SetBool("swalloBomb", true);
        rb.velocity = Vector2.zero;
        Destroy(bomb.transform.parent.gameObject, 0.35f);
    }

    private void KickBomb(GameObject bomb)
    {
        enemyAnim.SetBool("kickBomb", true);
        rb.velocity = Vector2.zero;
        Rigidbody2D bombRB = bomb.transform.parent.GetComponent<Rigidbody2D>();
        float dist;
        if(player != null)
        {
            dist = player.position.x - transform.position.x;
        }
        else
        {
            dist = bombThrowHeight;
        }
        bombRB.AddForce(new Vector2(dist, bombThrowHeight), ForceMode2D.Impulse);
    }

    private void ResetSwallowAnim()
    {
        enemyAnim.SetBool("swalloBomb", false);
    }

    private void ResetKickAnim()
    {
        enemyAnim.SetBool("kickBomb", false);
    }

    private void ResetDamageAnim()
    {
        healthSlider.gameObject.SetActive(false);
        enemyAnim.SetBool("isHit", false);
    }
    private void EnemyDead()
    {
        Destroy(this.gameObject);
    }
}

public enum EnemyType
{
    MIX,
    Whale,
    Bald_Pirate,
    Big_Guy,
    MAX
}
