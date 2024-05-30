using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {
    // Shake
    public float shakeDuration = 0f;
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    Vector3 originalPos;

    [HideInInspector]
    public bool isActivate = false;

    void Awake()
    {
        //if (t == null)
        //{
        //    camTransform = this.GetComponent(typeof(Transform)) as Transform;
        //}
    }

    void OnEnable()
    {
        originalPos = this.transform.localPosition;
    }

    public void Active(float duration, float amount)
    {
        shakeDuration = duration;
        shakeAmount = amount;
        isActivate = true;
    }

    void Update()
    {
        if (isActivate)
        {
            if (shakeDuration > 0)
            {
                this.transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

                shakeDuration -= Time.deltaTime * decreaseFactor;
            }
            else
            {
                shakeDuration = 0f;
                this.transform.localPosition = originalPos;
                isActivate = false;
            }
        }
    }
}
