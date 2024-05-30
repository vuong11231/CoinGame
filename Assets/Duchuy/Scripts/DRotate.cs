using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DRotate : MonoBehaviour
{
    public float angle = 1f;

    public bool smoothSync = false;

    void Update()
    {
        if (smoothSync)
        {
            transform.Rotate(new Vector3(0, 0, angle*Time.deltaTime));
        }
        else
        {
            transform.Rotate(new Vector3(0, 0, angle));
        }
    }
}
