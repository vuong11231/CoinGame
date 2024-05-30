using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class CanvasHelper : MonoBehaviour {


#if UNITY_EDITOR
    int preWidth, preHeight;
#endif

    void Awake()
    {
        var size3_2 = 9f / 16f;
        var sizeScene = UtilityGame.GetScreenDimension();
        var isMatchHeight = sizeScene <= size3_2;
        var canvas = GetComponent<CanvasScaler>();
        canvas.matchWidthOrHeight = isMatchHeight ? 0 : 1;
    }

#if UNITY_EDITOR
    void Update()
    {
        
        if (preWidth != Screen.width || preHeight != Screen.height)
        {
            preWidth = Screen.width;
            preHeight = Screen.height;
            Awake();
        }
    }
#endif
}
