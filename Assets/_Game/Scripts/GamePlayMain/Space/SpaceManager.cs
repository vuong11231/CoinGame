using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hellmade.Sound;
using System.IO;
using SteveRogers;
using TMPro;

public class SpaceManager : MonoBehaviour {
    public static SpaceManager Instance;

    public SpaceController spacePrefab;
    public MainPlanetController MainPlanet;
    public ParticleSystem effectCreatSun;
    public List<SpaceController> ListSpace;

    public GameObject restoreTimePanelGo;
    public GameObject restoreTimeWatchAds;
    public Transform restoreTimeItems;
    public TextMeshProUGUI txtAutoRestoreCount;

    public Sprite defaultAvatar;
    public Text TxtLevel;
    public Image Avatar;

    [HideInInspector]
    public GameObject FxBack;
    [HideInInspector]
    public PlanetController PlanetSelect;
    [HideInInspector]
    public DataBaseSun Data;

    public int grav;

    int IndexChecked;

    public int Level {
        get { return DataGameSave.dataServer.level; }

        set {
            if (DataGameSave.dataServer.level != value)
                DataGameSave.dataServer.level = value;
        }
    }

    public float LastPlanetRadius {
        get {
            if (ListSpace.IsNullOrEmpty())
                return -1;

            return ListSpace[ListSpace.Count - 1].Radius;
        }
    }

    public float SpaceRadius {
        get {
            if (ListSpace.IsNullOrEmpty() || !ListSpace[ListSpace.Count - 1].Planet)
                return -1;

            return Vector3.Distance(ListSpace[ListSpace.Count - 1].Planet.transform.position, transform.position);
        }
    }

    void Awake() {
        Instance = this;
        restoreTimePanelGo.CheckAndSetActive(false);

        if (txtAutoRestoreCount != null) {
            int count = 0;
            int.TryParse(DataGameSave.GetMetaData(MetaDataKey.SHOP_AUTO_RESTORE_COUNT), out count);
            txtAutoRestoreCount.text = count + "/" + GameManager.MAX_WATCH_ADS_PER_DAY_LIMIT;
        }

        // check battle result
        if (DataGameSave.dataServer.ListEnemy != null && DataGameSave.dataServer.ListEnemy.Count > 0) {
            PopupBattleResult.Show();
        }

        // setup restore skip btns
        if (!restoreTimeItems || !restoreTimeWatchAds) {
            return;
        }

        restoreTimeWatchAds.CheckAndSetActive(false);
        LeanTween.delayedCall(3f, () => {
            restoreTimeWatchAds.CheckAndSetActive(true);
        });

        foreach (Transform i in restoreTimeItems) {
            var index = i.GetSiblingIndex();
            var btn = i.GetChild(2).GetComponent<Button>();
            Transform trans = i;

            btn.onClick.AddListener(() => {
                var cost = btn.transform.GetChild(2).GetComponent<Text>().text.Parse(0);
                var autoRestore = GameManager.IsAutoRestorePlanet;

                if (autoRestore || DataGameSave.dataLocal.Diamond >= cost) {
                    if (!autoRestore)
                        DataGameSave.dataLocal.Diamond -= cost;

                    DataGameSave.dataServer.ListPlanet[index].ShootTime = DateTime.MinValue;
                    Instance.ListSpace[index].Planet.SetData(DataGameSave.dataServer.ListPlanet[index], index);
                    trans.gameObject.SetActive(false);
                    DataGameSave.SaveToServer();
                } else {
                    PopupShop.Show(true);
                }
            });
        }
    }

    public void WatchAdsToRestoreAllPlanet() {
        if (!restoreTimeItems) {
            return;
        }

        GoogleMobileAdsManager.Instance.ShowRewardedVideo(() => {
            foreach (Transform i in restoreTimeItems) {
                var index = i.GetSiblingIndex();
                Transform trans = i;
                DataGameSave.dataServer.ListPlanet[index].ShootTime = DateTime.MinValue;
                Instance.ListSpace[index].Planet.SetData(DataGameSave.dataServer.ListPlanet[index], index);
                trans.gameObject.SetActive(false);
            }

            PopupConfirm.ShowOK(TextMan.Get("Congratulations"), TextMan.Get("You planets are restored"));
        });
    }

