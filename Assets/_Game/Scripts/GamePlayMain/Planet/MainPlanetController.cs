using SteveRogers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;
using System;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class MainPlanetController : Singleton<MainPlanetController>
{
    public GameObject PreCollectionMaterial;
    public SpriteRenderer HealthBar;
    public DataBaseSun Data;

    public SpriteRenderer Sun;
    public GameObject PrefabFxBigbang;

    public Animator AnimFace;
    public GameObject PrefabBomb;
    public GameObject Space;
    public CanvasGroup OjHighLight;

    public GameObject RedDot;

    public TextMeshPro TxtCollect;

    // steve
    public GameObject sunFx_Blue = null;
    public GameObject sunFx_Yellow = null;
    public GameObject sunFx_Orange = null;
    public GameObject sunFx_Red = null;
    public GameObject[] sunParticleLevels = null;
    public Transform sunParticleParent = null;
    public GameObject[] sunUpgradeVFXGos = null;
    // steve.

    bool _IsDie;
    Coroutine collectCorou;

    float ScaleSize;
    float TimeMaxCollect = 0;
    int MoneyCollect = 0;

    public int size;

    // Duchuy code
    public bool isGeneratingMaterial = true;

    public int Size
    {
        get { return size; }
        set
        {
            size = value;
            SetSize();
        }
    }

    public void PlaySunUpgradeEffect(int indexLevel)
    {
        if (!TutMan.IsDone(TutMan.TUT_KEY_08_BACK_TO_GAMEPLAY))
            return;

        var go = sunUpgradeVFXGos.GetLastIfOverRange(indexLevel);

        go.SetActive(true);
        go.GetComponent<ParticleSystem>()?.Play();
    }

    private void Update()
    {
        //Debug.Log(Scenes.LastScene);
        //if (Utilities.IsPressed_Space)
        //{

        //}
    }

    public bool IsShowPopup = false;

    public bool IsEnemy
    {
        get
        {
            return SpaceManager.Instance.MainPlanet != this;
        }
    }

    GameObject currentSunVFX = null;

    public void UpdateSpriteSun()
    {
        if (!sunParticleParent)
            return;

        if (Sun)
            Sun.enabled = false;

        var lv = (IsEnemy ? SpaceEnemyManager.Instance.Level : SpaceManager.Instance.Level) - 1;

        var go = sunParticleLevels.Get(lv);

        if (!go)
            go = sunParticleLevels[0];

        if (sunParticleParent.childCount > 0)
        {
            // bigger

            LeanTween.scale(sunParticleParent.GetChild(0).gameObject, sunParticleParent.GetChild(0).transform.localScale * 2, 0.5f)
                .setOnComplete(() =>
                {
                    // create new

                    currentSunVFX = Instantiate(go, sunParticleParent);
                    var oriscale = currentSunVFX.transform.localScale;
                    currentSunVFX.transform.localScale = sunParticleParent.GetChild(0).transform.localScale;

                    // destroy old

                    Destroy(sunParticleParent.GetChild(0).gameObject);

                    // smaller

                    LeanTween.scale(currentSunVFX, oriscale, 0.5f);
                });

        }
        else
            currentSunVFX = Instantiate(go, sunParticleParent);
    }

    private void Start()
    {
        UpdateSpriteSun();

        if (Scenes.Current == SceneName.Gameplay)
        {
            if (RedDot != null)
                RedDot.SetActive(false);

            if (DataGameSave.dataServer.Name != "")
            {
                Collect();
            }
            else
            {
                GameStatics.IsAnimating = true;
                Sun.color = Color.clear;

                LeanPool.Spawn(PrefabFxBigbang, Vector3.zero, Quaternion.identity, transform);
                StartCoroutine(MonoHelper.DoSomeThing(2f, () =>
                 {
                     Sun.color = Color.white;

                     StartCoroutine(MonoHelper.DoSomeThing(1f, () =>
                      {
                          GameStatics.IsAnimating = false;
                          if (File.Exists(FacebookManager.LocalPathAvatar()))
                              File.Delete(FacebookManager.LocalPathAvatar());
                      }));
                 }));
            }
        }
    }


    public void Collect()
    {
        if (collectCorou == null && Scenes.Current == SceneName.Gameplay)
            collectCorou = StartCoroutine(CollectData());
    }

    public void SetSize()
    {
        ScaleSize = 0.05f * size;
        transform.localScale = Vector3.one * ScaleSize;
    }


    public void SetData(DataBaseSun _Data)
    {
        Data = _Data;
        Size = Data.Size;

        if (Scenes.Current == SceneName.Battle && HealthBar != null) {
            DataGameSave.dataEnemy.sunHp = ReadDataSun.Instance.ListDataSun.GetLastIfOverRange(Data.Level - 1).Hp;
            HealthBar.transform.localScale = new Vector3(1, 1, 1);
        } else if (Scenes.Current == SceneName.BattlePassGameplay && HealthBar != null) {
            DataGameSave.dataEnemy.sunHp = ReadDataSun.Instance.ListDataSun.GetLastIfOverRange(Data.Level - 1).Hp;
            HealthBar.transform.localScale = new Vector3(1, 1, 1);
        }
    }


    public void SetMaterialAfterUpdate()
    {
        PlayfabManager.GetServerTime(
          0,
          (result, index) =>
          {
              float collectTime = (float)(result - DataGameSave.dataServer.CollectTime).TotalSeconds;
              float maxCollectTime = DataGameSave.dataServer.MaxTimeCollect;

              if (maxCollectTime > collectTime)
              {
                  DataGameSave.dataServer.MaxTimeCollect -= collectTime;
              }
              else
              {
                  DataGameSave.dataServer.MaxTimeCollect = 0;
              }

              DataGameSave.dataServer.CollectTime = result;
              DataGameSave.dataServer.MaterialCollect = MoneyCollect;
          });
    }


    IEnumerator CollectData()
    {
        MoneyCollect = 0;
        float MoneyTemp = 0;
        DateTime now = DateTime.Now;
        PlayfabManager.GetServerTime(
            0,
            (result, index) =>
            {
                now = result;

                if (DataGameSave.dataServer.IsCollect)
                {
                    MoneyTemp = 0;
                    DataGameSave.dataServer.CollectTime = now;
                    DataGameSave.dataServer.IsCollect = false;
                    DataGameSave.dataServer.MaxTimeCollect = TimeMaxCollect = 43200;
                    DataGameSave.SaveToServer();
                }
                else
                {

                    MoneyTemp = DataGameSave.dataServer.GetStartMoney(now);

                    TimeMaxCollect = DataGameSave.dataServer.MaxTimeCollect;
                }

                MoneyCollect += (int)MoneyTemp;

                if (TxtCollect != null)
                {
                    if (MoneyCollect == 0)
                    {
                        TxtCollect.text = "";
                    }
                    else
                    {
                        TxtCollect.text = ((int)MoneyCollect * DataGameSave.dataLocal.itemX2.multiplyNumber).ToString() + TextConstants.M_Mater;
                    }
                }

            });
        yield return new WaitForSecondsRealtime(0.5f);


        while (true)
        {
            yield return new WaitForSecondsRealtime(5);
            TimeMaxCollect -= 5;
            if (TimeMaxCollect <= 0)
            {
                collectCorou = null;
                break;
            }

            MoneyCollect += (int)Data.MaterialPer5Sec *
               DataGameSave.dataLocal.itemX2.multiplyNumber;

            if (TxtCollect != null)
            {
                if (MoneyCollect == 0)
                {
                    TxtCollect.text = "";
                }
                else
                {
                    TxtCollect.text = ((int)MoneyCollect).ToString() + TextConstants.M_Mater;
                }
            }

            if (PopupUpgradeSun.Isshow)
            {
                PopupUpgradeSun.UpdateMoney(MoneyCollect);
            }
        }
    }

    private void OnMouseUp()
    {
        if (SpaceManager.Instance.FxBack)
            SpaceManager.Instance.FxBack.SetActive(false);
    }

    public void GetDamage(float dame, PlanetController.Owner owner) {
        if (_IsDie)
        {
            return;
        }

        if (Scenes.Current == SceneName.MeteorBelt && owner == PlanetController.Owner.Player)
        {
            return;
        }

        //apply value and check die
        bool isDied = false;
        bool playerWin = true;
        float healthBarValue = 1f;

        float sunMaxHp = ReadDataSun.Instance.ListDataSun.GetLastIfOverRange(Data.Level - 1).Hp;

        if (owner == PlanetController.Owner.Player)
        {
            DataGameSave.dataEnemy.sunHp -= (int)dame;

            if (DataGameSave.dataEnemy.sunHp <= 0)
            {
                isDied = true;
            }

            healthBarValue = DataGameSave.dataEnemy.sunHp / sunMaxHp;
        }
        else
        {
            GameManager.sunHp -= dame;

            if (GameManager.sunHp <= 0)
            {
                isDied = true;
                playerWin = false;
            }

            healthBarValue = GameManager.sunHp / sunMaxHp;
        }

        //update healthbar
        if (Scenes.IsBattleScene() && HealthBar != null)
        {
            if (healthBarValue < 0)
                healthBarValue = 0;

            HealthBar.transform.localScale = new Vector3(healthBarValue, 1, 1);
        }

        //die logic
        if (isDied)
        {
            _IsDie = true;

            var destroyvfx = DataScriptableObject.Instance.sunExplosionGo;
            LeanPool.Spawn(destroyvfx, transform.position, destroyvfx.transform.rotation);

            if (GameManager.isFromDailyBattle)
            {
                GameManager.DailyBattleAttackCount--;
                GameManager.isFromDailyBattle = false;
                GameManager.isClaimableDailyBattle = true;
            }

            if (Scenes.Current == SceneName.MeteorBelt && MeteorBelt.isMeteorFall)
            {
                //MeteorBelt.Instance.ShowFinalResult();
                //int currentSceneId = Scenes.CurrentSceneId;

                //PopupConfirm.ShowOK("Battle Result!", "YOU LOSE!", "OK", () => {
                //    MeteorBelt.Instance.ShowFinalResult();
                //    //Scenes.ReturnToLastScene(currentSceneId);
                //});

                //LeanTween.delayedCall(5f, () => {
                //    MeteorBelt.Instance.ShowFinalResult();
                //    //Scenes.ReturnToLastScene(currentSceneId);
                //});
                MeteorBelt.Instance.ShowFinalResult();

                return;
            }

            DataGameSave.dataEnemy.sunHp = ReadDataSun.Instance.ListDataSun.GetLastIfOverRange(Data.Level - 1).Hp;

            List<SpaceController> listSpace = null;

            if (owner == PlanetController.Owner.Player)
            {
                listSpace = SpaceEnemyManager.Instance.ListSpace;
            }
            else
            {
                listSpace = SpaceManager.Instance.ListSpace;
            }

            for (int i = 0; i < listSpace.Count; i++)
            {
                DataPlanet dataPlanet = listSpace[i].Planet.DataPlanet;

                if (listSpace[i].Planet != null && dataPlanet.type != TypePlanet.Destroy && PrefabBomb != null)
                {
                    LeanTween.cancel(listSpace[i].Planet.gameObject);
                    LeanPool.Spawn(PrefabBomb, listSpace[i].Planet.transform.position, Quaternion.identity).transform.localScale = Vector3.one * 1;
                    listSpace[i].Planet.gameObject.SetActive(false);

                    if (playerWin)
                    {
                        DataGameSave.dataLocal.DestroyPlanet++;
                        DataGameSave.dataRank.destroy_planet++;
                    }
                }

                listSpace[i].Planet.planetCollider.enabled = false;
                listSpace[i].Planet.gameObject.GetComponent<CircleCollider2D>().enabled = false;
                listSpace[i].Planet.CaculateReward();
            }

            // if player lose, then show popup and exit 
            if (!playerWin)
            {
                GameOver();
            }
            else
            {
                WinBattleAndRewardPlayer();
            }
        }
    }

    public void GetDamage(PlanetController planetEnemy)
    {
        //calculate dame
        float dame = 0;
        int elementLevel = DataGameSave.dataServer.ListPlanet[planetEnemy.Manager.IndexSpace].LevelElement;

        if (planetEnemy.Type == TypePlanet.Fire)
        {
            dame = planetEnemy.Dame * DatabasePlanet.Instance.ListFirePlanet[elementLevel].UpDame; //-1
        }
        else if (planetEnemy.Type == TypePlanet.Antimatter)
        {
            dame = DatabasePlanet.Instance.ListAntimatterialPlanet[elementLevel].UpDame; //-1
        }
        else
        {
            dame = planetEnemy.Dame;
        }

        // reduce dame take 30% in battle pass
        if (Scenes.Current == SceneName.BattlePassGameplay) {
            dame *= (1 - GameManager.BATTLE_PASS_DAME_REDUCE);
        }

        GetDamage(dame, planetEnemy.owner);
    }

    public void GameOver() {
        //DataGameSave.dataEnemy.SetAttackedInfoAndSaveEnemyDataToServer(DataGameSave.dataServer);
        DataGameSave.SaveAttackedInfo(DataGameSave.dataServer, DataGameSave.dataEnemy);

        int currentSceneId = Scenes.CurrentSceneId;

        PopupConfirm.ShowOK("Battle Result!", "YOU LOSE!", "OK", () => {
            Scenes.ReturnToLastScene(currentSceneId);
        });

        LeanTween.delayedCall(5f, () => {
            Scenes.ReturnToLastScene(currentSceneId);
        });

        if (Scenes.Current == SceneName.BattlePassGameplay)
        {
            int winCount = 0;
            int.TryParse(DataGameSave.GetMetaData(MetaDataKey.BATTLE_PASS_WIN_COUNT), out winCount);
            winCount--;
            if (winCount <= 0)
            {
                winCount = 0;
            }

            DataGameSave.SaveMetaData(MetaDataKey.BATTLE_PASS_WIN_COUNT, winCount.ToString());
        }

        if (GameManager.isFromRank) {
            WWWForm form = new WWWForm();

            form.AddField("userid", DataGameSave.dataServer.userid);
            form.AddField("enemyid", DataGameSave.dataEnemy.userid);
            form.AddField("daycode", DataGameSave.GetDayCode(GameManager.Now));

            ServerSystem.Instance.SendRequest(ServerConstants.LOSE_RANK, form, ()=> {
                int.TryParse(ServerSystem.result, out int newPoint);
                GameManager.rankPointChange = newPoint - GameManager.rankPoint;
                GameManager.rankPoint = newPoint;

                DataGameSave.eventDatas = null;
                DataGameSave.GetRankChartData();
            });

            GameManager.isFromRank = false;
        }

        return;
    }

    public void WinBattleAndRewardPlayer() {
        DataGameSave.SaveAttackedInfo(DataGameSave.dataServer, DataGameSave.dataEnemy);

        // player win, reward player
        DataReward dataReward = new DataReward();

        if (Scenes.Current == SceneName.BattlePassGameplay && !GameManager.isFromRank) {
            if (Scenes.LastScene == SceneName.BattlePass) {
                //battle pass win
                int materialReward = (int)(DataGameSave.dataEnemy.GetAllMaterialCollect() * GameManager.STEAL_MATERIAL_RATE_OFFLINE_COLLECT);
                materialReward += (int)(DataGameSave.dataEnemy.MaterialCollect * GameManager.STEAL_MATERIAL_RATE_TOTAL);

                //dataReward.material = (int)DataGameSave.dataEnemy.GetAllMaterialCollect();
                dataReward.material = materialReward;
                GameManager.reward = dataReward;
                DataGameSave.dataEnemy.ResetMaterialCollect();

                if (Scenes.Current == SceneName.BattlePassGameplay) {
                    int winCount = 0;
                    int.TryParse(DataGameSave.GetMetaData(MetaDataKey.BATTLE_PASS_WIN_COUNT), out winCount);
                    winCount++;
                    winCount = winCount > GameManager.BATTLE_PASS_PRIZE_AMOUNT ? GameManager.BATTLE_PASS_PRIZE_AMOUNT : winCount;
                    DataGameSave.SaveMetaData(MetaDataKey.BATTLE_PASS_WIN_COUNT, winCount.ToString());
                }

                DataGameSave.SaveMetaData(MetaDataKey.BATTLE_PASS_LAST_WON_ID, DataGameSave.battlePassEnemy.userid.ToString());
                DataGameSave.GetBattlePassEnemy();
            } else {
                //vfact event
                dataReward.diamond = UnityEngine.Random.Range(50, 201);
                GameManager.reward = dataReward;
                DataGameSave.dataServer.rankPoint += 1;
            }
        } else {
            GameManager.reward.material = (int)DataGameSave.dataEnemy.GetAllMaterialCollect();
            DataGameSave.dataEnemy.ResetMaterialCollect();
            dataReward.material = GameManager.reward.material;
        }

        if (GameStatics.ShootTimes == 1) {
            DataGameSave.dataLocal.DestroySolarByOneHit++;
        }

        if (GameManager.isFromRank) {
            WWWForm form = new WWWForm();

            form.AddField("userid", DataGameSave.dataServer.userid);
            form.AddField("enemyid", DataGameSave.dataEnemy.userid);
            form.AddField("daycode", DataGameSave.GetDayCode(GameManager.Now));

            ServerSystem.Instance.SendRequest(ServerConstants.WIN_RANK, form, () => {
                int.TryParse(ServerSystem.result, out int newPoint);
                GameManager.rankPointChange = newPoint - GameManager.rankPoint;
                GameManager.rankPoint = newPoint;

                DataGameSave.eventDatas = null;
                DataGameSave.GetRankChartData();
            });

            GameManager.isFromRank = false;
        }

        StartCoroutine(DestroyTime(() =>
        {
            int currentSceneID = Scenes.CurrentSceneId;

            PopupBattleResult2.Show(
                TextMan.Get("Reward"),
                TextMan.Get("You have received") + "\n" + GameManager.reward.material,
                dataReward, () => {
                    RewardPlayer(currentSceneID);
                });

            return;
        }));
    }

    public void RewardPlayer(int currentSceneID) {

        if (PopupBattleResult2.watchAdsX2)
        {
            GameManager.reward.Add(GameManager.reward);

            PopupBattleResult2.Show("Congratulation", "Return", GameManager.reward, okFunction: () =>
            {
                DataGameSave.dataLocal.destroyedSolars++;
                DataGameSave.dataRank.destroyed_solars++;
                DataGameSave.dataLocal.M_Material += GameManager.reward.material;
                DataGameSave.dataLocal.M_Ice += GameManager.reward.ice;
                DataGameSave.dataLocal.M_Fire += GameManager.reward.fire;
                DataGameSave.dataLocal.M_Air += GameManager.reward.air;
                DataGameSave.dataLocal.M_Gravity += GameManager.reward.gravity;
                DataGameSave.dataLocal.M_Antimatter += GameManager.reward.antimater;
                DataGameSave.dataLocal.Diamond += GameManager.reward.diamond;
                DataGameSave.SaveToLocal();
                DataGameSave.SaveToServer();
                GameManager.reward = new DataReward();

                Scenes.ReturnToLastScene(currentSceneID);
            }, false);
            return;
        }

        DataGameSave.dataLocal.destroyedSolars++;
        DataGameSave.dataRank.destroyed_solars++;
        DataGameSave.dataLocal.M_Material += GameManager.reward.material;
        DataGameSave.dataLocal.M_Ice += GameManager.reward.ice;
        DataGameSave.dataLocal.M_Fire += GameManager.reward.fire;
        DataGameSave.dataLocal.M_Air += GameManager.reward.air;
        DataGameSave.dataLocal.M_Gravity += GameManager.reward.gravity;
        DataGameSave.dataLocal.M_Antimatter += GameManager.reward.antimater;
        DataGameSave.dataLocal.Diamond += GameManager.reward.diamond;
        DataGameSave.SaveToLocal();
        DataGameSave.SaveToServer();
        GameManager.reward = new DataReward();

        Scenes.ReturnToLastScene(currentSceneID);
    }

    IEnumerator DestroyTime(Action _Action)
    {
        HealthBar.gameObject.SetActive(false);
        for (int i = 0; i < 3; i++)
        {
            LeanPool.Spawn(PrefabBomb, transform.position, Quaternion.identity).transform.localScale = Vector3.one * ((2 * i) + 1);
            yield return new WaitForSeconds(.5f);
        }

        OjHighLight.gameObject.SetActive(true);

        LeanTween.alphaCanvas(OjHighLight, 1, .5f).setOnComplete(() =>
        {
            LeanTween.delayedCall(.5f, () =>
             {
                 OjHighLight.gameObject.SetActive(false);
                 Space.transform.localScale = Vector3.zero;
                 _Action.Invoke();
             });
        });
    }

    public void ResetCollect()
    {
        MoneyCollect = 0;
        TxtCollect.text = "";
        DateTime now = DateTime.Now;
        DataGameSave.dataServer.MaterialCollect = 0;
        DataGameSave.dataServer.IsCollect = true;
        if (collectCorou != null)
            StopCoroutine(collectCorou);
        collectCorou = null;

        PlayfabManager.GetServerTime(
           0,
           (result, index) =>
           {
               now = result;

               DataGameSave.dataServer.MaxTimeCollect = TimeMaxCollect = 43200;

               DataGameSave.dataServer.CollectTime = now;

               if (collectCorou == null && Scenes.Current == SceneName.Gameplay)
                   collectCorou = StartCoroutine(CollectData());

               DataGameSave.SaveToServer();

           });
    }

    public void CheckCanUpgrade()
    {

        if (Data.Level < 50 &&
        Data.MaterialNeed <= DataGameSave.dataLocal.M_Material)
        {

            if (RedDot)
                RedDot.SetActive(true);
        }
        else
        {
            if (RedDot)
                RedDot.SetActive(false);
        }
        if (!DataHelper.GetBool(TextConstants.IsTutorial, false))
            if (RedDot)
                RedDot.SetActive(false);
    }
}
