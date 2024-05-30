using UnityEngine;
using UnityEngine.UI;
using System;
using Hellmade.Sound;
using SteveRogers;
using System.Collections.Generic;

public class PopupUpgradePlanet : Popups
{
    public static PopupUpgradePlanet _Instance;
    public static bool IsShow = false;

    public GameObject[] particlePlanetEffects = null;

    public GameObject group_Manage, group_Upgrade;
    public GameObject BtnAutoUp;
    public GameObject BtnFire;
    public GameObject redDotUpgrade = null;
    public GameObject redDotAvatar = null;
    public GameObject detailPanel = null;
    public GameObject upgradePanel = null;
    public GameObject effectPanel = null;

    public PopupSkinPlanet skinPlanetManager;
    public SpriteAnim spriteAnim = null;
    public SpriteAnim planetEffectTabSpriteAnim = null;
    public AudioSource audioSource;

    public Text TxtPriceBigger;
    public Text TxtPriceFaster;
    public Text TxtPriceHeavier;

    public Button BtnSpeed;
    public Button BtnSize;
    public Button BtnHeavier;

    public Image btnImage_Detail = null;
    public Image btnImage_Upgrade = null;
    public Image btnImage_Effect = null;

    public Text warningText = null;
    public Text
        detailTxt_Gravity = null,
        detailTxt_Size = null,
        detailTxt_HP = null,
        detailTxt_Speed = null,
        detailTxt_Material = null,

        detailTxt_Cost_Ice = null,
        detailTxt_Cost_Fire = null,
        detailTxt_Cost_Air = null,
        detailTxt_Cost_AntiMatter = null,
        detailTxt_Cost_Gravity = null,

        detailTxt_Name = null,
        effectTxt_Info = null,

        detailTxt_CurrentCollectMaterial = null,

        nameTxt = null,
        levelTxt = null;

    public Sprite buttonSprite_Active = null;
    public Sprite buttonSprite_Inactive = null;

    public PlanetController Planet;
    public PopupAvatar popupAvatar = null;

    public Transform
        sizeBar = null,
        speedBar = null,
        heavyBar = null;

    public PlanetScroller planetScroller = null;
    public GameObject newPlanetOnPopupUpgradeVFXPrefab = null;
    public Animation animTextLevelUP;

    private TypePlanet showingParticlePlanetEffect = TypePlanet.Default;
    private GameObject effectLevelUp;
    private Action _onClose;

    private int M_MatNeedSpeed;
    private int M_MatNeedSize;
    private int M_MatNeedHeavier;
    private int MoneyCollect = 0;
    private bool isManage = false;

    private void Start()
    {
        _Instance = this;
        effectLevelUp = Instantiate(newPlanetOnPopupUpgradeVFXPrefab, effectPanel.transform);
        audioSource = GetComponent<AudioSource>();
    }

    private void SetBar(Transform transform, int idx)
    {
        for (int i = 0; i < 10; i++)
        {
            transform.GetChild(i).GetChild(0).gameObject.SetActive(i < idx);
        }
    }

    public void SetData()
    {
        OnDetail();
        ShowWarningText();
        spriteAnim.gameObject.SetActive(false);
        IsShow = true;
        Planet = SpaceManager.Instance.PlanetSelect;

        StartCoroutine(MonoHelper.DoSomeThing(.02f, () =>
         {
             spriteAnim.gameObject.SetActive(true);
             spriteAnim.Set(Planet.DataPlanet.Name.ToString());
         }));

        SetDataSize();
        SetDataSpeed();
        SetDataHeavier();

        nameTxt.text = Planet.DataPlanet.Name.ToString();
        levelTxt.text = "Lv. " + ((int)Planet.DataPlanet.Name + 1);
        Check();
        planetScroller.RefreshActives();
        UpdateMoney(Planet.MoneyCollect);
        redDotAvatar.SetActive(Utilities.GetPlayerPrefsBool("redDotAvatar", false));
        SetCostEffects();
    }

    private void SetDataSize()
    {
        int LevelSize = Planet.LevelSize;

        SetBar(sizeBar, LevelSize);

        if (LevelSize < 10)
        {
            M_MatNeedSize = DataMaster.PlanetUpgradeMaterialNeeded_Get(Planet.DataPlanet.Name, LevelSize);
            TxtPriceBigger.text = Utilities.MoneyShorter(M_MatNeedSize, 1).ToString();
        }
        else
        {
            TxtPriceBigger.text = TextMan.Get("Max");
        }

        UpdateDetailPanel();
    }