    public void WatchAdsToAutoRestore(){
        int count = 0;
        int.TryParse(DataGameSave.GetMetaData(MetaDataKey.SHOP_AUTO_RESTORE_COUNT), out count);
        if (count <= 0) {
            PopupConfirm.ShowOK("Oops", TextMan.Get("You reach watch ads limit for today"),"OK",()=> {
                PopupShop.Show(true);
            });
            return;
        }

        GoogleMobileAdsManager.Instance.ShowRewardedVideo(() => {
            count--;
            txtAutoRestoreCount.text = count + "/" + GameManager.MAX_WATCH_ADS_PER_DAY_LIMIT;
            DataGameSave.SaveMetaData(MetaDataKey.SHOP_AUTO_RESTORE_COUNT, count.ToString());

            string message = string.Format(TextMan.Get("You activated auto restore for {0} mins."), GameManager.AUTO_RESTORE_WATCH_ADS_MINUTE);
            PopupConfirm.ShowOK(TextMan.Get("Congratulations"), message, "Great", () => {
                
                    if (!GameManager.IsAutoRestorePlanet) {
                    DataGameSave.autoRestoreEndTime = GameManager.Now.Ticks;
                }

                DateTime newEndTime = new DateTime(DataGameSave.autoRestoreEndTime).AddMinutes(GameManager.AUTO_RESTORE_WATCH_ADS_MINUTE);
                DataGameSave.autoRestoreEndTime = newEndTime.Ticks;
                DataGameSave.SaveToServer();
            });
        });
    }

    public void CheckAllPlanet()
    {
        if (Scenes.Current != SceneName.Gameplay)
            return;

        var count = ListSpace.Count;
        for (int i = 0; i < count; i++)
        {
            if (ListSpace[i].Planet)
                ListSpace[i].Planet.CheckCanUpgrade();
        }
        MainPlanet.CheckCanUpgrade();
    }

    void Start()
    {
        Level = DataGameSave.dataServer.level;
        inited = true;

        if (Scenes.IsBattleScene())
        {
            var orbitNumber = ReadDataSun.Instance.ListDataSun.GetLastIfOverRange(Level - 1).AmountOrbit;

            if (orbitNumber > 2)
                transform.position = new Vector3(transform.position.x, -30 - orbitNumber);

            StartCoroutine(SetDistance_CRT());
        }

        if (DataGameSave.dataServer.Name != "")
        {
            SetDataStart();
        }

        if (TxtLevel)
            TxtLevel.text = "Lv." + Level;

        UpdateAvatar();
    }

    public void UpdateAvatar()
    {
        try
        {
            if (Avatar)
                Avatar.sprite = defaultAvatar;
        }
        catch { }
    }

    public static bool inited { get; private set; } = true;

    private IEnumerator SetDistance_CRT()
    {
        inited = false;

        while (SpaceRadius <= 0 || SpaceEnemyManager.Instance.SpaceRadius <= 0) // wait for creating all planets done
        {
            yield return null;
        }

        var subdis = (CameraManager.Instance.MainCamera.orthographicSize * 20 / 163f);

        LeanTween.moveY(SpaceEnemyManager.Instance.gameObject, CameraManager.Instance.MainCamera.orthographicSize - SpaceEnemyManager.Instance.SpaceRadius - subdis, 1);
        LeanTween.moveY(SpaceManager.Instance.gameObject, -CameraManager.Instance.MainCamera.orthographicSize + SpaceRadius + subdis, 1)
            .setOnComplete(() => inited = true);

        yield break;
    }

    public void SetDataStart()
    {
        SetData();
        StartCoroutine(InitListSpace());
    }

    public bool callInitedListSpace = false;

