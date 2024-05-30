using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitController : MonoBehaviour
{
    public SpaceController Manager;
    public LineRenderer Line;
    public Color ColorBlue;
    public Color ColorGreen;
    public Color ColorYellow;
    public Color ColorRed;

    public bool IsDefault;

    [Header("Init")]
    public DrawCircle drawCircle;
    public CircleCollider2D orbitCollider;

    private void Start()
    {
        Line.SetColors(ColorBlue, ColorBlue);
    }


    public void SetLine(bool IsInvalid)
    {
        Line.startWidth = 0.2f;
        if (IsInvalid)
        {
            Line.SetColors(ColorGreen, ColorGreen);
        }
        else
        {
            Line.SetColors(ColorYellow,ColorYellow);
        }
    }
    public void SetLineRed()
    {
        Line.SetColors(ColorRed, ColorRed);
    }

    public void SetLineDefault()
    {
        Line.startWidth = 0.05f;
        Line.SetColors(ColorBlue, ColorBlue);
    }

    public  int CheckPos(Vector3 Pos)
    {
        float min = 1000;
        int Index = 0;
        for(int i=0;i<Line.positionCount;i++)
        {
            float check = Vector3.Distance(Line.GetPosition(i), Pos);
            if (min > check)
            {
                min = check;
                Index = i;
            }
        }
        return Index;
    }
}