    private void SetDataSpeed()
    {
        int LevelSpeed = Planet.LevelSpeed;

        SetBar(speedBar, LevelSpeed);

        if (LevelSpeed < 10)
        {
            M_MatNeedSpeed = DataMaster.PlanetUpgradeMaterialNeeded_Get(Planet.DataPlanet.Name, LevelSpeed);
            TxtPriceFaster.text = Utilities.MoneyShorter(M_MatNeedSpeed, 1).ToString();
        }
        else
        {
            TxtPriceFaster.text = TextMan.Get("Max");
        }

        UpdateDetailPanel();
    }

    public void OnPressed_AutoUpgrade()
    {
        ShowWarningText();

        while (true)
        {
            if (Planet.DataPlanet.Name >= PLanetName._Count)
                return;

            if (Planet.LevelSize < 10)
            {
                M_MatNeedSize = DataMaster.PlanetUpgradeMaterialNeeded_Get(Planet.DataPlanet.Name, Planet.LevelSize);

                if (DataGameSave.dataLocal.M_Material < M_MatNeedSize) // not enough
                {
                    ShowWarningText(TextMan.Get("not_enough_materials"));
                    return;
                }
                else // enough
                {
                    OnUpBigger();
                }
            }

            if (Planet.LevelSpeed < 10)
            {
                M_MatNeedSpeed = DataMaster.PlanetUpgradeMaterialNeeded_Get(Planet.DataPlanet.Name, Planet.LevelSpeed);

                if (DataGameSave.dataLocal.M_Material < M_MatNeedSpeed) // not enough
                {
                    ShowWarningText(TextMan.Get("not_enough_materials"));
                    return;
                }
                else // enough
                {
                    OnUpFaster();
                }
            }

            if (Planet.LevelHeavier < 10)
            {
                M_MatNeedHeavier = DataMaster.PlanetUpgradeMaterialNeeded_Get(Planet.DataPlanet.Name, Planet.LevelHeavier);

                if (DataGameSave.dataLocal.M_Material < M_MatNeedHeavier) // not enough
                {
                    ShowWarningText(TextMan.Get("not_enough_materials"));
                    return;
                }
                else // enough
                {
                    OnUpHeavier();
                }
            }

            if (Planet.LevelSize >= 10 && Planet.LevelSpeed >= 10 && Planet.LevelHeavier >= 10)
            {
                if (OnNewPlanet())
                {
                    Utilities.SetPlayerPrefsBool("just_level_up", true);
                    break;
                }
                else
                    return;
            }
        }

        GameManager.needToSaveData = true;
    }

    private void SetDataHeavier()
    {
        int LevelHeavier = Planet.LevelHeavier;

        SetBar(heavyBar, LevelHeavier);

        if (LevelHeavier < 10)
        {
            M_MatNeedHeavier = DataMaster.PlanetUpgradeMaterialNeeded_Get(Planet.DataPlanet.Name, LevelHeavier);
            TxtPriceHeavier.text = Utilities.MoneyShorter(M_MatNeedHeavier, 1).ToString();
        }
        else
        {
            TxtPriceHeavier.text = TextMan.Get("Max");
        }

        UpdateDetailPanel();
    }

    private void SetCostEffects()
    {
        detailTxt_Cost_Air.text = Utilities.MoneyShorter(DataGameSave.dataLocal.M_Air);
        detailTxt_Cost_Fire.text = Utilities.MoneyShorter(DataGameSave.dataLocal.M_Fire);
        detailTxt_Cost_Ice.text = Utilities.MoneyShorter(DataGameSave.dataLocal.M_Ice);
        detailTxt_Cost_Gravity.text = Utilities.MoneyShorter(DataGameSave.dataLocal.M_Gravity);
        detailTxt_Cost_AntiMatter.text = Utilities.MoneyShorter(DataGameSave.dataLocal.M_Antimatter);
    }

