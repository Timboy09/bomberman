using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private float timeToExplode = 2f;
    private float shakeIntensity = 3f;
    private float shakeDuration = 0.2f;

    [SerializeField] private CircleCollider2D explosionRadius;

    private Animator bombAnim;
    public GameManager GManager;

    private void Start()
    {
        GManager = GameManager.GManager;

        bombAnim = this.GetComponent<Animator>();
        StartCoroutine(StartBombTimer());
    }

    public IEnumerator StartBombTimer()
    {
        yield return new WaitForSeconds(timeToExplode);
        explosionRadius.enabled = true;
        bombAnim.SetBool("explode", true);        
        GManager.cameraShake.ShakeCamera(shakeIntensity, shakeDuration);
    }

    private void DestroyBombObj()
    {
        Destroy(this.gameObject);
    }
}