    public IEnumerator InitListSpace(DataGameServer data = null)
    {
        if (!TutMan.IsDone(TutMan.TUT_KEY_01_HIDE_GO_WHEN_FIRST_ENTER_GAME))
            yield break;

        if (data == null) {
            data = DataGameSave.dataServer;
        }

        callInitedListSpace = true;
        ListSpace = new List<SpaceController>();

        for (int i = 0; i < Data.AmountOrbit; i++)
        {
            DrawOrbit(true, i);

            yield return new WaitForSeconds(0.25f);

            if (i < data.ListPlanet.Count)
            {
                ListSpace[i].SpawnPlanetDefault(data.ListPlanet[i], i);
            }
            else
            {
                ListSpace[i].SpawnPlanet(TypePlanet.Default, ListSpace[i].Orbit.Line.GetPosition(0));
            }
        }

        GameStatics.IsAnimating = false;

        if (CameraManager.Instance)
            CameraManager.Instance.SetSize(Data.AmountOrbit);
    }

    public void ShowOrbit(int Amount)
    {
        if (ListSpace != null && ListSpace.Count >= DataMaster.MAX_PLANETS_NUMBER)
            return;

        DrawOrbit(false, Amount - 1);

        if (Amount - 1 < DataGameSave.dataServer.ListPlanet.Count)
        {
            ListSpace[Amount - 1].SpawnPlanetDefault(DataGameSave.dataServer.ListPlanet[Amount - 1], Amount - 1);
        }
        else
        {
            ListSpace[Amount - 1].SpawnPlanet(TypePlanet.Default, ListSpace[Amount - 1].Orbit.Line.GetPosition(0));
        }

        if (CameraManager.Instance)
            CameraManager.Instance.SetSize(Data.AmountOrbit);
    }

    void DrawOrbit(bool _, int count)
    {
        var space = Instantiate(spacePrefab, transform);

        space.transform.position = new Vector3(space.transform.position.x, space.transform.position.y, 10);
        ListSpace.Add(space);
        ListSpace[count].IsShow = true;
        space.IndexSpace = count;
        space.UpdateRadius(false);
        space.Orbit.drawCircle.enabled = true;
    }

    public bool CheckPosition(float _Distance)
    {
        TurnOffCheck();
        if (_Distance > 2.5f && _Distance < 3.5f)
        {
            IndexChecked = 0;
            return ListSpace[0].Check();
        }
        else if (_Distance > 4.5f && _Distance < 5.5f && ListSpace.Count > 1)
        {
            IndexChecked = 1;
            return ListSpace[1].Check();
        }
        else if (_Distance > 6.5f && _Distance < 7.5f && ListSpace.Count > 2)
        {
            IndexChecked = 2;
            return ListSpace[2].Check();
        }
        else if (_Distance > 8.5f && _Distance < 9.5f && ListSpace.Count > 3)
        {
            IndexChecked = 3;
            return ListSpace[3].Check();
        }
        else if (_Distance > 10.5f && _Distance < 11.5f && ListSpace.Count > 4)
        {
            IndexChecked = 4;
            return ListSpace[4].Check();
        }
        else if (_Distance > 12.5f && _Distance < 13.5f && ListSpace.Count > 5)
        {
            IndexChecked = 5;
            return ListSpace[5].Check();
        }
        else if (_Distance > 14.5f && _Distance < 15.5f && ListSpace.Count > 6)
        {
            IndexChecked = 6;
            return ListSpace[6].Check();
        }
        else if (_Distance > 16.5f && _Distance < 17.5f && ListSpace.Count > 7)
        {
            IndexChecked = 7;
            return ListSpace[7].Check();
        }
        else if (_Distance > 18.5f && _Distance < 19.5f && ListSpace.Count > 8)
        {
            IndexChecked = 8;
            return ListSpace[8].Check();
        }
        else if (_Distance > 20.5f && _Distance < 21.5f && ListSpace.Count > 9)
        {
            IndexChecked = 9;
            return ListSpace[9].Check();
        }
        else if (_Distance > 22.5f && _Distance < 23.5f && ListSpace.Count > 10)
        {
            IndexChecked = 10;
            return ListSpace[10].Check();
        }
        else
        {
            return false;
        }
    }