    private void SetParticlePlanetEffect()
    {
        detailTxt_Name.text = showingParticlePlanetEffect.ToString().ToUpper();
        SetCostEffects();

        if (showingParticlePlanetEffect == TypePlanet.Air)
            effectTxt_Info.text = TextMan.Get("airtransform");
        else if (showingParticlePlanetEffect == TypePlanet.Antimatter)
            effectTxt_Info.text = TextMan.Get("antimattertransform"); //"Transform into Antimatter Planet, increasing explosion radius and destroy any enemy planet in range.";
        else if (showingParticlePlanetEffect == TypePlanet.Fire)
            effectTxt_Info.text = TextMan.Get("firetransform"); //"Transform into Fire Planet, increasing extra 150% damage. Strong against Air Planet.";
        else if (showingParticlePlanetEffect == TypePlanet.Ice)
            effectTxt_Info.text = TextMan.Get("icetransform"); //"Transform into Ice Planet, decreasing 55% enemy's speed.";
        else if (showingParticlePlanetEffect == TypePlanet.Gravity)
            effectTxt_Info.text = TextMan.Get("gravitytransform"); //"Transform into Gravity Planet, increasing gravitational force and radius as well as attracted material.";
        else
            effectTxt_Info.text = TextMan.Get("defaulttransform"); //"There is no effect here.";
    }

    private void UpdateDetailPanel()
    {
        #region Minh.ho: Check change color
        if (detailTxt_Gravity.text != Utilities.MoneyShorter((int)Planet.DataPlanet.GetGravity(), 1) + " N")
        {
            detailTxt_Gravity.color = Color.green;
        }    
        else
        {
            detailTxt_Gravity.color = Color.white;
        }

        if (detailTxt_Size.text != Utilities.MoneyShorter((int)Planet.DataPlanet.GetSize(), 1) + " Km2")
        {
            detailTxt_Size.color = Color.green;
        }
        else
        {
            detailTxt_Size.color = Color.white;
        }

        if (detailTxt_HP.text != Utilities.MoneyShorter((int)Planet.DataPlanet.GetHP(), 1))
        {
            detailTxt_HP.color = Color.green;
        }
        else
        {
            detailTxt_HP.color = Color.white;
        }

        if (detailTxt_Speed.text != Utilities.MoneyShorter((int)Planet.DataPlanet.GetSpeed(), 1) + " Km/h")
        {
            detailTxt_Speed.color = Color.green;
        }
        else
        {
            detailTxt_Speed.color = Color.white;
        }

        if (detailTxt_Material.text != Utilities.MoneyShorter((int)Planet.DataPlanet.GetCollectMaterial()) + " mat/s")
        {
            detailTxt_Material.color = Color.green;
        }
        else
        {
            detailTxt_Material.color = Color.white;
        }
        #endregion

        detailTxt_Gravity.text = Utilities.MoneyShorter((int)Planet.DataPlanet.GetGravity(), 1) + " N";
        detailTxt_Size.text = Utilities.MoneyShorter((int)Planet.DataPlanet.GetSize(), 1) + " Km2";
        detailTxt_HP.text = Utilities.MoneyShorter((int)Planet.DataPlanet.GetHP(), 1);
        detailTxt_Speed.text = Utilities.MoneyShorter((int)Planet.DataPlanet.GetSpeed(), 1) + " Km/h";
        detailTxt_Material.text = Utilities.MoneyShorter((int)Planet.DataPlanet.GetCollectMaterial()) + " mat/s";
    }

    public static void Check()
    {
        if (_Instance.BtnSpeed == null)
        {
            Debug.Log("==>> btnIsNull");
            return;
        }

        if (DataGameSave.dataLocal.M_Material >= _Instance.M_MatNeedSpeed && _Instance.Planet.LevelSpeed < 10)
        {
            _Instance.BtnSpeed.interactable = true;
        }
        else
        {
            _Instance.BtnSpeed.interactable = false;
        }

        _Instance.BtnSpeed.GetComponent<Image>().color = _Instance.BtnSpeed.interactable ? Color.white : Color.gray;

        if (DataGameSave.dataLocal.M_Material >= _Instance.M_MatNeedSize && _Instance.Planet.LevelSize < 10)
        {
            _Instance.BtnSize.interactable = true;
        }
        else
        {
            _Instance.BtnSize.interactable = false;
        }

        _Instance.BtnSize.GetComponent<Image>().color = _Instance.BtnSize.interactable ? Color.white : Color.gray;

        if (DataGameSave.dataLocal.M_Material >= _Instance.M_MatNeedHeavier && _Instance.Planet.LevelHeavier < 10)
        {
            _Instance.BtnHeavier.interactable = true;
        }
        else
        {
            _Instance.BtnHeavier.interactable = false;
        }

        _Instance.BtnHeavier.GetComponent<Image>().color = _Instance.BtnHeavier.interactable ? Color.white : Color.gray;
    }

