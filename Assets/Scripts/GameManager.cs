using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GManager;

    public PlayerController playerController;
    public CameraShake cameraShake;
    public UIManager uiCanvas;
    public Door levelDoor;

    private void Awake()
    {
        if(GManager == null)
        {
            GManager = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        cameraShake = FindObjectOfType<CameraShake>();
        uiCanvas = FindObjectOfType<UIManager>();
        levelDoor = FindObjectOfType<Door>();
    }
}
