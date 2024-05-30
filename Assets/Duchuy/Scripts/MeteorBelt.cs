using Lean.Pool;
using SteveRogers;
using System;
//using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TMPro;

public class MeteorBelt : MonoBehaviour {
    public static MeteorBelt Instance;

    public static float CAMERA_Z = -50;
    public static int METEORITE_NUMBER = 300;
    public static int MONO_COLOR_NUMBER = 10;
    public static int MULTI_COLOR_NUMBER = 5;
    public static int BIG_METEOR_EACH_NUMBER = 4;
    public static int DIAMOND_NUMBER = 20;

    public static float SUN_RADIUS = 100f;
    public static float SPAWN_RANGE = 300f;

    public static float MIN_SPIN_RATE = 0.2f;
    public static float MAX_SPIN_RATE = 1f;

    public static int HP_METEORITE_SMALL = 30;
    public static int HP_METEORITE_MEDIUM = 60;
    public static int HP_METEORITE_BIG = 100;

    public static int DAME_METEORITE_SMALL = 30;
    public static int DAME_METEORITE_MEDIUM = 60;
    public static int DAME_METEORITE_BIG = 100;

    //lượng nhận thưởng thiên thạch
    public static int REWARD_METEORITE_SMALL = 100;
    public static int REWARD_METEORITE_MEDIUM = 200;
    public static int REWARD_METEORITE_BIG = 500;

    public static float METEORITE_GRAVITY_FORCE = 100f;
    public static float EXPLODE_METEOR_FORCE = 500f;

    public static float BIG_METEOR_MIN_DISTANCE = 100f;
    public static float BIG_METEOR_MAX_DISTANCE = 300f;

    public static float BIG_METEOR_MIN_EXPLODE_FORCE = 1000f;
    public static float BIG_METEOR_MAX_EXPLODE_FORCE = 2500f;
    public static float BIG_METEOR_EXPLODE_DURATION = 5f;

    public static List<MeteorBeltItem> listMeteors;
    public static List<BattlePassCellData> meteorFallDatas;

    public static bool isMeteorFall = true;
    public static float METEOR_FALL_SPEED = 30f;
    public static float METEOR_FALL_LIMIT = 200f;
    public static float METEOR_FALL_CAMERA_SIZE_MULTIPLE = 1.5f;

    public SpaceManager spaceMananger;

    public List<GameObject> stones;
    public List<GameObject> effectDestroys;

    public List<float> maxHps;

    public Transform hpValue;
    public GameObject sun;
    public GameObject meteor;
    public GameObject appearEffect;
    public GameObject explodeEffect;
    public GameObject btnTakeNoDame;
    public DCamera camera;

    public GameObject air;
    public GameObject antimater;
    public GameObject fire;
    public GameObject gravity;
    public GameObject ice;
    public GameObject colorful;
    public GameObject diamond;

    public GameObject rewardMaterial;
    public GameObject rewardAir;
    public GameObject rewardAntimater;
    public GameObject rewardFire;
    public GameObject rewardGravity;
    public GameObject rewardIce;
    public GameObject rewardColorful;
    public GameObject rewardDiamond;
    public GameObject rewardToy1;
    public GameObject rewardToy2;
    public GameObject rewardToy3;
    public GameObject rewardToy4;
    public GameObject rewardToy5;

    public GameObject bigMeteor1;
    public GameObject bigMeteor2;

    public GameObject explodeObject;
    public TextMeshProUGUI txtCountDown;
    public TextMeshProUGUI txtWave;

    public DataReward reward;
    public DataReward shootRewardCount;

    public DCamera cameraControl;
    public CameraManager cameraManage;

    public GameObject shootingPlanet;

    public Text textLevel;

    public bool pausing = false;

    [HideInInspector]
    public int shootingIndex;

    bool spawnMeteor = false;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        textLevel.text = "Lv." + DataGameSave.dataServer.level.ToString();
        txtCountDown.text = "";
        txtWave.text = "";
        btnTakeNoDame.SetActive(false);
        GameStatics.IsAnimating = false;
        GameManager.planetSpeedRatio = 1f;
        GameManager.takeNoDame = false;

        if (isMeteorFall) {
            for (int i = 0; i < DataGameSave.dataServer.ListPlanet.Count; i++) {
                DataGameSave.dataServer.ListPlanet[i].ShootTime = DateTime.MinValue;
                DataGameSave.dataServer.ListPlanet[i].Type = TypePlanet.Default;
            }
        }