    private void Update()
    {
        redDotUpgrade.SetActive(PlayScenesManager.Instance && PlayScenesManager.Instance.redDotUpgrades[0].activeSelf);
    }

    public override void Appear()
    {
        base.Appear();
        SetData();

        AnimationHelper.AnimatePopupShowScaleHalf(
            this,
            Background.GetComponent<Image>(),
            Panel.gameObject,
            Panel,
            PopupConstants.TIME_MULTIPLY_APPEAR);

        EazySoundManager.PlaySound(Sounds.Instance.Show_Popup);
    }

    public override void Disappear()
    {

        AnimationHelper.AnimatePopupCloseScaleHalf(
            this,
            Background.GetComponent<Image>(),
            Panel.gameObject,
            Panel,
            PopupConstants.TIME_MULTIPLY_DISAPPEAR,
            () =>
            {
                base.Disappear();
                IsShow = false;
                if (_onClose != null)
                {
                    _onClose.Invoke();
                    _onClose = null;
                }

            });
    }

    public override void Disable()
    {
        base.Disable();
    }

    public override void NextStep(object value = null)
    {

    }

    public void UpdateMoney(int _Collect)
    {
        MoneyCollect = _Collect;
        detailTxt_CurrentCollectMaterial.text = Utilities.MoneyShorter(_Collect, 1).ToString();
    }

