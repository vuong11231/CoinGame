using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SteveRogers;
using Newtonsoft.Json.Converters;
using UnityEngine.SceneManagement;

public class SpaceEnemyManager : MonoBehaviour
{
	public static SpaceEnemyManager Instance;

	public static Vector2 EnemyPosition
    {
        get
        {
            if (SpaceEnemyManager.Instance && SpaceEnemyManager.Instance.MainPlanet)
                return SpaceEnemyManager.Instance.MainPlanet.transform.position;
            else
                return Vector2.zero;
        }
    }

	public static float GravityRadius;
	public SpaceController spacePrefab;
	public DataBaseSun Data;
	public MainPlanetController MainPlanet;
	public Image FxDestroy;

	List<SpaceController> listSpace;	
	int userIndex = -1;

	public int Level
	{
		get { return DataGameSave.dataEnemy.level; }
		
		set
		{
			if (DataGameSave.dataEnemy.level != value)
				DataGameSave.dataEnemy.level = value;
		}
	}

	public List<SpaceController> ListSpace
	{
		get { return listSpace; }
	}

    public float SpaceRadius
    {
        get
        {
            if (ListSpace == null || ListSpace.IsNullOrEmpty() || !ListSpace[ListSpace.Count - 1].Planet)
                return -1;

            float lastRadius = Vector3.Distance(ListSpace[ListSpace.Count - 1].Planet.transform.position, transform.position);

            //if (SceneManager.GetActiveScene().name == SceneName.BattlePassGameplay.ToString()) {
            //    lastRadius += 20f;
            //}

            return lastRadius;
        }
    }

    public float LastPlanetRadius
    {
        get
        {
            if (ListSpace.IsNullOrEmpty())
                return -1;

            return ListSpace[ListSpace.Count - 1].Radius;
        }
    }

    void Awake()
	{
		Instance = this;
	}

    public void Start() {
        // Set solar system's starting position in battle mode

        var orbitNumber = ReadDataSun.Instance.ListDataSun.GetLastIfOverRange(Level - 1).AmountOrbit;

        if (orbitNumber > 5)
            transform.position = new Vector3(transform.position.x, 3 + (orbitNumber * 2));
        StartCoroutine(InitListSpace());
        SetData();

        // Lực hút mặt trời
        GravityRadius = 20 + (orbitNumber * 6);
        if (CameraManager.Instance && Level > DataGameSave.dataServer.level)
            CameraManager.Instance.SetSize(Data.AmountOrbit);

        PopupBattle.Show(DataGameSave.dataEnemy, GameStatics.EnemyPlayfabId);

        // Update boss interface
        if (SceneManager.GetActiveScene().name == SceneName.BattlePassGameplay.ToString()) {

            //MainPlanet.transform.localScale = new Vector3(GameManager.BOSS_SIZE, GameManager.BOSS_SIZE);
            //MainPlanet.sunParticleParent.gameObject.SetActive(false);
            //MainPlanet.Sun.enabled = true;
        }
    }

    public IEnumerator InitListSpace()
	{
		Debug.Assert(DataGameSave.dataEnemy != null, "DataGameSave.dataEnemy == null at InitListSpace");
		Debug.Assert(DataGameSave.dataEnemy.ListPlanet != null, "DataGameSave.dataEnemy.ListPlanet == null at InitListSpace");

        if (DataGameSave.dataEnemy.ListPlanet == null)
        {
            DataGameSave.dataEnemy.ListPlanet = new List<DataPlanet>();
            DataGameSave.dataEnemy.ListPlanet.Add(new DataPlanet());
        }

		listSpace = new List<SpaceController>();
		var count = DataGameSave.dataEnemy.ListPlanet.Count;

		for (int i = 0; i < count; i++)
		{
            DrawOrbit(true, i);
            yield return new WaitForSeconds(0.25f);

            //Debug.Assert(DataGameSave.dataEnemy != null && DataGameSave.dataEnemy.ListPlanet != null, "DataGameSave.dataEnemy null");

            if (i < DataGameSave.dataEnemy.ListPlanet.Count)
                listSpace[i].SpawnPlanetDefault(DataGameSave.dataEnemy.ListPlanet[i], i, true);
            else
                listSpace[i].SpawnPlanet(TypePlanet.Default, listSpace[i].Orbit.Line.GetPosition(0), true);
        }
        GameStatics.IsAnimating = false;

        if (CameraManager.Instance)
            CameraManager.Instance.SetSize(Data.AmountOrbit);
    }

	void DrawOrbit(bool isInit, int count)
	{
		var space = SpaceController.Instantiate(spacePrefab, transform);
		listSpace.Add(space);
		listSpace[count].IsShow = true;		
		space.IndexSpace = count;
        space.UpdateRadius(true);
		space.Orbit.drawCircle.enabled = true;
    }

	void SetData()
	{
		Data = ReadDataSun.Instance.ListDataSun.GetLastIfOverRange(Level - 1);
		MainPlanet.SetData(Data);
	}
}
