using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarProcessUpgrade : MonoBehaviour
{

    public RectTransform Bar;
    public RectTransform ProcessBar;
    float _Unit = -1;
    public void SetBar(int value,bool IsAntimat = false)
    {
        if(_Unit==-1)
            _Unit = (Bar.sizeDelta.x) / 10;

        if(IsAntimat)
        {
            Bar.sizeDelta = new Vector2(5* _Unit, Bar.sizeDelta.y);
        }
        else
        {
            Bar.sizeDelta = new Vector2(10 * _Unit, Bar.sizeDelta.y);
        }
        ProcessBar.sizeDelta = new Vector2(value * _Unit, ProcessBar.sizeDelta.y);
    }



}