    public void OnClose()
    {
        if (GameStatics.IsAnimating)
            return;

        _onClose = () =>
        {
            if (SpaceManager.Instance)
            {
                SpaceManager.Instance.PlanetSelect = null;

                if (SpaceManager.Instance.FxBack)
                    SpaceManager.Instance.FxBack.SetActive(false);

                DataGameSave.SaveToServer();
            }
        };

        Disappear();
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnUpBigger()
    {
        DataGameSave.dataServer.ListPlanet[Planet.Manager.IndexSpace].LevelSize++;
        Planet.LevelSize = DataGameSave.dataServer.ListPlanet[Planet.Manager.IndexSpace].LevelSize;
        DataGameSave.dataLocal.M_Material -= M_MatNeedSize;
        SetDataSize();
        Check();
        SpaceManager.Instance.UpdateRadiusAllPlanet();
        DataGameSave.dataLocal.UpgradePlanet++;
        DataGameSave.dataLocal.dailyMissions[(int)DailyQuests.UpgradePlanet].currentProgress++;

        if (BtnDailyMission.Instance)
        {
            BtnDailyMission.Instance.CheckDoneQuest();
        }

        AnalyticsManager.Instance.TrackUpgradePlanet("Size");
        audioSource.clip = Sounds.Instance.Upgrade;
        audioSource.Play();
        //EazySoundManager.PlaySound(Sounds.Instance.Upgrade);
    }

    public void OnUpFaster()
    {
        DataGameSave.dataServer.ListPlanet[Planet.Manager.IndexSpace].LevelSpeed++;
        Planet.LevelSpeed = DataGameSave.dataServer.ListPlanet[Planet.Manager.IndexSpace].LevelSpeed;
        DataGameSave.dataLocal.M_Material -= M_MatNeedSpeed;
        SetDataSpeed();
        Check();
        DataGameSave.dataLocal.UpgradePlanet++;
        DataGameSave.dataLocal.dailyMissions[(int)DailyQuests.UpgradePlanet].currentProgress++;

        if (BtnDailyMission.Instance)
        {
            BtnDailyMission.Instance.CheckDoneQuest();
        }

        AnalyticsManager.Instance.TrackUpgradePlanet("Speed");
        audioSource.clip = Sounds.Instance.Upgrade;
        audioSource.Play();
        //EazySoundManager.PlaySound(Sounds.Instance.Upgrade);
    }

    public void OnUpHeavier()
    {
        DataGameSave.dataServer.ListPlanet[Planet.Manager.IndexSpace].LevelHeavier++;
        if (BtnDailyMission.Instance)
        {
            BtnDailyMission.Instance.CheckDoneQuest();
        }
        Planet.LevelHeavier = DataGameSave.dataServer.ListPlanet[Planet.Manager.IndexSpace].LevelHeavier;
        DataGameSave.dataLocal.M_Material -= M_MatNeedHeavier;
        SetDataHeavier();
        Check();
        DataGameSave.dataLocal.UpgradePlanet++;
        DataGameSave.dataLocal.dailyMissions[(int)DailyQuests.UpgradePlanet].currentProgress++;
        AnalyticsManager.Instance.TrackUpgradePlanet("Weight");
        audioSource.clip = Sounds.Instance.Upgrade;
        audioSource.Play();
        //EazySoundManager.PlaySound(Sounds.Instance.Upgrade);
    }

    public void OnDetail()
    {
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
        btnImage_Detail.sprite = buttonSprite_Active;
        btnImage_Upgrade.sprite = buttonSprite_Inactive;
        btnImage_Effect.sprite = buttonSprite_Inactive;
    }

    public void OnUpgrade()
    {
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
        btnImage_Detail.sprite = buttonSprite_Inactive;
        btnImage_Upgrade.sprite = buttonSprite_Active;
        btnImage_Effect.sprite = buttonSprite_Inactive;
    }

    public void OnEffect()
    {
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
        btnImage_Detail.sprite = buttonSprite_Inactive;
        btnImage_Upgrade.sprite = buttonSprite_Inactive;
        btnImage_Effect.sprite = buttonSprite_Active;

        showingParticlePlanetEffect = Planet.Type;

        if (showingParticlePlanetEffect >= TypePlanet.Default)
            showingParticlePlanetEffect = TypePlanet.Ice;

        SetParticlePlanetEffect();
    }

    private void ShowWarningText(string content = null)
    {
        if (warningText)
            warningText.text = content;
    }

    public bool OnNewPlanet() // Revolution!
    {
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
        var planet = DataGameSave.dataServer.ListPlanet[Planet.Manager.IndexSpace];

        if (MainPlanetController.Instance.Data.Level - 1 <= (int)planet.Name)
        {
            ShowWarningText(TextMan.Get("You need to UPGRADE level of the Sun before"));
            return false;
        }

        effectLevelUp.GetComponent<ParticleSystem>()?.Play();
        animTextLevelUP?.Play("AnimLevelUP");

        ShowWarningText();
        planet.Name++;
        redDotAvatar.SetActive(true);
        Utilities.SetPlayerPrefsBool("redDotAvatar", true);

        nameTxt.text = planet.Name.ToString();
        levelTxt.text = "Lv. " + ((int)Planet.DataPlanet.Name + 1);

        spriteAnim.Set(planet.Name.ToString());

        // size

        DataGameSave.dataServer.ListPlanet[Planet.Manager.IndexSpace].LevelSize = 0;
        Planet.LevelSize = 0;
        SetDataSize();

        // speed

        DataGameSave.dataServer.ListPlanet[Planet.Manager.IndexSpace].LevelSpeed = 0;
        Planet.LevelSpeed = 0;
        SetDataSpeed();

        // weight

        DataGameSave.dataServer.ListPlanet[Planet.Manager.IndexSpace].LevelHeavier = 0;
        Planet.LevelHeavier = 0;
        SetDataHeavier();

        // Other

        Check();
        Planet.OnNewPlanet();
        audioSource.clip = Sounds.Instance.Upgrade;
        audioSource.Play();
        //EazySoundManager.PlaySound(Sounds.Instance.Upgrade);

        return true;
    }

    public bool OnAddEffect()
    {
        if (Planet.Type < TypePlanet.Default) // already had effect
        {
            PopupConfirm.ShowOK("Oops", TextMan.Get("This planet already has a effect!"), "OK");
            return false;
        }

        if (showingParticlePlanetEffect >= TypePlanet.Default)
        {
            PopupConfirm.ShowOK("Oops", TextMan.Get("You can't add the Default effect!"), "OK");
            return false;
        }

        var cost = DataMaster.GetAddEffectCost(showingParticlePlanetEffect);

        //cost.LogBlue(showingParticlePlanetEffect);

        if (showingParticlePlanetEffect == TypePlanet.Ice && !MenuItem_Debug_FullMoney.IsTrue(false) && cost.ice > DataGameSave.dataLocal.M_Ice)
        {
            PopupConfirm.ShowOK("Oops", TextMan.Get("You are not enough of Ice Material!"), "OK");
            return false;
        }
        else if (showingParticlePlanetEffect == TypePlanet.Ice)
            DataGameSave.dataLocal.M_Ice -= cost.ice;

        if (showingParticlePlanetEffect == TypePlanet.Fire && !MenuItem_Debug_FullMoney.IsTrue(false) && cost.fire > DataGameSave.dataLocal.M_Fire)
        {
            PopupConfirm.ShowOK("Oops", TextMan.Get("You are not enough of Fire Material!"), "OK");
            return false;
        }
        else if (showingParticlePlanetEffect == TypePlanet.Fire)
            DataGameSave.dataLocal.M_Fire -= cost.fire;

        if (showingParticlePlanetEffect == TypePlanet.Gravity && !MenuItem_Debug_FullMoney.IsTrue(false) && cost.gravity > DataGameSave.dataLocal.M_Gravity)
        {
            PopupConfirm.ShowOK("Oops", TextMan.Get("You are not enough of Gravity Material!"), "OK");
            return false;
        }
        else if (showingParticlePlanetEffect == TypePlanet.Gravity)
            DataGameSave.dataLocal.M_Gravity -= cost.gravity;

        if (showingParticlePlanetEffect == TypePlanet.Air && !MenuItem_Debug_FullMoney.IsTrue(false) && cost.air > DataGameSave.dataLocal.M_Air)
        {
            PopupConfirm.ShowOK("Oops", TextMan.Get("You are not enough of Air Material!"), "OK");
            return false;
        }
        else if (showingParticlePlanetEffect == TypePlanet.Air)
            DataGameSave.dataLocal.M_Air -= cost.air;

        if (showingParticlePlanetEffect == TypePlanet.Antimatter && !MenuItem_Debug_FullMoney.IsTrue(false) && cost.antimatter > DataGameSave.dataLocal.M_Antimatter)
        {
            PopupConfirm.ShowOK("Oops", TextMan.Get("You are not enough of Antimatter Material!"), "OK");
            return false;
        }
        else if (showingParticlePlanetEffect == TypePlanet.Antimatter)
            DataGameSave.dataLocal.M_Antimatter -= cost.antimatter;

        // logic

        if (showingParticlePlanetEffect == TypePlanet.Gravity || showingParticlePlanetEffect == TypePlanet.Air) // add effect 24h
        {
            Planet.DataPlanet.addedEffectTick = DateTime.Now.Ticks;
        }

        audioSource.clip = Sounds.Instance.Upgrade;
        audioSource.Play();
        //EazySoundManager.PlaySound(Sounds.Instance.Upgrade);
        DataGameSave.dataServer.ListPlanet[Planet.Manager.IndexSpace].type = showingParticlePlanetEffect;
        Planet.Type = showingParticlePlanetEffect;
        DataGameSave.SaveToServer();
        SetCostEffects();
        DataGameSave.SaveToLocal();

        // add effect (to ui)

        Planet.UpdateEffectOutside();

        // res

        return true;
    }

    public void OnCollectX2()
    {
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);

        if (MoneyCollect <= 0)
            return;

        GoogleMobileAdsManager.Instance.ShowRewardedVideo(() =>
        {
            DataGameSave.dataLocal.M_Material += 2 * MoneyCollect;
            SpaceManager.Instance.PlanetSelect.ResetCollect();
            MoneyCollect = 0;
            detailTxt_CurrentCollectMaterial.text = "000";
        });
    }

