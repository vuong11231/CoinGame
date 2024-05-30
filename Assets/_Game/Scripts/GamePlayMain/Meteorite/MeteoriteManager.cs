using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
public class MeteoriteManager : MonoBehaviour
{
    public GameObject PrefabMeteorite;
    public List<Sprite> SprMeteorite;
    public Sprite SprDiamond = null;

    [Header("time appear")]

    public int TimeOfSmallMeteorialfr = 10;
    public int TimeOfSmallMeteorialto = 30;
    public int TimeOfNormalMeteorialfr = 60;
    public int TimeOfNormalMeteorialto = 70;
    public int TimeOfBigMeteorialfr = 120;
    public int TimeOfBigMeteorialto = 150;

    public int TimeOfSpecialElement = 180;

    public int TimeOfDiamond = 180;

    [Header("value")]

    public int MinSmallMeteorial = 5;
    public int MaxSmallMeteorial = 10;

    public int MinNormalMeteorial = 40;
    public int MaxNormalMeteorial = 50;

    public int MinBigMeteorial = 80;
    public int MaxBigMeteorial = 100;

    public int DiamondValue = 5;

    [Header("other")]

    public int TimeFly = 20;

    public int Value;

    public int ValueScale;

    public static MeteoriteManager Instance = null;

    public static bool wasTut = false;

    private void Start()
    {
        Instance = this;

        wasTut = TutMan.IsDone(TutMan.TUT_KEY_03_FOCUS_GAMEPLAY_METEOR_PLANETS);

        if (TutMan.IsDone(TutMan.TUT_KEY_03_FOCUS_GAMEPLAY_METEOR_PLANETS))
            StartSpawn();
    }

    IEnumerator DelayToSpawnSmall()
    {
        while (true)
        {
            float Time = Random.Range(TimeOfSmallMeteorialfr, TimeOfSmallMeteorialto);
            yield return new WaitForSecondsRealtime(Time);
            SpawnSmall();
        }
    }

    void SpawnSmall()
    {
        float angle, angle2;
        //Isleft = !Isleft;
        if (Random.Range(0,2)==0)
        {
            angle = Random.Range(70 , 110f);
            angle2 = Random.Range(250, 290f);
        }
        else
        {
            angle2 = Random.Range(250, 290f);
            angle = Random.Range(70, 110f);
        }

        float x = Mathf.Sin(Mathf.Deg2Rad * angle) * 100;
        float y = Mathf.Cos(Mathf.Deg2Rad * angle) * 40 + Random.Range(-50, 50);

        float a = Mathf.Sin(Mathf.Deg2Rad * angle2) * 40;
        float b = Mathf.Cos(Mathf.Deg2Rad * angle2) * 40 + Random.Range(-50, 50);

        Vector3 StartPos = new Vector3(x, y, -30);
        Vector3 DesPos = new Vector3(a, b, -30);

        MeteoriteController newMeteorite = LeanPool.Spawn(PrefabMeteorite, StartPos, Quaternion.identity, transform).GetComponent<MeteoriteController>();

        newMeteorite.Move(TimeFly, DesPos);
        newMeteorite.Change(SprMeteorite[5]);
        newMeteorite.SetData((TypeMaterial.Default),TypeSize.Small, Random.Range(MinSmallMeteorial, MaxSmallMeteorial));

    }


    IEnumerator DelayToSpawnNormal ()
    {
        while (true)
        {
            float Time = Random.Range(TimeOfNormalMeteorialfr, TimeOfNormalMeteorialto);
            yield return new WaitForSecondsRealtime(Time);
            SpawnNormal();
        }
    }

    void SpawnNormal()
    {
        float angle, angle2;
        //  Isleft = !Isleft;
        if (Random.Range(0, 2) == 0)
        {
            angle = Random.Range(70, 110f);
            angle2 = Random.Range(250, 290f);
        }
        else
        {
            angle2 = Random.Range(250, 290f);
            angle = Random.Range(70, 110f);
        }

        float x = Mathf.Sin(Mathf.Deg2Rad * angle) * 100;
        float y = Mathf.Cos(Mathf.Deg2Rad * angle) * 40 + Random.Range(-50, 50);

        float a = Mathf.Sin(Mathf.Deg2Rad * angle2) * 40;
        float b = Mathf.Cos(Mathf.Deg2Rad * angle2) * 40 + Random.Range(-50, 50);

        Vector3 StartPos = new Vector3(x, y, -30);
        Vector3 DesPos = new Vector3(a, b, -30);

        MeteoriteController newMeteorite = LeanPool.Spawn(PrefabMeteorite, StartPos, Quaternion.identity, transform).GetComponent<MeteoriteController>();

        newMeteorite.Move(TimeFly, DesPos);
        newMeteorite.Change(SprMeteorite[5]);
        newMeteorite.SetData((TypeMaterial.Default), TypeSize.Normal, Random.Range(MinNormalMeteorial, MaxNormalMeteorial));

    }

