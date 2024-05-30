using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using SteveRogers;

public class SpaceController : MonoBehaviour
{
    public OrbitController Orbit;
    public PlanetController Planet;
    public List<GameObject> PrefabPlanet;
    public GameObject prefabTVHT;

    public int IndexSpace;
    public bool IsEnemy;

    public bool IsHavePlanet;
    public bool IsShow;

    [HideInInspector]
    public float SpaceRadius;

    public float Radius
    {
        get
        {
            float dis = 2;
            var data = IsEnemy ? DataGameSave.dataEnemy : DataGameSave.dataServer;

            for (int i = 0; i < IndexSpace && i < data.ListPlanet.Count; i++)
            {
                dis += data.ListPlanet[i].GetSize() / GameManager.Instance.devidePlanetDistance;
            }

            if (IndexSpace < data.ListPlanet.Count)
                dis += data.ListPlanet[IndexSpace].GetSize() / GameManager.Instance.devidePlanetDistance / 100;

            return DataScriptableObject.Instance.sunDistance + (IndexSpace * DataScriptableObject.Instance.planetsDistance) + dis;
        }
    }

    public void UpdateRadius(bool isEnemy)
    {
        IsEnemy = isEnemy;
        var radius = Radius;

        Orbit.orbitCollider.radius = radius;
        Orbit.drawCircle._segments = 100 + (IndexSpace * 40);
        Orbit.drawCircle._horizRadius = radius;
        Orbit.drawCircle._vertRadius = radius;

        SpaceRadius = radius;
    }

    public void SpawnPlanetDefault(DataPlanet Data, int OrbitIndex, bool isEnemy = false)
    {
        IsEnemy = isEnemy;
        var rand = Random.Range(0, Orbit.Line.positionCount - 1);

        Planet = LeanPool.Spawn(PrefabPlanet[(int)TypePlanet.Default], transform).GetComponent<PlanetController>();

        Spawn(rand, true, isEnemy);
        LeanTween.cancel(Planet.gameObject);
        Planet.transform.localScale = Vector3.zero;
        Planet.SetData(Data, OrbitIndex, isEnemy);

        if (isEnemy) {
            //this is magic
            //Planet.Gravitation = .05f * DatabasePlanet.Instance.ListDataPlanetDefault[Data.LevelHeavier].Gravity;
            Planet.Gravitation = .003f * Data.GetGravity();
            if (Planet.Type == TypePlanet.Gravity) {
                //Debug.LogError(Data.LevelElement - 1);
                Planet.Gravitation += .003f * DatabasePlanet.Instance.ListGravityPlanet[Data.LevelElement].UpGravity; //-1
            }
            //ban kinh luc hut hanh tinh
            Planet.GetComponent<CircleCollider2D>().radius = ((float)3 / transform.localScale.x) * 8;

            if (Planet.Type == TypePlanet.Gravity) {
                Planet.GetComponent<CircleCollider2D>().radius *= DatabasePlanet.Instance.ListGravityPlanet[Data.LevelElement].SizeGravity;
            }
        }

        IsHavePlanet = true;
    }

    public void Spawn(int index, bool Default = false, bool isEnemy = false)
    {
        IsEnemy = isEnemy;
        Planet.SpawnPos(index, Orbit.Line, this, Default, isEnemy);
    }

    public void SpawnPlanet(TypePlanet Type, Vector3 Pos, bool isEnemy = false)
    {
        IsEnemy = isEnemy;

        Planet = LeanPool.Spawn(PrefabPlanet[(int)TypePlanet.Default], transform).GetComponent<PlanetController>();

        Spawn(Orbit.CheckPos(Pos), false, isEnemy);
        IsHavePlanet = true;
    }

    public bool Check()
    {
        Orbit.SetLine(!IsHavePlanet);
        if (Planet.Type == TypePlanet.Destroy)
            Orbit.SetLineRed();
        // return (!IsHavePlanet && IsShow);
        return (IsShow && Planet.Type != TypePlanet.Destroy);
    }

    public void ColectMeteorite(TypeMaterial Type, int value)
    {
        value *= DataGameSave.dataLocal.itemX2.multiplyNumber;
        switch (Type)
        {
            case TypeMaterial.Air:
                {
                    DataGameSave.dataLocal.M_Air += value;
                    break;
                }

            case TypeMaterial.Antimatter:
                {
                    DataGameSave.dataLocal.M_Antimatter += value;
                    break;
                }

            case TypeMaterial.Default:
                {
                    DataGameSave.dataLocal.M_Material += value;
                    break;
                }

            case TypeMaterial.Fire:
                {
                    DataGameSave.dataLocal.M_Fire += value;
                    break;
                }

            case TypeMaterial.Gravity:
                {
                    DataGameSave.dataLocal.M_Gravity += value;
                    break;
                }

            case TypeMaterial.Ice:
                {
                    DataGameSave.dataLocal.M_Ice += value;
                    break;
                }
        }
        MoneyManager.Instance.UpdateMoneyDisplay();
        // Achievement
        if (Type != TypeMaterial.Default)
            DataGameSave.dataLocal.CollectElementMeterior++;
    }
}