    void TurnOffCheck()
    {
        ListSpace[IndexChecked].Orbit.SetLineDefault();
    }

    public int CheckPositionToChange(PlanetController Child, Vector3 Pos, int Index)
    {
        if (ListSpace[IndexChecked].IsHavePlanet) //PlanetController Child, Vector3 Pos
        {
            int IndexParent = Child.Manager.IndexSpace;
            Child.OffTrail();
            Child.Manager.Planet = ListSpace[IndexChecked].Planet;
            ListSpace[IndexChecked].Planet.Manager = Child.Manager;
            ListSpace[IndexChecked].Planet.OffTrail();
            ListSpace[IndexChecked].Planet.StopToChange(Index);
            ListSpace[IndexChecked].Planet.transform.SetParent(ListSpace[IndexParent].transform);
            Child.Manager = ListSpace[IndexChecked];

            return Child.Manager.Orbit.CheckPos(Pos);
        }
        else //PlanetController Child, int Index
        {
            Child.Manager = ListSpace[IndexChecked];
            return Index;

        }
    }

    public void TurnOffLine()
    {
        ListSpace[IndexChecked].Orbit.SetLineDefault();
    }

    void SetData()
    {
        Data = ReadDataSun.Instance.ListDataSun.GetLastIfOverRange(Level - 1);
        MainPlanet.SetData(Data);
        DataGameSave.dataBattle = DataGameSave.dataServer;
    }

    public void UpgradeSun()
    {
        TxtLevel.text = "Lv." + Level;
        Data = ReadDataSun.Instance.ListDataSun.GetLastIfOverRange(Level - 1);
        MainPlanet.Size = Data.Size;
        MainPlanet.Data.Level = Data.Level;
        ShowOrbit(Data.AmountOrbit);
        MainPlanet.SetMaterialAfterUpdate();
    }

    public void SetFxBack(GameObject planet)
    {
        if (FxBack == null)
        {
            FxBack = Instantiate(
                Resources.Load<GameObject>("Prefabs/Fx/Fx Back"));
        }
        FxBack.SetActive(true);
        FxBack.transform.SetParent(planet.transform);
        FxBack.transform.position = planet.transform.position;
        FxBack.transform.localScale = Vector3.one * 2;
    }

    public bool CheckCanUpgrade()
    {
        if (ListSpace == null)
            return false;

        var count = ListSpace.Count;

        for (int i = 0; i < count; i++)
        {
            if (ListSpace[i].Planet == null)
                continue;

            ListSpace[i].Planet.CheckCanUpgrade();

            if (ListSpace[i].Planet.CanUpgrade)
                return true;
        }

        return false;
    }

    public void OnPressed_UpgradePlanet()
    {
        if (ListSpace.IsNullOrEmpty())
            return;

        ListSpace[0].Planet.OnPressed_UpgradePlanet();
    }

    public void UpdateRadiusAllPlanet()
    {
        var count = ListSpace.Count;

        for (int i = 0; i < count; i++)
        {
            ListSpace[i].UpdateRadius(false);
        }

        if (UIMultiScreenCanvasMan.modeExplore == UIMultiScreenCanvasMan.Mode.Gameplay)
            CameraManager.Instance.SetSize();
    }

    public void StopAllPlanet()
    {
        var count = ListSpace.Count;
        for (int i = 0; i < count; i++)
        {
            LeanTween.cancel(ListSpace[i].Planet.gameObject);
        }
    }
    public void ResumePlanet()
    {
        var count = ListSpace.Count;
        for (int i = 0; i < count; i++)
        {
            if (ListSpace[i].Planet.Type != TypePlanet.Destroy)
                ListSpace[i].Planet.SpinAround();
        }
    }

    public void PlayEffectCreatSun()
    {
        if (effectCreatSun != null)
        {
            effectCreatSun.Play();
        }
    }
}
