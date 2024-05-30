using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMouseDownMeteor : MonoBehaviour
{
    private void OnMouseDown()
    {
        var m = transform.parent?.GetComponent<MeteoriteController>();

        if (m)
            m.actiondown();
    }
}
