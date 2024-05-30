using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UniverseModel {
    public string jsonlocal;
    public string jsonserver;

    public int userid;
    public int diamond;
    public int destroysolarbyonehit;
    public int destroyedsolars;
    public int randommissionreward;

    public int meteorplanethitcount = 0;
    public int meteorspecialplanethitcount = 0;
    public int meteormulticolorhitcount = 0;

    public int m_airstone = 0;
    public int m_firestone = 0;
    public int m_icestone = 0;
    public int m_gravitystone = 0;
    public int m_antimatterstone = 0;
    public int m_colorfulstone = 0;

    public int m_material;
    public int m_air;
    public int m_fire;
    public int m_ice;
    public int m_gravity;
    public int m_antimatter;

    //public DateTime lastonline;
    public int level = 1;
    public float materialcollect = 0f;
    public string listenemy = "";
    public string name = "";
    public string token = "";
    public string facebookid = "";
    public string googleid = "";
    public string status = "";
}