    public void OnClockWise(bool clockwise)
    {
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
        SpaceManager.Instance.PlanetSelect.InverseClockwise(clockwise);
    }

    public void OnCollect()
    {
        DataGameSave.dataLocal.M_Material += MoneyCollect;
        SpaceManager.Instance.PlanetSelect.ResetCollect();
        MoneyCollect = 0;
        detailTxt_CurrentCollectMaterial.text = "000";
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnPressPlanetEffects(int type)
    {
        showingParticlePlanetEffect = (TypePlanet)type;

        if (OnAddEffect())
            SetParticlePlanetEffect();
    }

    public void OnNextPlanetEffect(bool next)
    {
        if (next && showingParticlePlanetEffect <= TypePlanet.Default - 1)
            showingParticlePlanetEffect++;
        else if (!next && (int)showingParticlePlanetEffect > 0)
            showingParticlePlanetEffect--;
        else
            return;

        SetParticlePlanetEffect();
    }

    public void OnNextPlanet(bool next)
    {
        var cur = SpaceManager.Instance.ListSpace.FindIndex(i => i.Planet == SpaceManager.Instance.PlanetSelect);

        if (cur == -1)
            return;

        if (next && cur < SpaceManager.Instance.ListSpace.Count - 1)
            cur++;
        else if (!next && cur > 0)
            cur--;
        else
            return;

        SpaceManager.Instance.PlanetSelect = SpaceManager.Instance.ListSpace[cur].Planet;
        SetData();
    }

    public void ButtonGroupUpgrade()
    {
        if(isManage)
        {
            isManage = false;
            CheckShowGroup();
        }
    }
    public void ButtonGroupManage()
    {
        if (!isManage)
        {
            isManage = true;
            CheckShowGroup();
        }
    }
    public void CheckShowGroup()
    {
        group_Manage.SetActive(isManage);
        group_Upgrade.SetActive(!isManage);  
    }

    #region Init SkinCell Mini List

    public List<SkinCellMini> listScriptMiniSkinCell = new List<SkinCellMini>();
    public List<PopupSkinPlanet.OneSkinCell> listSkinCells = new List<PopupSkinPlanet.OneSkinCell>();
    public GameObject holderMiniSkinCell;
    public GameObject prefabsSkinCellMini;
    public void InitSkinCellMiniList(List<PopupSkinPlanet.OneSkinCell> listPlanetCells_Mini)
    {
        listSkinCells = listPlanetCells_Mini;
        if (listScriptMiniSkinCell.Count > 0)
        {
            foreach (SkinCellMini item in listScriptMiniSkinCell)
            {
                item.CheckUnlock();
            }
            return;
        }

        listScriptMiniSkinCell.Clear();
        if (listPlanetCells_Mini.Count > 0)
        {
            for (int i = 0; i < listPlanetCells_Mini.Count; i++)
            {
                GameObject oneSkinCell_Mini = Instantiate(prefabsSkinCellMini, holderMiniSkinCell.transform);
                oneSkinCell_Mini.SetActive(true);
                var scriptOneSkinCell_Mini = oneSkinCell_Mini.GetComponent<SkinCellMini>();
                scriptOneSkinCell_Mini.SetData(i, listPlanetCells_Mini[i], UpdateSelected_MiniSkinCell);
                listScriptMiniSkinCell.Add(scriptOneSkinCell_Mini);
            }
        }
    }

    public void UpdateSelected_MiniSkinCell(int index, float bonusPercent_with_Level)
    {
        foreach (SkinCellMini item in listScriptMiniSkinCell)
        {
            if (item.index_SkinCellMini == index)
            {
                item.tick.SetActive(true);
            }
            else
            {
                item.tick.SetActive(false);
            }
        }

        float finalBonusPercent_Damage = listSkinCells[index].damage_BonusPercent_with_SkinType + listSkinCells[index].damage_BonusPercent_with_SkinType * bonusPercent_with_Level;
        float finalBonusPercent_Size = listSkinCells[index].size_BonusPercent_with_SkinType + listSkinCells[index].size_BonusPercent_with_SkinType * bonusPercent_with_Level;
        float finalBonusPercent_HP = listSkinCells[index].hp_BonusPercent_with_SkinType + listSkinCells[index].hp_BonusPercent_with_SkinType * bonusPercent_with_Level;
        float finalBonusPercent_Speed = listSkinCells[index].speed_BonusPercent_with_SkinType + listSkinCells[index].speed_BonusPercent_with_SkinType * bonusPercent_with_Level;
        float finalBonusPercent_MaterialFarm = listSkinCells[index].materialFarm_BonusPercent_with_SkinType + listSkinCells[index].materialFarm_BonusPercent_with_SkinType * bonusPercent_with_Level;



        SpaceManager.Instance.PlanetSelect.DataPlanet.skin = (PLanetName)index;
        DataGameSave.SaveToServer();
        if (SpaceManager.Instance.PlanetSelect)
        {
            SpaceManager.Instance.PlanetSelect.UpdateSkin();
        }
        UpdateDetailPanel();
    }
    #endregion
}