    IEnumerator DelayToSpawnBig()
    {
        while (true)
        {
            float Time = Random.Range(TimeOfBigMeteorialfr, TimeOfBigMeteorialto);
            yield return new WaitForSecondsRealtime(Time);
            SpawnBig();
        }
    }

    void SpawnBig()
    {
        float angle, angle2;
        if (Random.Range(0, 2) == 0)
        {
            angle = Random.Range(70, 110f);
            angle2 = Random.Range(250, 290f);
        }
        else
        {
            angle2 = Random.Range(250, 290f);
            angle = Random.Range(70, 110f);
        }

        float x = Mathf.Sin(Mathf.Deg2Rad * angle) * 100;
        float y = Mathf.Cos(Mathf.Deg2Rad * angle) * 40 + Random.Range(-50, 50);

        float a = Mathf.Sin(Mathf.Deg2Rad * angle2) * 40;
        float b = Mathf.Cos(Mathf.Deg2Rad * angle2) * 40 + Random.Range(-50, 50);

        Vector3 StartPos = new Vector3(x, y, -30);
        Vector3 DesPos = new Vector3(a, b, -30);

        MeteoriteController newMeteorite = LeanPool.Spawn(PrefabMeteorite, StartPos, Quaternion.identity, transform).GetComponent<MeteoriteController>();

        newMeteorite.Move(TimeFly, DesPos);
        newMeteorite.Change(SprMeteorite[5]);
        newMeteorite.SetData((TypeMaterial.Default), TypeSize.Big, Random.Range(MinBigMeteorial, MaxBigMeteorial));

    }


    IEnumerator DelayToSpawnSpecial()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(TimeOfSpecialElement);
            SpawnSpec();
        }
    }

    IEnumerator DelayToSpawnDiamond()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(TimeOfDiamond);
            SpawnDiamond();
        }
    }

    void SpawnDiamond()
    {
        float angle, angle2;
        if (Random.Range(0, 2) == 0)
        {
            angle = Random.Range(70, 110f);
            angle2 = Random.Range(250, 290f);
        }
        else
        {
            angle2 = Random.Range(250, 290f);
            angle = Random.Range(70, 110f);
        }

        float x = Mathf.Sin(Mathf.Deg2Rad * angle) * 100;
        float y = Mathf.Cos(Mathf.Deg2Rad * angle) * 40 + Random.Range(-50, 50);

        float a = Mathf.Sin(Mathf.Deg2Rad * angle2) * 40;
        float b = Mathf.Cos(Mathf.Deg2Rad * angle2) * 40 + Random.Range(-50, 50);

        Vector3 StartPos = new Vector3(x, y, -30);
        Vector3 DesPos = new Vector3(a, b, -30);

        MeteoriteController newMeteorite = LeanPool.Spawn(PrefabMeteorite, StartPos, Quaternion.identity, transform).GetComponent<MeteoriteController>();
        newMeteorite.Move(TimeFly, DesPos);        
        newMeteorite.Change(SprDiamond);
        newMeteorite.SetData(TypeMaterial.Default, TypeSize.Small, DiamondValue, true);
    }

    void SpawnSpec()
    {
        float angle, angle2;
        if (Random.Range(0, 2) == 0)
        {
            angle = Random.Range(70, 110f);
            angle2 = Random.Range(250, 290f);
        }
        else
        {
            angle2 = Random.Range(250, 290f);
            angle = Random.Range(70, 110f);
        }

        float x = Mathf.Sin(Mathf.Deg2Rad * angle) * 100;
        float y = Mathf.Cos(Mathf.Deg2Rad * angle) * 40 + Random.Range(-50, 50);

        float a = Mathf.Sin(Mathf.Deg2Rad * angle2) * 40;
        float b = Mathf.Cos(Mathf.Deg2Rad * angle2) * 40 + Random.Range(-50, 50);

        Vector3 StartPos = new Vector3(x, y, -30);
        Vector3 DesPos = new Vector3(a, b, -30);

        MeteoriteController newMeteorite = LeanPool.Spawn(PrefabMeteorite, StartPos, Quaternion.identity, transform).GetComponent<MeteoriteController>();
        newMeteorite.Move(TimeFly, DesPos);
        int value = Random.Range(0, SprMeteorite.Count - 1);
        newMeteorite.Change(SprMeteorite[value]);
        newMeteorite.SetData((TypeMaterial)value, TypeSize.Small, 1);
    }

    public void StartSpawn() // main 
    {
        if (!TutMan.IsDone(TutMan.TUT_KEY_03_FOCUS_GAMEPLAY_METEOR_PLANETS))
            return;

        StartCoroutine(DelayToSpawnSmall());
        StartCoroutine(DelayToSpawnNormal());
        StartCoroutine(DelayToSpawnBig());
        StartCoroutine(DelayToSpawnSpecial());
        StartCoroutine(DelayToSpawnDiamond());
    }
}
