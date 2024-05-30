using Lean.Pool;
using Newtonsoft.Json;
using SteveRogers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BlackHole : MonoBehaviour
{
    public static BlackHole Instance = null;

    public static List<DataReward> flyingRewards;

    public Sprite[] stoneImages;
    public Image currentImage;
    public Image previousImage;
    public Image nextImage;
    public TextMeshProUGUI[] stoneNumber;
    public GameObject collectObject;
    public GameObject diamond;
    public Text txtLevel;

    public TextMeshProUGUI txtMaterial;
    public TextMeshProUGUI txtDiamond;

    public GameObject vfxBigBang = null;

    [HideInInspector]
    public DataReward reward;

    [HideInInspector]
    public DataReward totalReward;

    public int CurrentAmount {
        get {
            switch (current)
            {
                case 0:
                    return DataGameSave.dataLocal.M_AirStone;
                case 1:
                    return DataGameSave.dataLocal.M_AntimatterStone;
                case 2:
                    return DataGameSave.dataLocal.M_FireStone;
                case 3:
                    return DataGameSave.dataLocal.M_GravityStone;
                case 4:
                    return DataGameSave.dataLocal.M_IceStone;
                case 5:
                    return DataGameSave.dataLocal.M_ColorfulStone;
                case 6:
                    return DataGameSave.dataLocal.M_ToyStone1;
                case 7:
                    return DataGameSave.dataLocal.M_ToyStone2;
                case 8:
                    return DataGameSave.dataLocal.M_ToyStone3;
                case 9:
                    return DataGameSave.dataLocal.M_ToyStone4;
                case 10:
                    return DataGameSave.dataLocal.M_ToyStone5;

                default:
                    return 0;
            }
        }
    }

    [HideInInspector]
    public int current = 0;

    public enum Kind_Of_Stone
    {
        AirStone = 0,
        AntimatterStone,
        FireStone,
        GravityStone,
        IceStone,
        ColorfulStone,
        Toy1,
        Toy2,
        Toy3,
        Toy4,
        Toy5
    }

    public int GetAmount(int current) {
        switch (current)
        {
            case 0:
                return DataGameSave.dataLocal.M_AirStone;
            case 1:
                return DataGameSave.dataLocal.M_AntimatterStone;
            case 2:
                return DataGameSave.dataLocal.M_FireStone;
            case 3:
                return DataGameSave.dataLocal.M_GravityStone;
            case 4:
                return DataGameSave.dataLocal.M_IceStone;
            case 5:
                return DataGameSave.dataLocal.M_ColorfulStone;
            case 6:
                return DataGameSave.dataLocal.M_ToyStone1;
            case 7:
                return DataGameSave.dataLocal.M_ToyStone2;
            case 8:
                return DataGameSave.dataLocal.M_ToyStone3;
            case 9:
                return DataGameSave.dataLocal.M_ToyStone4;
            case 10:
                return DataGameSave.dataLocal.M_ToyStone5;

            default:
                return 0;
        }
    }

    private void Awake()
    {
        Instance = this;

        vfxBigBang.SetActive(false);
        vfxBigBang.GetComponent<ParticleSystem>().Stop();

        totalReward = new DataReward();
        reward = new DataReward();
    }

    private void Start()
    {
        ShowStoneNumber();
        txtLevel.text = "Lv." + DataGameSave.dataServer.level;

        if (flyingRewards == null)
        {
            if (PlayerPrefs.HasKey("BlackHole.flyingRewards"))
            {
                string json = PlayerPrefs.GetString("BlackHole.flyingRewards");
                flyingRewards = JsonConvert.DeserializeObject<List<DataReward>>(json);
            }

            if (flyingRewards == null)
            {
                flyingRewards = new List<DataReward>();
            }
        }

        if (flyingRewards.Count > 0)
        {
            DataReward sum = new DataReward();
            flyingRewards.ForEach((item) => sum.Add(item));

            if (!sum.IsEmpy()) {
                PopupBlackHoleResult.Show("You found", "Great", sum, () => {
                    DataGameSave.SaveToServer();
                    flyingRewards.Clear();
                });
            }
        }


    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("BlackHole.flyingRewards", JsonConvert.SerializeObject(BlackHole.flyingRewards));
        Debug.Log(JsonConvert.SerializeObject(BlackHole.flyingRewards));
    }

    public void ShowNext()
    {
        current = NextInt(current);
        currentImage.sprite = stoneImages[current];
        previousImage.sprite = stoneImages[PreviousInt(current)];
        nextImage.sprite = stoneImages[NextInt(current)];
        ShowStoneNumber();
    }

    public void ShowPrevious()
    {
        current = PreviousInt(current);
        currentImage.sprite = stoneImages[current];
        previousImage.sprite = stoneImages[PreviousInt(current)];
        nextImage.sprite = stoneImages[NextInt(current)];
        ShowStoneNumber();
    }

    public int NextInt(int current)
    {
        current++;
        return (current == stoneImages.Length) ? 0 : current;
    }

    public int PreviousInt(int current)
    {
        current--;
        return (current == -1) ? stoneImages.Length - 1 : current;
    }

    public void ShowStoneNumber()
    {
        for (int i = 0; i < stoneNumber.Length; i++)
        {
            switch (i)
            {
                case 0:
                    stoneNumber[i].text = DataGameSave.dataLocal.M_AirStone.ToString();
                    break;
                case 1:
                    stoneNumber[i].text = DataGameSave.dataLocal.M_AntimatterStone.ToString();
                    break;
                case 2:
                    stoneNumber[i].text = DataGameSave.dataLocal.M_FireStone.ToString();
                    break;
                case 3:
                    stoneNumber[i].text = DataGameSave.dataLocal.M_GravityStone.ToString();
                    break;
                case 4:
                    stoneNumber[i].text = DataGameSave.dataLocal.M_IceStone.ToString();
                    break;
                case 5:
                    stoneNumber[i].text = DataGameSave.dataLocal.M_ColorfulStone.ToString();
                    break;
                case 6:
                    stoneNumber[i].text = DataGameSave.dataLocal.M_ToyStone1.ToString();
                    break;
                case 7:
                    stoneNumber[i].text = DataGameSave.dataLocal.M_ToyStone2.ToString();
                    break;
                case 8:
                    stoneNumber[i].text = DataGameSave.dataLocal.M_ToyStone3.ToString();
                    break;
                case 9:
                    stoneNumber[i].text = DataGameSave.dataLocal.M_ToyStone4.ToString();
                    break;
                case 10:
                    stoneNumber[i].text = DataGameSave.dataLocal.M_ToyStone5.ToString();
                    break;
            }
        }
    }

    public DataReward ConsumeStone()
    {
        switch (current)
        {
            case 0:
                if (DataGameSave.dataLocal.M_AirStone > 0)
                    DataGameSave.dataLocal.M_AirStone -= 1;
                break;
            case 1:
                if (DataGameSave.dataLocal.M_AntimatterStone > 0)
                    DataGameSave.dataLocal.M_AntimatterStone -= 1;
                break;
            case 2:
                if (DataGameSave.dataLocal.M_FireStone > 0)
                    DataGameSave.dataLocal.M_FireStone -= 1;
                break;
            case 3:
                if (DataGameSave.dataLocal.M_GravityStone > 0)
                    DataGameSave.dataLocal.M_GravityStone -= 1;
                break;
            case 4:
                if (DataGameSave.dataLocal.M_IceStone > 0)
                    DataGameSave.dataLocal.M_IceStone -= 1;
                break;
            case 5:
                if (DataGameSave.dataLocal.M_ColorfulStone > 0)
                    DataGameSave.dataLocal.M_ColorfulStone -= 1;
                break;
            case 6:
                if (DataGameSave.dataLocal.M_ToyStone1 > 0)
                    DataGameSave.dataLocal.M_ToyStone1 -= 1;
                break;
            case 7:
                if (DataGameSave.dataLocal.M_ToyStone2 > 0)
                    DataGameSave.dataLocal.M_ToyStone2 -= 1;
                break;
            case 8:
                if (DataGameSave.dataLocal.M_ToyStone3 > 0)
                    DataGameSave.dataLocal.M_ToyStone3 -= 1;
                break;
            case 9:
                if (DataGameSave.dataLocal.M_ToyStone4 > 0)
                    DataGameSave.dataLocal.M_ToyStone4 -= 1;
                break;
            case 10:
                if (DataGameSave.dataLocal.M_ToyStone5 > 0)
                    DataGameSave.dataLocal.M_ToyStone5 -= 1;
                break;
        }

        reward = new DataReward();
        if (current < 8)
        {
            AddMaterialCollect();
            AddDiamond();

            if (current == 5)
                AddMultiColorStone();
            else
                AddColorStone();
        }
        else {
            if (current == 8) {
                reward.diamond = UnityEngine.Random.Range(20, 101);
                reward.material = UnityEngine.Random.Range(20000, 50001);
            }
            if (current == 9)
            {
                reward.diamond = UnityEngine.Random.Range(50, 201);
                reward.material = UnityEngine.Random.Range(50000, 100001);
            }
            if (current == 10)
            {
                reward.diamond = UnityEngine.Random.Range(100, 1001);
                reward.material = UnityEngine.Random.Range(100000, 1000000);
            }
        }

        DataGameSave.dataLocal.M_Material += reward.material;
        DataGameSave.dataLocal.M_Air += reward.air;
        DataGameSave.dataLocal.M_Antimatter += reward.antimater;
        DataGameSave.dataLocal.M_Fire += reward.fire;
        DataGameSave.dataLocal.M_Gravity += reward.gravity;
        DataGameSave.dataLocal.M_Ice += reward.ice;
        DataGameSave.dataLocal.Diamond += reward.diamond;
        //DataGameSave.SaveToLocal();

        DataGameSave.dataServer.MaterialCollect += reward.material;
        //DataGameSave.SaveToServer();

        txtDiamond.text = DataGameSave.dataLocal.Diamond.ToString();
        txtMaterial.text = SteveRogers.Utilities.MoneyShorter(DataGameSave.dataLocal.M_Material, 1);

        totalReward.Add(reward);

        CheckAllStoneConsumed();

        return reward;
    }

    public void SpawnRewardVisual(DataReward reward) {

        //material
        float mat = reward.material;
        int spawnNumber = (int)((float)mat / 5000) + 1;
        int valueEachSpawn = (int)((float)mat / spawnNumber);

        for (int i = 0; i < spawnNumber; i++)
        {
            Vector3 pos = transform.position + new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-2f, 2f));
            StartCoroutine(SpawnMaterial(pos, valueEachSpawn, UnityEngine.Random.Range(0f, 1.2f)));
        }

        //diamond
        mat = reward.diamond;
        //Debug.Log("SPAWN DIAMOND: " + reward.diamond);

        spawnNumber = reward.diamond;
        valueEachSpawn = 1;

        if (reward.diamond > 500) {
            spawnNumber = reward.diamond / 50;
        }
        else if (reward.diamond > 100) {
            spawnNumber = reward.diamond / 10;
        }

        for (int i = 0; i < spawnNumber; i++)
        {
            Vector3 pos = transform.position + new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-2f, 2f));
            StartCoroutine(SpawnDiamondCoroutine(pos, 1, UnityEngine.Random.Range(0f, 1.2f)));
        }

        //color stone
        for (int i = 0; i < reward.air; i++)
        {
            Vector3 pos = transform.position + new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-2f, 2f));
            StartCoroutine(SpawnColorObject("air", pos, reward.air, UnityEngine.Random.Range(0f, 1.2f)));
        }

        for (int i = 0; i < reward.antimater; i++)
        {
            Vector3 pos = transform.position + new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-2f, 2f));
            StartCoroutine(SpawnColorObject("antimater", pos, reward.antimater, UnityEngine.Random.Range(0f, 1.2f)));
        }

        for (int i = 0; i < reward.fire; i++)
        {
            Vector3 pos = transform.position + new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-2f, 2f));
            StartCoroutine(SpawnColorObject("fire", pos, reward.fire, UnityEngine.Random.Range(0f, 1.2f)));
        }

        for (int i = 0; i < reward.gravity; i++)
        {
            Vector3 pos = transform.position + new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-2f, 2f));
            StartCoroutine(SpawnColorObject("gravity", pos, reward.gravity, UnityEngine.Random.Range(0f, 1.2f)));
        }

        for (int i = 0; i < reward.ice; i++)
        {
            Vector3 pos = transform.position + new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-2f, 2f));
            StartCoroutine(SpawnColorObject("ice", pos, reward.ice, UnityEngine.Random.Range(0f, 1.2f)));
        }
    }


    public void AddMaterialCollect()
    {
        int mat = 0;
        int rand = UnityEngine.Random.Range(0, 101);
        if (rand == 0)
        {
            mat = UnityEngine.Random.Range(400000, 600001);
        }
        else if (rand <= 5)
        {
            mat = UnityEngine.Random.Range(200000, 400001);
        }
        else if (rand <= 15)
        {
            mat = UnityEngine.Random.Range(100000, 200001);
        }
        else if (rand <= 30)
        {
            mat = UnityEngine.Random.Range(50000, 100001);
        }
        else
        {
            mat = UnityEngine.Random.Range(20000, 100001);
        }

        reward.material += mat;
    }

    public void AddDiamond()
    {
        int mat = 0;

        if (UnityEngine.Random.Range(0, 11) != 0)
            return;

        int rand = UnityEngine.Random.Range(0, 101);
        if (rand == 0)
        {
            mat = UnityEngine.Random.Range(500, 1001);
        }
        else if (rand <= 5)
        {
            mat = UnityEngine.Random.Range(50, 101);
        }
        else if (rand <= 15)
        {
            mat = UnityEngine.Random.Range(20, 51);
        }
        else if (rand <= 30)
        {
            mat = UnityEngine.Random.Range(10, 21);
        }
        else
        {
            mat = 1;
        }

        reward.diamond += mat;
    }

    public int CalculateColorStoneNumber()
    {
        int mat = 0;

        int rand = UnityEngine.Random.Range(0, 101);
        if (rand == 0)
        {
            mat = UnityEngine.Random.Range(20, 51);
        }
        else if (rand <= 1)
        {
            mat = UnityEngine.Random.Range(5, 11);
        }
        else if (rand <= 2)
        {
            mat = UnityEngine.Random.Range(3, 6);
        }
        else if (rand <= 10)
        {
            mat = 1;
        }
        else
        {
            mat = 0;
        }

        return mat;
    }

    public void AddColorStone()
    {
        // only 25% have stone color effect. May need to adjust
        if (UnityEngine.Random.Range(0, 5) != 0)
            return;

        int mat = CalculateColorStoneNumber();

        switch (current) {
            case 0:
                reward.air += mat;
                break;
            case 1:
                reward.antimater += mat;
                break;
            case 2:
                reward.fire += mat;
                break;
            case 3:
                reward.gravity += mat;
                break;
            case 4:
                reward.ice += mat;
                break;
        }
    }

    public void AddMultiColorStone()
    {
        if (UnityEngine.Random.Range(0, 5) != 0)
            return;

        int mat = CalculateColorStoneNumber();

        int rand = UnityEngine.Random.Range(0, 101);

        if (rand == 0)
        {
            reward.antimater += mat;
        }
        else if (rand <= 35)
        {
            reward.fire += mat;
        }
        else if (rand <= 70)
        {
            reward.ice += mat;
        }
        else if (rand <= 85)
        {
            reward.gravity += mat;
        }
        else
        {
            reward.air += mat;
        }
    }

    public IEnumerator SpawnMaterial(Vector3 pos, int amount, float time)
    {
        yield return new WaitForSeconds(time);
        GameObject spawn = Instantiate(collectObject, pos, Quaternion.identity);
        spawn.transform.localPosition = new Vector3(pos.x, pos.y, -10);
        spawn.GetComponent<CollectMaterialFx>().SetFx((float)amount, TypePlanet.Default);
    }

    public IEnumerator SpawnColorObject(string color, Vector3 pos, int amount, float time)
    {
        yield return new WaitForSeconds(time);
        GameObject spawn = Instantiate(collectObject, pos, Quaternion.identity);
        spawn.transform.localPosition = new Vector3(pos.x, pos.y, -10);
        spawn.GetComponent<CollectMaterialFx>().SetFxMeteor(color);
    }

    public IEnumerator SpawnDiamondCoroutine(Vector3 pos, int amount, float time)
    {
        yield return new WaitForSeconds(time);
        GameObject spawn = Instantiate(diamond, pos, Quaternion.identity);
        spawn.transform.localPosition = new Vector3(pos.x, pos.y, -10);
        spawn.transform.LeanScale(Vector3.one, 0);
        LeanTween.cancel(spawn);
        LeanTween.moveLocal(spawn, new Vector3(9.1f, 23.1f, 50.0f), 3f).setEaseInCubic().setOnComplete(() =>
        {
            LeanPool.Despawn(spawn);
        });
    }

    public void BackGamePlay() {

        PopupConfirm.ShowYesNo("Return", "Do you want to exit Black Hole?", () =>
        {
            if (!totalReward.IsEmpy())
            {
                PopupBlackHoleResult.Show("You found", "Great", totalReward, () =>
                {
                    DataGameSave.SaveToServer();
                    flyingRewards.Clear();
                    //SceneManager.LoadScene(SceneName.Gameplay.ToString());
                    Scenes.ReturnToLastScene(Scenes.CurrentSceneId);
                    //SceneManager.LoadScene(1);
                });
            }
            else
            {
                //SceneManager.LoadScene(SceneName.Gameplay.ToString());
                Scenes.ReturnToLastScene(Scenes.CurrentSceneId);
            }
        });

        //Scenes.ChangeScene(SceneName.Gameplay);
        
    }
    //public void Back()
    //{
    //    PopupConfirm.ShowYesNo("Return", "Do you want to exit Black Hole?", () =>
    //    {
    //        if (!totalReward.IsEmpy())
    //        {
    //            PopupBlackHoleResult.Show("You found", "Great", totalReward, () =>
    //            {
    //                DataGameSave.SaveToServer();
    //                flyingRewards.Clear();
    //                Scenes.ReturnToLastScene(Scenes.CurrentSceneId);
    //                //SceneManager.LoadScene(1);
    //            });
    //        }
    //        else
    //        {
    //            Scenes.ReturnToLastScene(Scenes.CurrentSceneId);
    //        }
    //    });



    public void CheckAllStoneConsumed()
    {
        if (DataGameSave.dataLocal.M_Air == 0 &&
               DataGameSave.dataLocal.M_AntimatterStone == 0 &&
               DataGameSave.dataLocal.M_FireStone == 0 &&
               DataGameSave.dataLocal.M_GravityStone == 0 &&
               DataGameSave.dataLocal.M_IceStone == 0 &&
               DataGameSave.dataLocal.M_ColorfulStone == 0 &&
               DataGameSave.dataLocal.M_ToyStone1 == 0 &&
               DataGameSave.dataLocal.M_ToyStone2 == 0 &&
               DataGameSave.dataLocal.M_ToyStone3 == 0 &&
               DataGameSave.dataLocal.M_ToyStone4 == 0 &&
               DataGameSave.dataLocal.M_ToyStone5 == 0)
        {
            LeanTween.delayedCall(5f, () =>
        {
            if (!totalReward.IsEmpy())
            {
                PopupBlackHoleResult.Show("You found", "Great", totalReward, () =>
                {
                    DataGameSave.SaveToServer();
                    flyingRewards.Clear();
                    //Scenes.ReturnToLastScene(Scenes.CurrentSceneId);
                    //Scenes.ReturnToLastScene(Scenes.CurrentSceneId);
                    SceneManager.LoadScene(SceneName.Gameplay.ToString());
                    //SceneManager.LoadScene(1);
                });
            }
            else
            {
                //Scenes.ReturnToLastScene(Scenes.CurrentSceneId);
                SceneManager.LoadScene(SceneName.Gameplay.ToString());
            }
        
        });
    }

    }



}