using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedDotCopy : MonoBehaviour
{
    public Image origin;

    void Start()
    {
        
    }


    void Update()
    {
        if (origin.gameObject.activeSelf == true)
            GetComponent<Image>().color = new Color(1, 0, 0, 1);
        else
            GetComponent<Image>().color = new Color(1, 0, 0, 0);
    }
}
