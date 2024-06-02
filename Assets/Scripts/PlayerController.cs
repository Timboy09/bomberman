using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameManager GManager;

    [SerializeField] private float runSpeed = 40f;
    [SerializeField] private float bombDropRate = 1f;

    public GameObject bombPrefab;

    private CharacterController2D controller;
    public Animator playerAnim;
    private Rigidbody2D rb;
    private float horizontalMove = 0f;
    private float nextBombDrop; 
    private float shakeIntensity = 3f;
    private float shakeDuration = 0.2f;
    public bool canMove = false;
    public bool canJump = false;
    private const int maxLives = 5;
    public int currentLives;

    private void Awake()
    {
        canMove = false;
    }

    private void Start()
    {
        GManager = GameManager.GManager;

        controller = this.GetComponent<CharacterController2D>();
        playerAnim = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody2D>();

        currentLives = maxLives;
        GManager.uiCanvas.heartText.text = "x0" + currentLives.ToString();
    }

    private void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        playerAnim.SetFloat("speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump"))
        {
            canJump = true;
            playerAnim.SetBool("canJump", true);
        }

        if (Input.GetMouseButtonDown(0) && canMove)
        {
            PlaceBombs();
        }
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            controller.Move(horizontalMove * Time.deltaTime, canJump);
        }
        canJump = false;
    }

    public void OnLanding()
    {
        playerAnim.SetBool("canJump", false);
    }

    private void PlaceBombs()
    {
        if(Time.time > nextBombDrop)
        {
            nextBombDrop = Time.time + bombDropRate;
            GameObject bomb = Instantiate(bombPrefab);
            bomb.transform.position = this.transform.position;
        }        
    }

    private void OnTriggerEnter2D(Collider2D others)
    {
        if (others.tag == "Bomb Explosion")
        {
            TakeDamage(5f, others);
        } 
        
        if(others.tag == "Door")
        {
            GManager.levelDoor.LoadNextLevel();
        }
    }

    private void OnCollisionEnter2D(Collision2D others)
    {
        if (others.collider.tag == "Heart PickUp")
        {
            HeartPickUp(others.gameObject);
        }
    }

    private void HeartPickUp(GameObject heart)
    {
        currentLives++;
        HealthText(currentLives);
        Destroy(heart);
    }

    private void HealthText(int lives)
    {
        if(lives < 10)
        {
            GManager.uiCanvas.heartText.text = "x0" + lives.ToString();
        }
        else
        {
            GManager.uiCanvas.heartText.text = "x" + lives.ToString();
        }
    }

    public void TakeDamage(float pushBackValue, Collider2D collider)
    {        
        rb.velocity = Vector2.zero;
        StartCoroutine(TimerAfterDamage(collider));
        currentLives--;
        GManager.cameraShake.ShakeCamera(shakeIntensity, shakeDuration);

        if (currentLives > 0)
        {
            HealthText(currentLives);
            playerAnim.SetBool("isHit", true);
            playerAnim.SetBool("canJump", false);
            rb.velocity = new Vector2(Random.Range(pushBackValue, -pushBackValue), Random.Range(pushBackValue, -pushBackValue));
        }
        else
        {
            if(currentLives < 0)
            {
                currentLives = 0;
            }
            HealthText(currentLives);
            canJump = false;
            GManager.uiCanvas.gameOverPanel.SetActive(true);
            playerAnim.SetBool("isDead", true);
            playerAnim.SetBool("canJump", false);
        }
    }

    public IEnumerator EndLevelAnimation()
    {
        canMove = false;
        canJump = false;
        yield return new WaitForSeconds(2f);

    }

    private IEnumerator TimerAfterDamage(Collider2D col)
    {
        if(col != null)
        {
            col.enabled = false;
        }
        yield return new WaitForSeconds(1f);
        if(col != null)
        {
            col.enabled = true;
        }
    }

    private void ResetDamageAnim()
    {
        canMove = true;
        playerAnim.SetBool("isHit", false);
    }

    private void PlayerDead()
    {
        Destroy(this.gameObject);
    }
}