        hpValue.transform.parent.gameObject.SetActive(isMeteorFall);
    }

    private void SetupMeteorBeltLevel() {
        if (isMeteorFall) {
            // chỉ số meteor fall event
            METEORITE_NUMBER = 100;
            MONO_COLOR_NUMBER = 2;
            MULTI_COLOR_NUMBER = 2;
            BIG_METEOR_EACH_NUMBER = 0;
            DIAMOND_NUMBER = 2;

            //REWARD_METEORITE_SMALL = 100;
            //REWARD_METEORITE_MEDIUM = 200;
            //REWARD_METEORITE_BIG = 300;

            //METEORITE_NUMBER = 300;
            //MONO_COLOR_NUMBER = 4;
            //MULTI_COLOR_NUMBER = 4;
            //BIG_METEOR_EACH_NUMBER = 1;
            //DIAMOND_NUMBER = 5;

            REWARD_METEORITE_SMALL = 3000;
            REWARD_METEORITE_MEDIUM = 6000;
            REWARD_METEORITE_BIG = 10000;

            return;
        }

        if (GameManager.meteorBeltLevel == 1) {
            METEORITE_NUMBER = 300;
            MONO_COLOR_NUMBER = 4;
            MULTI_COLOR_NUMBER = 4;
            BIG_METEOR_EACH_NUMBER = 1;
            DIAMOND_NUMBER = 5;

            REWARD_METEORITE_SMALL = 1000;
            REWARD_METEORITE_MEDIUM = 2000;
            REWARD_METEORITE_BIG = 5000;
        } else if (GameManager.meteorBeltLevel == 2) {
            METEORITE_NUMBER = 300;
            MONO_COLOR_NUMBER = 10;
            MULTI_COLOR_NUMBER = 10;
            BIG_METEOR_EACH_NUMBER = 1;
            DIAMOND_NUMBER = 5;

            REWARD_METEORITE_SMALL = 2000;
            REWARD_METEORITE_MEDIUM = 4000;
            REWARD_METEORITE_BIG = 10000;
        } else if (GameManager.meteorBeltLevel == 3) {
            METEORITE_NUMBER = 300;
            MONO_COLOR_NUMBER = 10;
            MULTI_COLOR_NUMBER = 10;
            BIG_METEOR_EACH_NUMBER = 10;
            DIAMOND_NUMBER = 20;

            REWARD_METEORITE_SMALL = 4000;
            REWARD_METEORITE_MEDIUM = 8000;
            REWARD_METEORITE_BIG = 20000;
        } else {
            // chỉ số gốc lúc chưa sửa
            METEORITE_NUMBER = 300;
            MONO_COLOR_NUMBER = 10;
            MULTI_COLOR_NUMBER = 5;
            BIG_METEOR_EACH_NUMBER = 4;
            DIAMOND_NUMBER = 20;

            REWARD_METEORITE_SMALL = 100;
            REWARD_METEORITE_MEDIUM = 200;
            REWARD_METEORITE_BIG = 300;
        }
    }

    private void Update() {
        if (!spawnMeteor && spaceMananger.ListSpace != null && spaceMananger.ListSpace.Count != 0) {
            if (spaceMananger.ListSpace.Last().Planet != null) {

                if (isMeteorFall) {
                    StartCoroutine(MeteorFallCoroutine());
                } else {
                    SetupMeteorBeltLevel();
                    CreateMeteorBelt();
                }
                spawnMeteor = true;
            }
        }
    }

    IEnumerator MeteorFallCoroutine() {
        int wave = 0;
        float CAMERA_SIZE_MULTIPLE = 1.5f;

        Camera.main.orthographicSize = Camera.main.orthographicSize * CAMERA_SIZE_MULTIPLE;

        maxHps = new List<float>();

        for (int i = 0; i < DataGameSave.dataBattle.ListPlanet.Count; i++) {
            maxHps.Add(DataGameSave.dataBattle.ListPlanet[i].HP);
        }

        while (true) {
            if (!spawnMeteor) {
                yield return new WaitForSeconds(1);
            }

            SetupMeteorFallDatas();

            // start a wave
            txtWave.text = TextMan.Get("Wave ") + (wave + 1);

            METEORITE_NUMBER = meteorFallDatas[wave].meteorSmall;
            MONO_COLOR_NUMBER = meteorFallDatas[wave].meteorColor;
            BIG_METEOR_EACH_NUMBER = meteorFallDatas[wave].meteorGiant;
            DIAMOND_NUMBER = meteorFallDatas[wave].meteorDiamond;

            CreateMeteorBelt();

            for (int i = 0; i < listMeteors.Count; i++) {
                Vector3 rand = new Vector3(Random.Range(-METEOR_FALL_LIMIT, METEOR_FALL_LIMIT), Random.Range(-METEOR_FALL_LIMIT, METEOR_FALL_LIMIT));
                listMeteors[i].transform.position = rand + new Vector3(METEOR_FALL_LIMIT, METEOR_FALL_LIMIT) * 2;
            }

            GameManager.sunHp = GameManager.METEOR_FALL_HP;

            float count = GameManager.METEOR_FALL_WAVE_TIME_START + GameManager.METEOR_FALL_WAVE_TIME_INCREASE_PER_LEVEL * DataGameSave.dataServer.level + wave * GameManager.METEOR_FALL_WAVE_TIME_INCREASE_PER_WAVE;
            float willComeIn = 5f;

            while (count >= 0) {
                foreach (var meteor in listMeteors) {
                    meteor.GetComponent<Rigidbody2D>().velocity = new Vector3(-METEOR_FALL_SPEED, -METEOR_FALL_SPEED);
                    if (meteor.transform.position.x < -METEOR_FALL_LIMIT || meteor.transform.position.y < -METEOR_FALL_LIMIT) {
                        Vector3 rand = new Vector3(Random.Range(-METEOR_FALL_LIMIT, METEOR_FALL_LIMIT), Random.Range(-METEOR_FALL_LIMIT, METEOR_FALL_LIMIT));
                        meteor.transform.position = rand + new Vector3(METEOR_FALL_LIMIT, METEOR_FALL_LIMIT);
                    }
                }

                count -= Time.deltaTime;
                willComeIn -= Time.deltaTime;

                if (willComeIn > 0) {
                    txtCountDown.text = string.Format(TextMan.Get("Meteor Fall will come in {0}"), (int)willComeIn);
                } else {
                    txtCountDown.text = string.Format(TextMan.Get("Survive in {0} seconds to win"), (int)count);
                }

                yield return new WaitForEndOfFrame();
            }

            for (int i = 0; i < listMeteors.Count; i++) {
                Vector3 rand = new Vector3(Random.Range(-METEOR_FALL_LIMIT, METEOR_FALL_LIMIT), Random.Range(-METEOR_FALL_LIMIT, METEOR_FALL_LIMIT));
                listMeteors[i].transform.position = rand + new Vector3(METEOR_FALL_LIMIT, METEOR_FALL_LIMIT) * 2;
            }

            //choose buff
            foreach (var meteor in listMeteors) {
                meteor.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            }

            PopupMeteorBuff.selectedBuff = false;
            PopupCustom.Show("Prefabs/Pop-ups/Custom/Popup Meteor Buff");

            yield return new WaitUntil(() => PopupMeteorBuff.selectedBuff);

            wave++;
            int.TryParse(DataGameSave.GetMetaData(MetaDataKey.METEOR_FALL_WIN_MAX_COUNT), out int maxWin);
            if (wave + 1 > maxWin) {
                DataGameSave.SaveMetaData(MetaDataKey.METEOR_FALL_WIN_MAX_COUNT, (wave + 1).ToString());
            }

            if (wave == 50) {
                ShowFinalResult();
                break;
            }

            GameManager.executeFinish = false;
        }
    }

    public void CreateMeteorBelt() {
        try {
            Vector3 lastPlanetPos = spaceMananger.ListSpace.Last().Planet.transform.position;
            SUN_RADIUS = Vector3.Distance(sun.transform.position, lastPlanetPos) + 6f;
        } catch { }

        listMeteors = new List<MeteorBeltItem>();
        SpawnMeteorite();
        SpawnSpecialMeteorite();

        for (int i = 0; i < BIG_METEOR_EACH_NUMBER; i++) {
            SpawnBigMeteor(bigMeteor1, GameManager.bigMeteorHp1);
        }

        for (int i = 0; i < BIG_METEOR_EACH_NUMBER; i++) {
            SpawnBigMeteor(bigMeteor2, GameManager.bigMeteorHp2);
        }
    }

    private void SpawnBigMeteor(GameObject src, float hp) {
        Vector3 pos = GetRandomPosition(BIG_METEOR_MIN_DISTANCE, BIG_METEOR_MAX_DISTANCE);
        GameObject spawn = CreateMeteorObject(src, pos);

        DataReward reward = new DataReward(
            UnityEngine.Random.Range(90000, 110000),
            UnityEngine.Random.Range(1, 3),
            UnityEngine.Random.Range(1, 3),
            UnityEngine.Random.Range(1, 3),
            UnityEngine.Random.Range(1, 3),
            UnityEngine.Random.Range(1, 3),
            UnityEngine.Random.Range(80, 101),
            UnityEngine.Random.Range(1, 3),
            0, 0, 0, 0, 0);
        spawn.GetComponent<MeteorBeltItem>().SetData((int)hp, 99999, reward);
    }

    private float RandomSign() {
        if (UnityEngine.Random.Range(0, 2) == 0) {
            return -1;
        }

        return 1;
    }

    public Vector3 GetRandomPosition(float minDistance, float maxDistance) {
        Vector3 pos;

        while (true) {
            pos = transform.position = UnityEngine.Random.insideUnitCircle * maxDistance;
            if (Vector3.Distance(pos, sun.transform.position) > minDistance)
                break;
        }
        return pos;
    }

    public GameObject CreateMeteorObject(GameObject source, Vector3 pos) {
        GameObject spawn = Instantiate(source, pos, Quaternion.identity);

        var meteor = spawn.GetComponent<MeteorBeltItem>();
        meteor.meteorBelt = this;

        DataReward reward = meteor.GetComponent<MeteorBeltItem>().reward;

        reward.toy5 = (UnityEngine.Random.Range(1, 1000) <= 1) ? 1 : 0;
        reward.toy4 = (UnityEngine.Random.Range(1, 1000) <= 2) ? 1 : 0;
        reward.toy3 = (UnityEngine.Random.Range(1, 1000) <= 5) ? 1 : 0;
        reward.toy2 = (UnityEngine.Random.Range(1, 1000) <= 10) ? 1 : 0;
        reward.toy1 = (UnityEngine.Random.Range(1, 1000) <= 20) ? 1 : 0;

        if (UnityEngine.Random.Range(0, 2) == 0) {
            spawn.GetComponent<DRotate>().angle = UnityEngine.Random.Range(MIN_SPIN_RATE, MAX_SPIN_RATE);
        } else {
            spawn.GetComponent<DRotate>().angle = UnityEngine.Random.Range(-MAX_SPIN_RATE, -MIN_SPIN_RATE);
        }

        listMeteors.Add(meteor);

        return spawn;
    }

    public void Pressed_Pause() {
        pausing = !pausing;

        if (!pausing) {
            foreach (var i in SpaceManager.Instance.ListSpace) {
                if (i.Planet.IEShoot == null) {
                    i.Planet.SpinAround();
                }
            }
        }
    }

    public void SpawnMeteorite() {
        for (int i = 0; i < METEORITE_NUMBER; i++) {
            Vector3 pos = GetRandomPosition(SUN_RADIUS, SPAWN_RANGE);
            int index = UnityEngine.Random.Range(0, stones.Count);

            GameObject spawn = CreateMeteorObject(stones[index], pos);

            var meteor = spawn.GetComponent<MeteorBeltItem>();

            int rand = UnityEngine.Random.Range(0, 3);
            if (rand == 0) {
                meteor.SetData(HP_METEORITE_SMALL, DAME_METEORITE_SMALL, new DataReward(REWARD_METEORITE_SMALL, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1));
                meteor.transform.localScale = new Vector3(2f, 2f);
                meteor.gravityRange = 50f;
            } else if (rand == 1) {
                meteor.SetData(HP_METEORITE_MEDIUM, DAME_METEORITE_MEDIUM, new DataReward(REWARD_METEORITE_MEDIUM, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1));
                meteor.transform.localScale = new Vector3(3f, 3f);
                meteor.gravityRange = 120f;
            } else if (rand == 2) {
                meteor.SetData(HP_METEORITE_BIG, DAME_METEORITE_BIG, new DataReward(REWARD_METEORITE_BIG, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1));
                meteor.transform.localScale = new Vector3(4f, 4f);
                meteor.gravityRange = 180f;
            }

            listMeteors.Add(meteor);
        }
    }

    public void SpawnSpecialMeteorite() {
        List<GameObject> colorMeteors = new List<GameObject> { air, antimater, fire, gravity, ice };

        for (int i = 0; i < colorMeteors.Count; i++) {
            for (int j = 0; j < MONO_COLOR_NUMBER; j++) {
                Vector3 pos = GetRandomPosition(SUN_RADIUS, SPAWN_RANGE);
                GameObject spawn = CreateMeteorObject(colorMeteors[i], pos);
            }
        }

        for (int i = 0; i < MULTI_COLOR_NUMBER; i++) {
            Vector3 pos = GetRandomPosition(SUN_RADIUS, SPAWN_RANGE);
            GameObject spawn = CreateMeteorObject(colorful, pos);
        }

        for (int i = 0; i < DIAMOND_NUMBER; i++) {
            Vector3 pos = GetRandomPosition(SUN_RADIUS, SPAWN_RANGE);
            GameObject spawn = CreateMeteorObject(diamond, pos);
        }
    }

    public void SwitchToMainGameplay() {
        PopupConfirm.ShowYesNo("Return", "Do you want to leave Meter Belt?", () => {
            ShowFinalResult();
        });
        TextMan.Get("Return", "Do you want to leave Meter Belt?");
    }

    public void ShowFinalResult() {
        if (reward == null) {
            Scenes.ChangeScene(SceneName.Gameplay);
            return;
        }

        if (reward.IsEmpy()) {
            Scenes.ChangeScene(SceneName.Gameplay);
            return;
        }

        PopupMeteorResult.Show("Congratulation", "Return", reward, okFunction: () => {
            if (PopupMeteorResult.meteorWatchAdsX2) {
                reward.Add(reward);
                PopupMeteorResult.Show("Congratulation", "Return", reward, okFunction: () => {
                    DataGameSave.dataLocal.M_Material += reward.material;
                    DataGameSave.dataLocal.M_AirStone += reward.air;
                    DataGameSave.dataLocal.M_AntimatterStone += reward.antimater;
                    DataGameSave.dataLocal.M_FireStone += reward.fire;
                    DataGameSave.dataLocal.M_GravityStone += reward.gravity;
                    DataGameSave.dataLocal.M_IceStone += reward.ice;
                    DataGameSave.dataLocal.Diamond += reward.diamond;
                    DataGameSave.dataLocal.M_ToyStone1 += reward.toy1;
                    DataGameSave.dataLocal.M_ToyStone2 += reward.toy2;
                    DataGameSave.dataLocal.M_ToyStone3 += reward.toy3;
                    DataGameSave.dataLocal.M_ToyStone4 += reward.toy4;
                    DataGameSave.dataLocal.M_ToyStone5 += reward.toy5;

                    DataGameSave.dataServer.MaterialCollect += reward.material;

                    DataGameSave.SaveToLocal();
                    DataGameSave.SaveToServer();

                    Scenes.ChangeScene(SceneName.Gameplay);
                }, false);
                return;
            }

            DataGameSave.dataLocal.M_Material += reward.material;
            DataGameSave.dataLocal.M_AirStone += reward.air;
            DataGameSave.dataLocal.M_AntimatterStone += reward.antimater;
            DataGameSave.dataLocal.M_FireStone += reward.fire;
            DataGameSave.dataLocal.M_GravityStone += reward.gravity;
            DataGameSave.dataLocal.M_IceStone += reward.ice;
            DataGameSave.dataLocal.Diamond += reward.diamond;
            DataGameSave.dataLocal.M_ToyStone1 += reward.toy1;
            DataGameSave.dataLocal.M_ToyStone2 += reward.toy2;
            DataGameSave.dataLocal.M_ToyStone3 += reward.toy3;
            DataGameSave.dataLocal.M_ToyStone4 += reward.toy4;
            DataGameSave.dataLocal.M_ToyStone5 += reward.toy5;

            DataGameSave.dataServer.MaterialCollect += reward.material;

            DataGameSave.SaveToLocal();
            DataGameSave.SaveToServer();

            Scenes.ChangeScene(SceneName.Gameplay);
        });

        try {
            Camera.main.GetComponent<DCamera>().enabled = false;
        } catch { }

        MeteorBelt.Instance.StopCameraControl();
    }

    public void AddReward(MeteorBeltItem meteor, string type = "normal") {
        AddSpecificReward("material", meteor.reward.material, meteor.transform.position, type);
        AddSpecificReward("air", meteor.reward.air, meteor.transform.position);
        AddSpecificReward("antimater", meteor.reward.antimater, meteor.transform.position);
        AddSpecificReward("fire", meteor.reward.fire, meteor.transform.position);
        AddSpecificReward("gravity", meteor.reward.gravity, meteor.transform.position);
        AddSpecificReward("ice", meteor.reward.ice, meteor.transform.position);
        AddSpecificReward("diamond", meteor.reward.diamond, meteor.transform.position);
        AddSpecificReward("unknown", meteor.reward.unknown, meteor.transform.position);
        AddSpecificReward("toy1", meteor.reward.toy1, meteor.transform.position);
        AddSpecificReward("toy2", meteor.reward.toy2, meteor.transform.position);
        AddSpecificReward("toy3", meteor.reward.toy3, meteor.transform.position);
        AddSpecificReward("toy4", meteor.reward.toy4, meteor.transform.position);
        AddSpecificReward("toy5", meteor.reward.toy5, meteor.transform.position);

        if (meteor.meteorType == MeteorBeltItem.METEOR_TYPE.METEORITE) {
            AddSpecificReward("material", UnityEngine.Random.Range(100, 201), meteor.transform.position);
        }

        if (meteor.meteorType == MeteorBeltItem.METEOR_TYPE.MONO_COLOR) {
            AddSpecificReward("material", UnityEngine.Random.Range(400, 500), meteor.transform.position);
            AddSpecificReward(meteor.effectType, 1, meteor.transform.position);
        }

        if (meteor.meteorType == MeteorBeltItem.METEOR_TYPE.MULTI_COLOR) {
            List<string> list = new List<string>() { "air", "antimater", "fire", "gravity", "ice" };
            var shuffled = list.OrderBy(x => System.Guid.NewGuid()).ToList();
            AddSpecificReward(shuffled[0], 1, meteor.transform.position);
            AddSpecificReward(shuffled[1], 1, meteor.transform.position);
            AddSpecificReward(shuffled[2], 1, meteor.transform.position);
            AddSpecificReward("material", UnityEngine.Random.Range(100, 201), meteor.transform.position);
        }

        if (meteor.meteorType == MeteorBeltItem.METEOR_TYPE.DIAMOND) {
            AddSpecificReward("material", UnityEngine.Random.Range(100, 201), meteor.transform.position);
            AddSpecificReward("diamond", 5, meteor.transform.position);
        }
    }

    public void AddSpecificReward(string rewardName, int amount, Vector3 meteorPosition, string meteorType = "normal") {
        if (rewardName == "material") {
            shootRewardCount.material += amount;
            int spawnAmount = 0;
            if (meteorType == "bigmeteor") {
                spawnAmount = 10;
            } else {
                spawnAmount = ((int)amount / 1000) + 1;
            }

            ExplodeMeteor(rewardMaterial, meteorPosition, spawnAmount, (int)amount / spawnAmount, meteorType);
        } else if (rewardName == "air") {
            shootRewardCount.air += amount;
            ExplodeMeteor(rewardAir, meteorPosition, amount, 1, meteorType);
        } else if (rewardName == "antimater") {
            shootRewardCount.antimater += amount;
            ExplodeMeteor(rewardAntimater, meteorPosition, amount, 1, meteorType);
        } else if (rewardName == "fire") {
            shootRewardCount.fire += amount;
            ExplodeMeteor(rewardFire, meteorPosition, amount, 1, meteorType);
        } else if (rewardName == "gravity") {
            shootRewardCount.gravity += amount;
            ExplodeMeteor(rewardGravity, meteorPosition, amount, 1, meteorType);
        } else if (rewardName == "ice") {
            shootRewardCount.ice += amount;
            ExplodeMeteor(rewardIce, meteorPosition, amount, 1, meteorType);
        } else if (rewardName == "diamond") {
            shootRewardCount.diamond += amount;

            if (amount < 10) {
                ExplodeMeteor(rewardDiamond, meteorPosition, amount, 1, meteorType);
            } else if (amount < 100) {
                int spawnNumber = (int)(amount / 10);
                ExplodeMeteor(rewardDiamond, meteorPosition, spawnNumber, (int)(amount / spawnNumber), meteorType);
            } else {
                int spawnNumber = (int)(amount / 100);
                ExplodeMeteor(rewardDiamond, meteorPosition, spawnNumber, (int)(amount / spawnNumber), meteorType);
            }

        } else if (rewardName == "toy1") {
            shootRewardCount.toy1 += amount;
            ExplodeMeteor(rewardToy1, meteorPosition, amount, 1, meteorType);
        } else if (rewardName == "toy2") {
            shootRewardCount.toy2 += amount;
            ExplodeMeteor(rewardToy2, meteorPosition, amount, 1, meteorType);
        } else if (rewardName == "toy3") {
            shootRewardCount.toy3 += amount;
            ExplodeMeteor(rewardToy3, meteorPosition, amount, 1, meteorType);
        } else if (rewardName == "toy4") {
            shootRewardCount.toy4 += amount;
            ExplodeMeteor(rewardToy4, meteorPosition, amount, 1, meteorType);
        } else if (rewardName == "toy5") {
            shootRewardCount.toy5 += amount;
            ExplodeMeteor(rewardToy5, meteorPosition, amount, 1, meteorType);
        } else if (rewardName == "unknown")
            shootRewardCount.unknown += amount;
    }

    public void ShootMeteorBeltEnd(GameObject shootingPlanet) {
        reward.Add(shootRewardCount);
        shootRewardCount = new DataReward();

        if (shootingPlanet == this.shootingPlanet) {
            camera.transform.SetParent(null);
            camera.transform.localScale = new Vector3(1, 1, 1);
            camera.target = sun;
            camera.speed = 50f;
            camera.returnToSunButton.SetActive(false);
        }
    }

    public void ShootMeteorBeltStart(GameObject shootPlanet) {
        this.shootingPlanet = shootPlanet;
        camera.gameObject.transform.SetParent(shootPlanet.transform);
        camera.transform.localPosition = new Vector3(0, 0, camera.transform.localPosition.z);
        camera.transform.localScale = new Vector3(1, 1, 1);
        camera.returnToSunButton.SetActive(true);
    }

    public void StopCameraControl() {
        cameraManage.enabled = false;
    }

    public IEnumerator DestroyGameObject(GameObject gameObject, float delayTime = -1f) {
        if (delayTime == -1)
            delayTime = UnityEngine.Random.Range(3f, 6f);
        yield return new WaitForSeconds(delayTime);
        Destroy(gameObject);
    }

    public void ExplodeMeteor(GameObject explodeObject, Vector3 explodePosition, int pieceCount = 3, int value = 10, string meteorType = "normal") {
        for (int i = 0; i < pieceCount; i++) {
            GameObject effect = Instantiate(explodeObject, explodePosition, Quaternion.identity);
            Vector2 rand = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));

            if (meteorType == "bigmeteor") {
                rand = rand.normalized * UnityEngine.Random.Range(BIG_METEOR_MIN_EXPLODE_FORCE, BIG_METEOR_MAX_EXPLODE_FORCE);
            } else {
                rand = rand.normalized * EXPLODE_METEOR_FORCE;
            }

            effect.GetComponent<Rigidbody2D>().AddForce(rand);
            if (value == 1)
                effect.GetComponentInChildren<TMPro.TextMeshPro>().text = "";
            else
                effect.GetComponentInChildren<TMPro.TextMeshPro>().text = value.ToString();
            StartCoroutine(DestroyGameObject(effect, UnityEngine.Random.Range(2f, 3f)));
        }
    }

    //can sua lai theo data battle
    public void MeteorFallBuff_RestoreAllPlanet() {
        for (int i = 0; i < DataGameSave.dataServer.ListPlanet.Count; i++) {
            DataGameSave.dataBattle.ListPlanet[i].ShootTime = DateTime.MinValue;
            DataGameSave.dataBattle.ListPlanet[i].Type = TypePlanet.Default;
            DataGameSave.dataBattle.ListPlanet[i].HP = maxHps[i];
        }

        foreach (var spaceController in SpaceManager.Instance.ListSpace) {
            Destroy(spaceController.Planet.gameObject);
        }

        StartCoroutine(SpaceManager.Instance.InitListSpace(DataGameSave.dataBattle));

        LeanTween.delayedCall(5f, () => {
            Camera.main.orthographicSize = Camera.main.orthographicSize * METEOR_FALL_CAMERA_SIZE_MULTIPLE;
        });
    }

    public void MeteorFallBuff_IncreaseHp() {
        for (int i = 0; i < maxHps.Count; i++) {
            maxHps[i] *= 10f;
        }

        for (int i = 0; i < DataGameSave.dataBattle.ListPlanet.Count; i++) {
            float newHp = DataGameSave.dataBattle.ListPlanet[i].HP + maxHps[i] * 0.2f;

            DataGameSave.dataBattle.ListPlanet[i].HP = newHp > maxHps[i] ? maxHps[i] : newHp;
        }
    }

    public void MeteorFallBuff_TransformAllPlanet(TypePlanet type) {
        for (int i = 0; i < DataGameSave.dataServer.ListPlanet.Count; i++) {
            if (DataGameSave.dataBattle.ListPlanet[i].type != TypePlanet.Destroy) {
                DataGameSave.dataBattle.ListPlanet[i].Type = type;
                SpaceManager.Instance.ListSpace[i].Planet.SetData(DataGameSave.dataBattle.ListPlanet[i], i);
            }
        }
    }

    public void MeteorFallBuff_NoDameTake() {
        StartCoroutine(MeteorFallBuff_NoDameTake_Coroutine(5f));
        btnTakeNoDame.SetActive(false);
    }

    public IEnumerator MeteorFallBuff_NoDameTake_Coroutine(float sec) {
        for (int i = 0; i < DataGameSave.dataServer.ListPlanet.Count; i++) {
            if (DataGameSave.dataBattle.ListPlanet[i].type != TypePlanet.Destroy) {
                SpriteAnim anim = SpaceManager.Instance.ListSpace[i].Planet.spriteAnim;
                anim.spriteRenderer.color = Color.yellow;
                //GetComponent<Image>().color = Color.yellow;
            }
        }

        GameManager.takeNoDame = true;

        yield return new WaitForSeconds(sec);

        for (int i = 0; i < DataGameSave.dataServer.ListPlanet.Count; i++) {
            if (DataGameSave.dataBattle.ListPlanet[i].type != TypePlanet.Destroy) {
                //SpaceManager.Instance.ListSpace[i].GetComponent<Image>().color = Color.white;
                SpriteAnim anim = SpaceManager.Instance.ListSpace[i].Planet.spriteAnim;
                anim.spriteRenderer.color = Color.white;
            }
        }

        GameManager.takeNoDame = false;
    }

    public void SetupMeteorFallDatas() {
        meteorFallDatas = new List<BattlePassCellData>();
        meteorFallDatas.Add(new BattlePassCellData(1, 50, 2, 2, 0, BattlePassRewardType.DIAMOND, 100));
        meteorFallDatas.Add(new BattlePassCellData(2, 55, 2, 2, 0, BattlePassRewardType.BLACK_HOLE_STONES_RANDOM, 10));
        meteorFallDatas.Add(new BattlePassCellData(3, 60, 2, 2, 0, BattlePassRewardType.EFFECT_STONES_RANDOM, 5));
        meteorFallDatas.Add(new BattlePassCellData(4, 65, 2, 2, 0, BattlePassRewardType.PLANET_RANDOM, 10));
        meteorFallDatas.Add(new BattlePassCellData(5, 70, 4, 2, 1, BattlePassRewardType.CHEST, 1));
        meteorFallDatas.Add(new BattlePassCellData(6, 75, 2, 2, 0, BattlePassRewardType.DIAMOND, 150));
        meteorFallDatas.Add(new BattlePassCellData(7, 80, 2, 2, 0, BattlePassRewardType.BLACK_HOLE_STONES_RANDOM, 15));
        meteorFallDatas.Add(new BattlePassCellData(8, 85, 2, 2, 0, BattlePassRewardType.EFFECT_STONES_RANDOM, 6));
        meteorFallDatas.Add(new BattlePassCellData(9, 90, 2, 2, 0, BattlePassRewardType.PLANET_RANDOM, 15));
        meteorFallDatas.Add(new BattlePassCellData(10, 95, 4, 2, 2, BattlePassRewardType.CHEST, 2));

        meteorFallDatas.Add(new BattlePassCellData(11, 10, 40, 10, 0, BattlePassRewardType.DIAMOND, 200));
        meteorFallDatas.Add(new BattlePassCellData(12, 105, 4, 2, 0, BattlePassRewardType.BLACK_HOLE_STONES_RANDOM, 20));
        meteorFallDatas.Add(new BattlePassCellData(13, 110, 4, 2, 0, BattlePassRewardType.EFFECT_STONES_RANDOM, 7));
        meteorFallDatas.Add(new BattlePassCellData(14, 115, 4, 2, 0, BattlePassRewardType.PLANET_RANDOM, 20));
        meteorFallDatas.Add(new BattlePassCellData(15, 120, 4, 2, 3, BattlePassRewardType.CHEST, 3));
        meteorFallDatas.Add(new BattlePassCellData(16, 125, 4, 100, 0, BattlePassRewardType.DIAMOND, 250));
        meteorFallDatas.Add(new BattlePassCellData(17, 130, 4, 2, 0, BattlePassRewardType.BLACK_HOLE_STONES_RANDOM, 25));
        meteorFallDatas.Add(new BattlePassCellData(18, 135, 4, 2, 0, BattlePassRewardType.EFFECT_STONES_RANDOM, 8));
        meteorFallDatas.Add(new BattlePassCellData(19, 140, 4, 2, 0, BattlePassRewardType.PLANET_RANDOM, 25));
        meteorFallDatas.Add(new BattlePassCellData(20, 145, 4, 2, 4, BattlePassRewardType.CHEST, 4));

        meteorFallDatas.Add(new BattlePassCellData(21, 145, 4, 2, 0, BattlePassRewardType.DIAMOND, 300));
        meteorFallDatas.Add(new BattlePassCellData(22, 150, 4, 2, 0, BattlePassRewardType.BLACK_HOLE_STONES_RANDOM, 30));
        meteorFallDatas.Add(new BattlePassCellData(23, 0, 100, 2, 0, BattlePassRewardType.EFFECT_STONES_RANDOM, 9));
        meteorFallDatas.Add(new BattlePassCellData(24, 160, 10, 2, 0, BattlePassRewardType.PLANET_RANDOM, 30));
        meteorFallDatas.Add(new BattlePassCellData(25, 165, 10, 2, 5, BattlePassRewardType.CHEST, 5));
        meteorFallDatas.Add(new BattlePassCellData(26, 170, 10, 2, 0, BattlePassRewardType.DIAMOND, 350));
        meteorFallDatas.Add(new BattlePassCellData(27, 175, 10, 2, 0, BattlePassRewardType.BLACK_HOLE_STONES_RANDOM, 35));
        meteorFallDatas.Add(new BattlePassCellData(28, 180, 10, 2, 0, BattlePassRewardType.EFFECT_STONES_RANDOM, 10));
        meteorFallDatas.Add(new BattlePassCellData(29, 185, 10, 2, 0, BattlePassRewardType.PLANET_RANDOM, 35));
        meteorFallDatas.Add(new BattlePassCellData(30, 190, 10, 2, 0, BattlePassRewardType.CHEST, 6));

        meteorFallDatas.Add(new BattlePassCellData(31, 195, 10, 2, 6, BattlePassRewardType.DIAMOND, 400));
        meteorFallDatas.Add(new BattlePassCellData(32, 0, 50, 2, 0, BattlePassRewardType.BLACK_HOLE_STONES_RANDOM, 40));
        meteorFallDatas.Add(new BattlePassCellData(33, 205, 10, 2, 0, BattlePassRewardType.EFFECT_STONES_RANDOM, 11));
        meteorFallDatas.Add(new BattlePassCellData(34, 210, 10, 2, 0, BattlePassRewardType.PLANET_RANDOM, 40));
        meteorFallDatas.Add(new BattlePassCellData(35, 215, 10, 2, 0, BattlePassRewardType.CHEST, 7));
        meteorFallDatas.Add(new BattlePassCellData(36, 220, 10, 2, 7, BattlePassRewardType.DIAMOND, 450));
        meteorFallDatas.Add(new BattlePassCellData(37, 225, 10, 2, 0, BattlePassRewardType.BLACK_HOLE_STONES_RANDOM, 45));
        meteorFallDatas.Add(new BattlePassCellData(38, 230, 10, 2, 0, BattlePassRewardType.EFFECT_STONES_RANDOM, 12));
        meteorFallDatas.Add(new BattlePassCellData(39, 235, 10, 2, 0, BattlePassRewardType.PLANET_RANDOM, 45));
        meteorFallDatas.Add(new BattlePassCellData(40, 240, 10, 2, 8, BattlePassRewardType.CHEST, 8));

        meteorFallDatas.Add(new BattlePassCellData(41, 245, 10, 2, 0, BattlePassRewardType.DIAMOND, 500));
        meteorFallDatas.Add(new BattlePassCellData(42, 250, 10, 2, 0, BattlePassRewardType.BLACK_HOLE_STONES_RANDOM, 50));
        meteorFallDatas.Add(new BattlePassCellData(43, 255, 10, 2, 0, BattlePassRewardType.EFFECT_STONES_RANDOM, 13));
        meteorFallDatas.Add(new BattlePassCellData(44, 260, 10, 2, 0, BattlePassRewardType.PLANET_RANDOM, 50));
        meteorFallDatas.Add(new BattlePassCellData(45, 0, 10, 120, 20, BattlePassRewardType.CHEST, 9));
        meteorFallDatas.Add(new BattlePassCellData(46, 270, 10, 2, 0, BattlePassRewardType.DIAMOND, 550));
        meteorFallDatas.Add(new BattlePassCellData(47, 275, 10, 2, 0, BattlePassRewardType.BLACK_HOLE_STONES_RANDOM, 55));
        meteorFallDatas.Add(new BattlePassCellData(48, 280, 10, 2, 0, BattlePassRewardType.EFFECT_STONES_RANDOM, 14));
        meteorFallDatas.Add(new BattlePassCellData(49, 285, 10, 2, 0, BattlePassRewardType.PLANET_RANDOM, 55));
        meteorFallDatas.Add(new BattlePassCellData(50, 0, 0, 0, 40, BattlePassRewardType.CHEST, 10));
    }
}
