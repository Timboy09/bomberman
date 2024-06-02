using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObjects : MonoBehaviour
{
    [SerializeField] private bool isHeartBarrel = false;
    [SerializeField] private GameObject heartObj;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float displaceForceValue = 25f;
    [SerializeField] private float displaceTorqueValue = 5f;
    [SerializeField] private float stopVelocitySpeed = 10f;

    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool spawnedHeartOnce = false;
    private float groundRadius = 0.2f;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayerMask);
        rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, stopVelocitySpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D others)
    {
        if (others.tag == "Bomb Explosion")
        {
           DisplaceObjects();
        }
    }

    private void DisplaceObjects()
    {
        float randomeX = Random.Range(-displaceForceValue, displaceForceValue);
        float randomeY = Random.Range(1f, displaceForceValue);
        rb.AddForce(new Vector2(randomeX, randomeY), ForceMode2D.Impulse);
        rb.AddTorque(displaceTorqueValue, ForceMode2D.Impulse);

        //Spawn Heart ONLY once if it is a Heart Barrel
        if (isHeartBarrel)
        {
            if (!spawnedHeartOnce)
            {
                spawnedHeartOnce = true;
                GameObject newHeart = Instantiate(heartObj, this.transform.position, Quaternion.identity);
            }
        }
    }
}
