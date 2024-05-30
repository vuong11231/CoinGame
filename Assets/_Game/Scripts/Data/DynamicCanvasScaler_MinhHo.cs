using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicCanvasScaler_MinhHo : MonoBehaviour
{
    //Minh.ho: default resolution is 1152 * 2048 ~ 9 * 16
    private float ratio;
    private float default_Ratio;
    private CanvasScaler canvasScaler;

    private void OnEnable()
    {
        canvasScaler = this.GetComponent<CanvasScaler>();
        default_Ratio = 2048 / 1152;
        ratio = Screen.height / Screen.width;
        CheckScaler();
    }

    public void CheckScaler()
    {
        if (ratio > default_Ratio)
        {
            canvasScaler.matchWidthOrHeight = 0;
        }
        else
        {
            canvasScaler.matchWidthOrHeight = 1;
        }
    }
}
