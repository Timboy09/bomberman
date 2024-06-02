using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineCamera;
    private float shakeIntensity;
    private float shakeTimer;
    private float shakeTimerTotal;

    private void Awake()
    {
        cinemachineCamera = this.GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        this.GetComponent<CinemachineVirtualCamera>().Follow = FindObjectOfType<PlayerController>().gameObject.transform;
    }

    public void ShakeCamera(float intensity, float duration)
    {
        CinemachineBasicMultiChannelPerlin perlin = cinemachineCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        shakeIntensity = intensity;
        perlin.m_AmplitudeGain = shakeIntensity;
        perlin.m_FrequencyGain = shakeIntensity;
        shakeTimer = duration;
        shakeTimerTotal = duration;
    }

    private void Update()
    {
        if(shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            CinemachineBasicMultiChannelPerlin perlin = cinemachineCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            perlin.m_AmplitudeGain = Mathf.Lerp(0f, shakeIntensity, shakeTimer / shakeTimerTotal);
            perlin.m_FrequencyGain = Mathf.Lerp(0f, shakeIntensity, shakeTimer / shakeTimerTotal);
        }        
    }
}
