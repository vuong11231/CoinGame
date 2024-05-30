////////////////////////////////////////////////////////////////////////////
// bl_AutoRotation
//
//
//                    Lovatto Studio 2016
////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

public class bl_AutoRotation : MonoBehaviour
{
    [Range(1,100)] public float Speed = 2;
    public Vector3 Direction;

    void Update()
    {
        transform.Rotate(Direction * Time.deltaTime * Speed);
    }

}