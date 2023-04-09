using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class ScreenShaker : MonoBehaviour {
 
    public float shakeAmount = 10f;
 
    private float shakeTime = 0.0f;
    private Vector3 initialPosition;
    private bool isScreenShaking = false;
 
    //Code adapted from Camera Vibration in Canvas Based Unity Game · Yuno's Wonderland
 
    // Use this for initialization
    void Start () {
         
    }
     
    // Update is called once per frame
    void Update () {
        if(shakeTime > 0)
        {
            this.transform.position = Random.insideUnitSphere * shakeAmount + initialPosition;
            shakeTime -= Time.deltaTime;
        }
        else if(isScreenShaking)
        {
            isScreenShaking = false;
            shakeTime = 0.0f;
            this.transform.position = initialPosition;
        }
    }
 
    public void ScreenShakeForTime(float time)
    {
        if(!GameSystemManager.CancelShake)
        {
            return;
        }
        initialPosition = this.transform.position;
        shakeTime = time;
        isScreenShaking = true;
    }
}