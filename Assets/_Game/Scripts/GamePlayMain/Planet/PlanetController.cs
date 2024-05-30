using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;
using System;
using TMPro;
using Hellmade.Sound;
using SteveRogers;
using UnityEngine.SceneManagement;

public class PlanetController : MonoBehaviour {
    public static int MY_GRAVITY_MIN_LEVEL = 5;
    public static float MY_GRAVITY_RANGE_START = 15f;
    public static float MY_GRAVITY_RANGE_INCREASE_BY_LEVEL = 5f;
    public static float MY_GRAVITY_FORCE_START = 400f;
    public static float MY_GRAVITY_FORCE_INCREASE = 200f;

    public static float BOSS_DIRECT_ATTACK_FORCE = 60f;

    public enum Owner { Player, Enemy }
    public Owner owner = Owner.Player;

    public GameObject[] EffectDestroy;
    public Gradient[] TrailEffects;

    [HideInInspector]
    public IEnumerator IEShoot;

    //public GameObject 
    public GameObject PreCollectionMaterial;
    public GameObject EffectTrailGameplay;
    public GameObject WaitFx;
    public GameObject radiusGo;
    public GameObject target;
    public GameObject FxFreeze;
    public GameObject fireParticleByGotDameByFireAttack = null;
    public GameObject RedDot;
    public GameObject go;

    public Image hpFill = null;
    public Transform bloodGo = null;
    public TypePlanet Type;
    public LineRenderer Orbit;
    public SpriteAnim spriteAnim = null;
    public CircleCollider2D planetCollider;
    public SpaceController Manager;
    public Rigidbody2D rigid;
    public PlanetBombZone Zone;
    public TextMeshPro TxtCollect;
    public TrailRenderer LongTrail;
    public ParticleSystem effectTrail;

    public string enemyTag = GameConstants.ENEMY_TAG;
    public int Index = 0;
    public int Level;
    public int MoneyCollect = 0;
    public float Dame;
    public float Gravitation;
    public float Speed;
    public float speed;
    public float ScaleSize;
    public float size;
    public float startHP = 0;
    public bool showHPBar = false;
    public bool isFlying;
    public bool isActive = false;
    public bool CanUpgrade;
    public bool attackDirect = false;

    Coroutine collectCorou;
    Coroutine ShowTrail;
    Coroutine ReturnPlanet;
    Coroutine SlowCoroutine;

    Vector3 dir;
    Vector3 currentDown;
    Vector3 startDragPosition;
    Vector3 endDragPosition;
    Vector3 mousePosition;

    int oldIndex;
    int moveId;

    float Time;
    float HPinBattle;
    float TimeMaxCollect = 0;
    float elapsedTimeFly;
    float SlowSpeed = 1;

    bool IsDrag = false;
    bool IsActive = false;
    bool _isTap;
    bool _IsClick;
    bool airInvertEffect = false;

    /// <summary>
    /// 0: stop moving, -1: not being attacked, >0: slowing down
    /// </summary>
    private int iceStopCount = -1;

    /// <summary>
    /// -1: not being attacked, >0: counting time, >1: should got dame now
    /// </summary>
    private float fireLastTimeGotDame = -1;
    private PlanetController fireAttackUserPlanet = null;

    /// <summary>
    /// -1 inverting
    /// = 0 not being effect
    /// 1 normal dir
    /// </summary>
    private int invertedDirWhenAttackAirPlanet_State = 0;

    private LTDescr leanMoveToHoleAntiMatter = null;
    private bool isCreatedAntiMatterZone = false;

    public void GotAttacked(TypePlanet type, bool start, PlanetController userPlanetController, IceAttackZone zone) // got attacked by user
    {
        if (type == TypePlanet.Ice) {
            if (start) {
                if (iceStopCount >= 0) {
                    return;
                }

                iceStopCount = GameManager.Instance.planetAttacked_IceStopCountMax;
            } else // end
              {
                iceStopCount = -1;
                SpinAround();
            }
        } else if (type == TypePlanet.Fire) {
            if (start) {
                if (fireLastTimeGotDame == -1) {
                    fireLastTimeGotDame = 0;
                    fireAttackUserPlanet = userPlanetController;
                }
            } else {
                fireLastTimeGotDame = -1;
                fireAttackUserPlanet = null;
            }
        } else if (type == TypePlanet.Antimatter) {
            if (start) {
                if (leanMoveToHoleAntiMatter == null) {
                    leanMoveToHoleAntiMatter = LeanTween.move(gameObject, zone.transform.position, 1)
                        .setEase(LeanTweenType.easeInQuart)
                        .setOnComplete(() => {
                            SetDie();
                            leanMoveToHoleAntiMatter = null;
                        });
                }
            } else // end
              {
                if (leanMoveToHoleAntiMatter != null) {
                    LeanTween.cancel(leanMoveToHoleAntiMatter.uniqueId);
                    leanMoveToHoleAntiMatter = null;
                }
            }
        }
    }

    public DataPlanet DataPlanet {
        get {
            if (Manager.IsEnemy)
                return DataGameSave.dataEnemy.ListPlanet[Manager.IndexSpace];
            else {
                if (DataGameSave.dataBattle != null && DataGameSave.dataBattle.ListPlanet.IsValid())
                    return DataGameSave.dataBattle.ListPlanet[Manager.IndexSpace];
                else
                    return null;
            }
        }
    }

    public int LevelSpeed {
        get {
            if (Manager.IsEnemy)
                return DataGameSave.dataEnemy.ListPlanet[Manager.IndexSpace].LevelSpeed;
            else
                return DataPlanet.LevelSpeed;
        }
        set {
            DataPlanet.LevelSpeed = value;
            speed = DataPlanet.GetSpeed();
            Dame = DataPlanet.GetDame();
            Change();
        }
    }

    public int LevelSize {
        get {
            if (Manager.IsEnemy)
                return DataGameSave.dataEnemy.ListPlanet[Manager.IndexSpace].LevelSize;
            else {
                return DataPlanet.LevelSize;
            }
        }

        set {
            DataPlanet.LevelSize = value;
            DataPlanet.HP += DataMaster.DataMasterModel.HpPerUpgrade;
            size = DataPlanet.GetSize();

            SetSize();
        }
    }

    public int LevelHeavier {
        get {
            if (Manager.IsEnemy)
                return DataGameSave.dataEnemy.ListPlanet[Manager.IndexSpace].LevelHeavier;
            else
                return DataPlanet.LevelHeavier;
        }

        set {
            DataPlanet.LevelHeavier = value;
            DataPlanet.MaterialPerSec = DataPlanet.GetCollectMaterial();

            if (Type == TypePlanet.Gravity) {
                DataPlanet.MaterialPerSec *= DatabasePlanet.Instance.ListGravityPlanet[DataPlanet.LevelElement].UpMater;
            }
        }
    }

    public int LevelElement {
        get {
            if (Manager.IsEnemy)
                return DataGameSave.dataEnemy.ListPlanet[Manager.IndexSpace].LevelElement;
            else
                return DataPlanet.LevelElement;
        }
        set {
            DataPlanet.LevelElement = value;

            if (Type == TypePlanet.Gravity) {
                DataPlanet.MaterialPerSec =
                    DataPlanet.GetCollectMaterial() *
                    DatabasePlanet.Instance.ListGravityPlanet[DataPlanet.LevelElement].UpMater;
            }

            SetSize();
        }
    }

    public GameObject Go {
        get { return go; }
    }

    void Start() {
        RedDot.SetActive(false);
        Change();
        Type = DataPlanet.type;
        startHP = DataPlanet.HP;

        if (Utilities.ActiveScene.name.Equals("MeteorBelt")) {
            if (DTutorial.instance && SteveRogers.Utilities.GetPlayerPrefsBool("mete-tut", true)) {
                DTutorial.instance.Show(this);
                Utilities.SetPlayerPrefsBool("mete-tut", false);
            }
        } else if (Utilities.ActiveScene.name.Equals("GamePlay")) {
            StartCoroutine(SetRestoreTime_CRT());
        }

    }

    private readonly TimeSpan TIMESPAN_24H = new TimeSpan(24, 0, 0);

    private void CheckOutCamInBattle() {
        var dis = 600f;

        if (Math.Abs(transform.position.x) >= dis || Math.Abs(transform.position.y) >= dis) {
            if (Type != TypePlanet.Destroy) {
                AfterHitSun();
            }
        }
    }

    void Update() {
        if (Scenes.IsBattleScene()) {
            rigid.velocity = rigid.velocity.normalized * Speed;
            CheckGotAttackedByFirePlanet();
            CheckOutCamInBattle();
        } else // main scene
          {
            // remove effect after 24h (gravity / air effect)

            if (Type == TypePlanet.Air || Type == TypePlanet.Gravity) {
                if (DataPlanet.addedEffectTick > 0) {
                    if (DateTime.Now - new DateTime(DataPlanet.addedEffectTick) >= TIMESPAN_24H) {
                        DataPlanet.addedEffectTick = 0;
                        SetType(TypePlanet.Default);
                        UpdateEffectOutside(false);
                    }
                }
            }
        }

        if (bloodGo.gameObject.activeSelf) {
            var data = Manager.IsEnemy ? DataGameSave.dataEnemy : DataGameSave.dataServer;

            var scaleratio = transform.localScale.x / 0.6f;
            var scale = Vector3.one / scaleratio;

            if (scale.x > 0 && scale.x <= 1)
                bloodGo.localScale = Vector3.one / scaleratio;

            Utilities.SetPos_Y(bloodGo.gameObject, (size - 10) / 19f, true);

            hpFill.fillAmount = data.ListPlanet[Manager.IndexSpace].HP / startHP;
        } else {
            if (showHPBar)
                bloodGo.gameObject.SetActive(true);
        }

        if (Scenes.Current == SceneName.Gameplay) {
            CheckShowTrail();
        }

        if (Scenes.Current == SceneName.BattlePassGameplay && 
            Vector3.Distance(transform.position, SpaceManager.Instance.transform.position) < GameManager.BATTLE_PASS_PLAYER_HIT_RANGE &&
            owner == Owner.Enemy) {
            HitPlayer();
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (Scenes.Current == SceneName.Battle && !SpaceManager.inited) {
            return;
        }

        if (!gameObject.CompareTag(GameConstants.ENEMY_TAG) && Scenes.Current != SceneName.BattlePassGameplay) // only check on enemy planet
            return;

        if (collision.CompareTag("AttackEffect_FireZone")) // quit fire attack zone
        {
            GotAttacked(TypePlanet.Fire, false, null, null);
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (Scenes.Current == SceneName.Battle && !SpaceManager.inited) {
            return;
        }

        if (collision.gameObject == Manager.gameObject) {
            return;
        }

        GameObject thisPlanet = gameObject;

        if (thisPlanet.tag != GameConstants.ENEMY_TAG &&
            thisPlanet.tag != GameConstants.PLANET_TAG &&
            thisPlanet.tag != GameConstants.SUN_TAG &&
            thisPlanet.tag != GameConstants.BOMBZONE_TAG) // team side (NOT enemy)
        {
            if (collision.gameObject.tag == GameConstants.ENEMY_TAG && owner != Owner.Enemy) {
                if (target == null) {
                    target = collision.gameObject;
                } else {
                    target = Vector2.Distance(transform.position, target.transform.position) < Vector2.Distance(transform.position, collision.transform.position) ? target : collision.gameObject;
                }

                if (collision.gameObject.tag == "AttackEffect_Air") {
                    airInvertEffect = true;
                }
            } else if (collision.gameObject.tag == GameConstants.PLANET_TAG) {
                HitPlanet(collision);
            } else if (collision.gameObject.tag == GameConstants.SUN_TAG) {
                HitSun(collision);
            } else if (collision.gameObject.tag == GameConstants.METEOR_TAG) {
                HitMeteor(collision);
            }
        }
    }


    private void HitPlayer() {

        //SpaceManager.Instance.MainPlanet.GetDamage(this);

        //DataGameSave.dataEnemy.SetAttackedInfoAndSaveEnemyDataToServer(DataGameSave.dataServer);


        // attack effect

        if (Type == TypePlanet.Ice)
        {
            Instantiate(GameManager.Instance.attackEffect_IceZone, transform.position, Quaternion.identity);
            SetType(TypePlanet.Default);
        }
        else if (Type == TypePlanet.Antimatter)
        {
            if (!isCreatedAntiMatterZone)
            {
                isCreatedAntiMatterZone = true;
                Instantiate(GameManager.Instance.attackEffect_AntiMatterZone, transform.position, Quaternion.identity);
            }
        }
        else if (Type == TypePlanet.Fire)
        {
            var go = Instantiate(GameManager.Instance.attackEffect_FireZone, transform.position, Quaternion.identity);
            go.transform.GetChild(0).GetChild(0).GetComponent<IceAttackZone>().owner = this;
            SpaceManager.Instance.MainPlanet.GetDamage(this);
        }
        else
            SpaceManager.Instance.MainPlanet.GetDamage(this);

        // olds

        AfterHitSun();
    }

    public void CheckShowTrail() {
        if (UIMultiScreenCanvasMan.modeExplore == UIMultiScreenCanvasMan.Mode.Gameplay) {
            if (!EffectTrailGameplay.activeSelf) {
                LongTrail.enabled = false;
                StartCoroutine(DelayShowTrail());
            }
        } else {
            if (EffectTrailGameplay.activeSelf) {
                EffectTrailGameplay.SetActive(false);
            }
        }
    }

    IEnumerator DelayShowTrail() {
        EffectTrailGameplay.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        LongTrail.enabled = true;
    }

    private void CheckGotAttackedByFirePlanet() {
        if (fireLastTimeGotDame == -1)
            return;

        fireLastTimeGotDame += UnityEngine.Time.deltaTime;

        if (fireLastTimeGotDame > 1) {
            fireLastTimeGotDame = 0;

            DataGameSave.dataEnemy.ListPlanet[Manager.IndexSpace].HP -= 20;

            if (CheckDie()) {
                GotAttacked(TypePlanet.Fire, false, null, null);
                return;
            }

            fireParticleByGotDameByFireAttack.SetActive(true);
        }
    }

    private FollowHP hpfollow = null;
    public FollowHP hpfollowPrefab = null;

    private void HitMeteor(Collider2D collision) {
        if (!hpfollow) {
            hpfollow = Instantiate<FollowHP>(hpfollowPrefab);
            hpfollow.planet = this;
        }

        DataGameSave.dataLocal.meteorPlanetHitCount++;
        DataGameSave.dataRank.meteor_planet_hit_count++;

        if (Type == TypePlanet.Ice) {
            Instantiate(GameManager.Instance.attackEffect_IceZone, transform.position, Quaternion.identity);
            SetType(TypePlanet.Default);
        } else if (Type == TypePlanet.Antimatter) {
            if (!isCreatedAntiMatterZone) {
                isCreatedAntiMatterZone = true;
                Instantiate(GameManager.Instance.attackEffect_AntiMatterZone, transform.position, Quaternion.identity);
                SetType(TypePlanet.Default);
            }
        } else if (Type == TypePlanet.Fire) {
            var go = Instantiate(GameManager.Instance.attackEffect_FireZone, transform.position, Quaternion.identity);
            go.transform.GetChild(0).GetChild(0).GetComponent<IceAttackZone>().owner = this;
            SetType(TypePlanet.Default);
        }

        var meteor = collision.gameObject.GetComponent<MeteorBeltItem>();

        float planetDame = DataPlanet.GetDame();

        meteor.GetHit(planetDame, EffectDestroy);

        if (meteor.meteorType == MeteorBeltItem.METEOR_TYPE.MULTI_COLOR)
            DataGameSave.dataLocal.meteorMultiColorHitCount++;

        if (meteor.meteorType != MeteorBeltItem.METEOR_TYPE.METEORITE)
            DataGameSave.dataLocal.meteorSpecialPlanetHitCount++;

        if (!GameManager.takeNoDame) {
            DataPlanet.HP -= meteor.dame;
        }

        // If planet not enough dame to break the meteor --> so planet can't go further, planet must be destroyed.
        if (DataPlanet.HP < 0 || planetDame < meteor.hp) {
            MeteorBelt.Instance.ShootMeteorBeltEnd(gameObject);

            DataPlanet.Type = TypePlanet.Destroy;
            PlayfabManager.GetServerTime(
                0,
                (result, index) => {
                    DataPlanet.ShootTime = result;
                });

            var destroyvfx = EffectDestroy[UnityEngine.Random.Range(0, EffectDestroy.Length)];
            LeanPool.Spawn(destroyvfx, transform.position, destroyvfx.transform.rotation);// destroy user planet when user attack

            GameStatics.IsAnimating = false;
            isFlying = false;
            elapsedTimeFly = 0;
            if (IEShoot != null) {
                StopCoroutine(IEShoot);
                IEShoot = null;
            }

            MonoManager.StopCoroutineWithManager("InvertDirWhenAttackAirPlanet_CRT");

            destroyvfx = EffectDestroy[UnityEngine.Random.Range(0, EffectDestroy.Length)];
            LeanPool.Spawn(destroyvfx, transform.position, destroyvfx.transform.rotation);// destroy user planet when user attack


            gameObject.SetActive(false);

            if (DataGameSave.IsAllPlanetDestroyed() && !MeteorBelt.isMeteorFall)
            {
                // show ket qua o day 
                LeanTween.delayedCall(5f, () =>
                {
                    MeteorBelt.Instance.ShowFinalResult();
                });
            }

        }

        Sounds.Instance.PlayCollect();
    }

    public void HitSun(Collider2D collision) {
        //DataGameSave.dataEnemy.SetAttackedInfoAndSaveEnemyDataToServer(DataGameSave.dataServer);

        // attack effect

        if (Type == TypePlanet.Ice) {
            Instantiate(GameManager.Instance.attackEffect_IceZone, transform.position, Quaternion.identity);
            SetType(TypePlanet.Default);
        } else if (Type == TypePlanet.Antimatter) {
            if (!isCreatedAntiMatterZone) {
                isCreatedAntiMatterZone = true;
                Instantiate(GameManager.Instance.attackEffect_AntiMatterZone, transform.position, Quaternion.identity);
            }
        } else if (Type == TypePlanet.Fire) {
            var go = Instantiate(GameManager.Instance.attackEffect_FireZone, transform.position, Quaternion.identity);
            go.transform.GetChild(0).GetChild(0).GetComponent<IceAttackZone>().owner = this;
            collision.gameObject.GetComponent<MainPlanetController>().GetDamage(this);
        } else
            collision.gameObject.GetComponent<MainPlanetController>().GetDamage(this);

        // olds

        AfterHitSun();
    }

    private void AfterHitSun() {
        SetType(TypePlanet.Destroy);

        PlayfabManager.GetServerTime(
            0,
            (result, index) => {
                DataPlanet.ShootTime = result;
            });

        var destroyvfx = EffectDestroy[UnityEngine.Random.Range(0, EffectDestroy.Length)];
        LeanPool.Spawn(destroyvfx, transform.position, destroyvfx.transform.rotation);// destroy user planet when user attack

        gameObject.SetActive(false);
        GameStatics.IsAnimating = false;
        isFlying = false;
        elapsedTimeFly = 0;

        if (IEShoot != null) {
            StopCoroutine(IEShoot);
            IEShoot = null;
        }

        MonoManager.StopCoroutineWithManager("InvertDirWhenAttackAirPlanet_CRT");
    }

    public void HitPlanet(Collider2D collision) // coll này là enemy
    {
        var planetTmp = collision.transform.parent.GetComponent<PlanetController>(); // enemy
        planetTmp.GetCollision(DataPlanet);
        planetTmp.showHPBar = true;
        //DataGameSave.dataEnemy.SetAttackedInfoAndSaveEnemyDataToServer(DataGameSave.dataServer);

        // attack effect 

        if (Type == TypePlanet.Ice) {
            Instantiate(GameManager.Instance.attackEffect_IceZone, transform.position, Quaternion.identity);
            SetType(TypePlanet.Default);
        } else if (Type == TypePlanet.Antimatter) {
            if (!isCreatedAntiMatterZone) {
                isCreatedAntiMatterZone = true;
                Instantiate(GameManager.Instance.attackEffect_AntiMatterZone, transform.position, Quaternion.identity);
                SetType(TypePlanet.Default);
            }
        } else if (Type == TypePlanet.Fire) {
            var go = Instantiate(GameManager.Instance.attackEffect_FireZone, transform.position, Quaternion.identity);
            go.transform.GetChild(0).GetChild(0).GetComponent<IceAttackZone>().owner = this;
            SetType(TypePlanet.Default);
        }

        // old code

        if (Type == TypePlanet.Antimatter) { } else {
            collision.transform.parent.GetComponent<PlanetController>().GetDame(this);
        }

        SetType(TypePlanet.Destroy);

        PlayfabManager.GetServerTime(
           0,
           (result, index) => {
               DataPlanet.ShootTime = result;
           });

        var destroyvfx = EffectDestroy[UnityEngine.Random.Range(0, EffectDestroy.Length)];
        LeanPool.Spawn(destroyvfx, transform.position, destroyvfx.transform.rotation);// destroy user planet when user attack

        PlanetController TempEnemy = collision.transform.parent.gameObject.GetComponent<PlanetController>();

        if (Type != TypePlanet.Air && TempEnemy.Type != TypePlanet.Fire) {
            HPinBattle -= TempEnemy.Dame;
        } else if (Type == TypePlanet.Air && TempEnemy.Type == TypePlanet.Fire) {
            float DameTemp = TempEnemy.Dame *
                (DatabasePlanet.Instance.ListFirePlanet[DataGameSave.dataEnemy.ListPlanet[TempEnemy.Manager.IndexSpace].LevelElement].UpDame -
                DatabasePlanet.Instance.ListAirPlanet[DataPlanet.LevelElement].DownDame);//-1 x2
            HPinBattle -= DameTemp;
        } else if (Type == TypePlanet.Air) {
            float DameTemp = TempEnemy.Dame *
                DatabasePlanet.Instance.ListAirPlanet[DataPlanet.LevelElement].DownDame;//-1
            HPinBattle -= DameTemp;
        } else if (TempEnemy.Type == TypePlanet.Fire) {
            float DameTemp = TempEnemy.Dame *
                DatabasePlanet.Instance.ListFirePlanet[DataGameSave.dataEnemy.ListPlanet[TempEnemy.Manager.IndexSpace].LevelElement].UpDame;//-1
            HPinBattle -= DameTemp;
        }

        if (TempEnemy.Type == TypePlanet.Ice) {
            float SpeedTemp = Speed;

            SlowSpeed = DatabasePlanet.Instance.ListIcePlanet[DataGameSave.dataEnemy.ListPlanet[TempEnemy.Manager.IndexSpace].LevelElement].Slow;//-1
            FxFreeze.SetActive(true);
            Speed *= SlowSpeed;

            if (SlowCoroutine != null) {
                StopCoroutine(SlowCoroutine);
            }

            SlowCoroutine = StartCoroutine(MonoHelper.DoSomeThing(DatabasePlanet.Instance.ListIcePlanet[DataGameSave.dataEnemy.ListPlanet[TempEnemy.Manager.IndexSpace].LevelElement].TimeSlow, () =>//-1
            {
                SlowSpeed = 1;
                Speed = Speed * SlowSpeed;
                FxFreeze.SetActive(false);
            }));
        }

        if (HPinBattle <= 0 || Type == TypePlanet.Fire) // destroy the this planet (fire) for first hit
        {
            gameObject.SetActive(false);

            GameStatics.IsAnimating = false;
            isFlying = false;
            elapsedTimeFly = 0;
            if (IEShoot != null) {
                StopCoroutine(IEShoot);
                IEShoot = null;
            }
            MonoManager.StopCoroutineWithManager("InvertDirWhenAttackAirPlanet_CRT");
        }

        planetCollider.enabled = false;
        planetCollider.enabled = true;
    }

    void OnMouseDown() {
        if (!gameObject || gameObject.tag == GameConstants.ENEMY_TAG || gameObject.tag == GameConstants.PLANET_TAG || Popups.IsShowed || isFlying)
            return;

        if (Type == TypePlanet.Destroy) {
            SpaceManager.Instance.PlanetSelect = this;
            return;
        }

        SpaceManager.Instance.SetFxBack(gameObject);

        if (Scenes.Current == SceneName.Gameplay) {
            if (Popups.IsShowed)
                return;

            Sounds.Instance.PlaySelect();

            if (ShowTrail != null) {
                StopCoroutine(ShowTrail);
            }

            EffectTrailGameplay.SetActive(false);
            SpaceManager.Instance.PlanetSelect = this;

            _IsClick = true;
            LeanTween.cancel(gameObject);

            currentDown = Input.mousePosition;
            currentDown = Camera.main.ScreenToWorldPoint(currentDown);

            transform.localScale = Vector3.one * 1.5f * ScaleSize;
            Manager.IsHavePlanet = false;
            oldIndex = Manager.IndexSpace;
        }

        StartDrag();
    }

    void OnMouseDrag() {
        if (gameObject.tag == GameConstants.ENEMY_TAG || gameObject.tag == GameConstants.PLANET_TAG || Popups.IsShowed || isFlying)
            return;
        if (Type == TypePlanet.Destroy) {
            return;
        }
        if (Scenes.Current == SceneName.Gameplay) {
            if (!_IsClick) return;
            if (Popups.IsShowed) return;

            mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            if (!IsDrag) {

                float _Distance = Vector3.Distance(mousePosition, currentDown);
                if (_Distance < .1f) {
                    _isTap = true;
                    return;
                }

            }

            IsDrag = true;

            _isTap = false;

            transform.position = Vector3.Lerp(new Vector3(transform.position.x, transform.position.y),
                        new Vector3(mousePosition.x, mousePosition.y),
                       10);

            float check = Vector3.Distance(Vector3.zero, transform.position);
            IsActive = SpaceManager.Instance.CheckPosition(check);
        } else if (Scenes.Current == SceneName.Battle) {
            UpdatePath();
        } else if (Scenes.Current == SceneName.MeteorBelt) {
            UpdatePath();
        } else if (Scenes.Current == SceneName.BattlePassGameplay) {
            UpdatePath();
        }
    }

    public long restoreTime = -1;
    public static Button skipButtonRestore = null;

    public IEnumerator SetRestoreTime_CRT() {
        while (true) {
            //if (!Utilities.ActiveScene.name.Equals("GamePlay"))
            //    yield return null;

            yield return new WaitForSecondsRealtime(1f);

            if (!SpaceManager.Instance.restoreTimeItems)
                continue;

            restoreTime = (long)(GameManager.Now - DataPlanet.ShootTime).TotalSeconds;
            restoreTime = ReadDataRestorePlanet.Instance.ListDataRestore[Manager.IndexSpace].Time - restoreTime;

            if (restoreTime < 0) {
                SpaceManager.Instance.restoreTimeItems.GetChild(Manager.IndexSpace).gameObject.CheckAndSetActive(false);
                bool shouldInactiveAll = false;

                foreach (Transform t in SpaceManager.Instance.restoreTimeItems) {
                    if (t.gameObject.activeSelf) {
                        shouldInactiveAll = true;
                        break;
                    }
                }

                if (UIMultiScreenCanvasMan.modeExplore == UIMultiScreenCanvasMan.Mode.Gameplay) {
                    SpaceManager.Instance.restoreTimePanelGo.CheckAndSetActive(shouldInactiveAll);
                } else {
                    SpaceManager.Instance.restoreTimePanelGo.CheckAndSetActive(false);
                }

                continue;
            }

            TutGameplayScene.TutFocusRestore();

            if (UIMultiScreenCanvasMan.modeExplore == UIMultiScreenCanvasMan.Mode.Gameplay) {
                SpaceManager.Instance.restoreTimePanelGo.CheckAndSetActive(true);
            } else {
                SpaceManager.Instance.restoreTimePanelGo.CheckAndSetActive(false);
            }

            var item = SpaceManager.Instance.restoreTimeItems.GetChild(Manager.IndexSpace);
            item.gameObject.CheckAndSetActive(true);

            Transform timeObj = item.GetChild(1);
            timeObj.GetComponent<Text>().text = Utilities.GetTimeSpanString(new TimeSpan(0, 0, (int)restoreTime), showMinutes: true);

            Button skipButton = item.GetChild(2).gameObject.GetComponent<Button>();
            skipButton.transform.GetChild(2).GetComponent<Text>().text = (restoreTime / 60 + 1).ToString();

            if (GameManager.IsAutoRestorePlanet)
                skipButton.onClick.Invoke();

            skipButtonRestore = skipButton;
        }
    }

    void OnMouseUp() {
        if (gameObject.tag == GameConstants.ENEMY_TAG || gameObject.tag == GameConstants.PLANET_TAG || Popups.IsShowed || isFlying)
            return;

        if (Type == TypePlanet.Destroy) {
            if (GameStatics.IsAnimating || isActive) return;

            if (ReturnPlanet != null)
                StopCoroutine(ReturnPlanet);

            return;
        }

        if (Scenes.Current == SceneName.Gameplay) {
            if (!_IsClick)
                return;
            else
                _IsClick = false;

            if (Popups.IsShowed)
                return;

            if (Scenes.Current == SceneName.Gameplay) {
                if (ShowTrail != null) {
                    StopCoroutine(ShowTrail);
                }

                ShowTrail = StartCoroutine(MonoHelper.DoSomeThing(2, () => {
                    EffectTrailGameplay.SetActive(true);
                }));
            }

            IsDrag = false;
            Manager.IsHavePlanet = true;

            if (Type != TypePlanet.Destroy) {
                if ((!Popups.IsShowed)) {
                    if (_isTap) {
                        SpaceManager.Instance.PlanetSelect = this;
                    }
                }
            }
            if (IsActive) {
                mousePosition = Input.mousePosition;
                mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

                int IndexTemp = SpaceManager.Instance.CheckPositionToChange(this, transform.position, Index);

                if (IndexTemp == Index) {
                } else {
                    int _Id = Manager.IndexSpace;
                    Index = IndexTemp;
                    transform.SetParent(Manager.transform);
                    Orbit = Manager.Orbit.Line;
                    SpaceManager.Instance.FxBack.SetActive(false);

                    DataPlanet DataTemp = DataGameSave.dataServer.ListPlanet[oldIndex];
                    DataGameSave.dataServer.ListPlanet[oldIndex] = DataGameSave.dataServer.ListPlanet[_Id];
                    DataGameSave.dataServer.ListPlanet[_Id] = DataTemp;
                    DataGameSave.SaveToServer();
                }

                Manager.Planet = this;

                transform.position = (Orbit.GetPosition(Index));
            } else {
                transform.position = (Orbit.GetPosition(Index));

                if (!_isTap)
                    SpaceManager.Instance.FxBack.SetActive(false);

                LeanTween.cancel(gameObject);
            }
            IsActive = false;

            transform.localScale = Vector3.one * ScaleSize;
            LeanTween.cancel(gameObject);
            SpinAround();

            SpaceManager.Instance.TurnOffLine();
        }
    }

    public void GetCollision(DataPlanet Data) {
        switch (Data.Type) {
            case TypePlanet.Ice: {
                    SlowSpeed = 1;
                    FxFreeze.SetActive(true);
                    Change();

                    if (SlowCoroutine != null) {
                        StopCoroutine(SlowCoroutine);
                    }

                    SlowCoroutine = StartCoroutine(MonoHelper.DoSomeThing(1, () => {
                        SlowSpeed = 1;
                        FxFreeze.SetActive(false);
                        Change();
                    }));
                    break;
                }
        }
    }

    public void GetDame(PlanetController PlanetEnemy) // PlanetEnemy = user
    {
        float DameTemp = 0f;

        if (Type == TypePlanet.Antimatter) {
            return;
        } else if (Type != TypePlanet.Air && PlanetEnemy.Type != TypePlanet.Ice && PlanetEnemy.Type != TypePlanet.Fire)// if PlanetEnemy (user) is ice /fire then not need to destroy (attack effect)
          {
            DataGameSave.dataEnemy.ListPlanet[Manager.IndexSpace].HP -= PlanetEnemy.Dame;
        } else if (Type == TypePlanet.Air && PlanetEnemy.Type == TypePlanet.Fire) {
            DameTemp = PlanetEnemy.Dame *
                (DatabasePlanet.Instance.ListFirePlanet[DataGameSave.dataServer.ListPlanet[PlanetEnemy.Manager.IndexSpace].LevelElement].UpDame -
                DatabasePlanet.Instance.ListAirPlanet[DataGameSave.dataEnemy.ListPlanet[Manager.IndexSpace].LevelElement].DownDame);//-1 x2
            DataGameSave.dataEnemy.ListPlanet[Manager.IndexSpace].HP -= DameTemp;
        } else if (Type == TypePlanet.Air) {
            DameTemp = PlanetEnemy.Dame *
                DatabasePlanet.Instance.ListAirPlanet[DataGameSave.dataEnemy.ListPlanet[Manager.IndexSpace].LevelElement].DownDame;//-1
            DataGameSave.dataEnemy.ListPlanet[Manager.IndexSpace].HP -= DameTemp;
        }

        CheckDie(PlanetEnemy);
    }

    private void SetDie(PlanetController userPlanet = null) {
        if (userPlanet)
            userPlanet.target = null;

        // Set data
        DataGameSave.dataEnemy.ListPlanet[Manager.IndexSpace].Type = TypePlanet.Destroy;

        PlayfabManager.GetServerTime(
            0,
            (result, index) => {
                DataGameSave.dataEnemy.ListPlanet[Manager.IndexSpace].ShootTime = result;
            });

        CaculateReward();

        // Achievement
        DataGameSave.dataLocal.DestroyPlanet++;
        DataGameSave.dataRank.destroy_planet++;

        // Daily mission
        DataGameSave.dataLocal.dailyMissions[(int)DailyQuests.DestroyPlanet].currentProgress++;

        if (BtnDailyMission.Instance) {
            BtnDailyMission.Instance.CheckDoneQuest();
        }

        // Turn off renderer
        gameObject.SetActive(false);

        // Disable collider
        planetCollider.enabled = false;

        if (go && go.GetComponent<CircleCollider2D>())
            go.GetComponent<CircleCollider2D>().enabled = false;
    }

    private bool CheckDie(PlanetController userPlanet = null) {
        if (DataGameSave.dataEnemy.ListPlanet[Manager.IndexSpace].HP > 0)
            return false;

        SetDie(userPlanet);
        return true;
    }

    public void CaculateReward() {
        // Collect Reward
        var rand = UnityEngine.Random.value;
        var bonus = 0;
        int levelx3 = (int)Mathf.Pow(LevelElement, 3);
        RewardMaterialFX reward = null;
        RewardMaterialFX rewardMat = null;
        int value = 0;
        if (Type != TypePlanet.Destroy) {
            reward = LeanPool.Spawn(RewardMaterialFX.CreateReward());
            RewardMaterialFX.AddPool(reward);
            rewardMat = LeanPool.Spawn(RewardMaterialFX.CreateReward());
            RewardMaterialFX.AddPool(rewardMat);
        }
        switch (Type) {
            case TypePlanet.Antimatter:
                value = LevelSize * 30 + LevelSpeed * 10 + LevelHeavier * 50 + 10;
                rewardMat.Init(5, value, transform.position + Vector3.left);
                if (value == 0) {
                    LeanPool.Despawn(rewardMat);
                }
                GameManager.reward.material += LevelSize * 30 + LevelSpeed * 10 + LevelHeavier * 50 + 10;
                if (rand <= (float)((levelx3 % 100) + 2) / 100)
                    bonus = 1;
                GameManager.reward.antimater += (levelx3 / 100) + bonus;
                value = (levelx3 / 100) + bonus;
                reward.Init((int)Type, value, transform.position + Vector3.right);
                if (value == 0) {
                    LeanPool.Despawn(reward);
                }
                break;
            case TypePlanet.Gravity:
                value = LevelSize * 30 + LevelSpeed * 10 + LevelHeavier * 50 + 10;
                rewardMat.Init(5, value, transform.position + Vector3.left);
                if (value == 0) {
                    LeanPool.Despawn(rewardMat);
                }
                GameManager.reward.material += LevelSize * 30 + LevelSpeed * 10 + LevelHeavier * 50 + 10;
                if (rand <= (float)((levelx3 % 100) + 10) / 100)
                    bonus = 1;
                GameManager.reward.gravity += (levelx3 / 100) + bonus;
                value = (levelx3 / 100) + bonus;
                reward.Init((int)Type, value, transform.position + Vector3.right);
                if (value == 0) {
                    LeanPool.Despawn(reward);
                }
                break;
            case TypePlanet.Ice:
                value = LevelSize * 15 + LevelSpeed * 10 + LevelHeavier * 20 + 10;
                rewardMat.Init(5, value, transform.position + Vector3.left);
                if (value == 0) {
                    LeanPool.Despawn(rewardMat);
                }
                GameManager.reward.material += LevelSize * 15 + LevelSpeed * 10 + LevelHeavier * 20 + 10;
                if (rand <= (float)((levelx3 % 100) + 10) / 100)
                    bonus = 1;
                value = (levelx3 / 100) + bonus;
                GameManager.reward.ice += (levelx3 / 100) + bonus;
                value = (levelx3 / 100) + bonus;
                reward.Init((int)Type, value, transform.position + Vector3.right);
                if (value == 0) {
                    LeanPool.Despawn(reward);
                }
                break;
            case TypePlanet.Fire:
                value = LevelSize * 15 + LevelSpeed * 10 + LevelHeavier * 20 + 10;
                rewardMat.Init(5, value, transform.position + Vector3.left);
                if (value == 0) {
                    LeanPool.Despawn(rewardMat);
                }
                GameManager.reward.material += LevelSize * 15 + LevelSpeed * 10 + LevelHeavier * 20 + 10;
                if (rand <= (float)((levelx3 % 100) + 10) / 100)
                    bonus = 1;
                value = (levelx3 / 100) + bonus;
                GameManager.reward.fire += (levelx3 / 100) + bonus;
                value = (levelx3 / 100) + bonus;
                reward.Init((int)Type, value, transform.position + Vector3.right);
                if (value == 0) {
                    LeanPool.Despawn(reward);
                }
                break;
            case TypePlanet.Air:
                value = LevelSize * 30 + LevelSpeed * 10 + LevelHeavier * 50 + 10;
                rewardMat.Init(5, value, transform.position + Vector3.left);
                if (value == 0) {
                    LeanPool.Despawn(rewardMat);
                }
                GameManager.reward.material += LevelSize * 30 + LevelSpeed * 10 + LevelHeavier * 50 + 10;
                if (rand <= (float)((levelx3 % 100) + 10) / 100)
                    bonus = 1;
                GameManager.reward.air += (levelx3 / 100) + bonus;
                value = (levelx3 / 100) + bonus;
                reward.Init((int)Type, value, transform.position + Vector3.right);
                if (value == 0) {
                    LeanPool.Despawn(reward);
                }
                break;
            case TypePlanet.Default:
                value = LevelSize * 15 + LevelSpeed * 10 + LevelHeavier * 20 + 10;
                rewardMat.Init(5, value, transform.position + Vector3.left);
                if (value == 0) {
                    LeanPool.Despawn(rewardMat);
                }
                LeanPool.Despawn(reward);
                GameManager.reward.material += LevelSize * 15 + LevelSpeed * 10 + LevelHeavier * 20 + 10;
                break;
        }
    }

    public void Change() {
        Time = (float)((float)(1430 - 1.2 * speed) / SlowSpeed) / 20000;
    }

    public void SetSize() {
        ScaleSize = 0.06f * size;

        if (Type != TypePlanet.Destroy && LongTrail != null) {
            LongTrail.startWidth = 1.5f * ScaleSize;
            LongTrail.gameObject.SetActive(true);
        }

        if (effectTrail != null)    //Minh.ho setting effect scale base on size 
        {
            var shapeEffectTrail = effectTrail.shape;
            shapeEffectTrail.radius = 0.6f * ScaleSize;

            var emissionEffectTrail = effectTrail.emission;
            emissionEffectTrail.rateOverTime = 30 * ScaleSize;
        }

        try {
            if (transform)
                transform.localScale = Vector3.one * ScaleSize;
        } catch { }
    }

    public void SpawnPos(int IndexPos, LineRenderer _Orbit, SpaceController _Manager, bool Default, bool isEnemy = false) {
        Manager = _Manager;
        Orbit = _Orbit;

        Index = IndexPos;
        transform.position = (Orbit.GetPosition(Index));
        SpinAround();

        //Thêm data
        if (!Default) {
            LeanTween.cancel(gameObject);
            DataGameSave.dataServer.ListPlanet.Add(new DataPlanet());
            if (isEnemy) {
                SetData(DataGameSave.dataEnemy.ListPlanet[Manager.IndexSpace], _Manager.IndexSpace, isEnemy);
            } else
                SetData(DataPlanet, _Manager.IndexSpace, isEnemy);
            DataGameSave.SaveToServer();
        }

        if (isEnemy) {
            //this is magic
            gameObject.tag = GameConstants.ENEMY_TAG;

            //this is super magic
            GetComponent<CircleCollider2D>().isTrigger = true;
            go = new GameObject();
            go.transform.SetParent(transform);
            go.AddComponent<CircleCollider2D>().radius = 1f;
            go.AddComponent<Rigidbody2D>().gravityScale = 0;
            go.tag = GameConstants.PLANET_TAG;
            go.layer = LayerMask.NameToLayer("Enemy");
            go.GetComponent<CircleCollider2D>().isTrigger = true;

            go.transform.localPosition = Vector3.zero;

            {
                var radius = 10;// go.AddComponent<CircleCollider2D>().radius;            
                radiusGo.transform.localScale = new Vector3(radius, radius, radius);
                radiusGo.SetActive(true);
            }
        }

        if (isEnemy)
            Type = DataGameSave.dataEnemy.ListPlanet[Manager.IndexSpace].Type;
        else
            Type = DataPlanet.Type;

    }

    private void SetType(TypePlanet type) {
        if (Manager.IsEnemy)
            Type = DataGameSave.dataEnemy.ListPlanet[Manager.IndexSpace].Type = type;
        else
            Type = DataPlanet.Type = type;

        //DataGameSave.SaveToServer();
    }

    public void SpinAround() {
        try {
            // attacked by ice slow down then stop. attacked by antimatter then stop moving
            if (iceStopCount == 0 || leanMoveToHoleAntiMatter != null) {
                return;
            } else if (iceStopCount > 0) {
                iceStopCount--;
            }

            LeanTween.cancel(moveId);

            if (!gameObject)
                return;

            if ((PlayScenesManager.Instance && PlayScenesManager.Instance.pausingSpins) ||
                HomeBottomUI.Instance && HomeBottomUI.Instance.pausingSpins && !Manager.IsEnemy ||
                MeteorBelt.Instance && MeteorBelt.Instance.pausing)
                return;

            moveId = LeanTween.moveLocal(gameObject, Orbit.GetPosition(Index), Time / GameManager.planetSpeedRatio).setOnComplete(() => {
                try {
                    bool clockwise = false;

                    if (Utilities.IsOutOfRange(DataGameSave.dataServer.ListPlanet, Manager.IndexSpace))
                        clockwise = true;
                    else
                        clockwise = DataPlanet.ClockWise;

                    if (clockwise) {
                        if (Index < Orbit.positionCount - 2)
                            Index++;
                        else
                            Index = 0;
                    } else // ImClockWise
                      {
                        if (Index > 0)
                            Index--;
                        else
                            Index = Orbit.positionCount - 2;
                    }

                    SpinAround();
                } catch { }
            }).id;
        } catch { }
    }

    private void RestoreSpinAroundOtherPlanetsWhenTapThis() {
        if (!ShootPath.Instance.GetActivePath())
            return;

        if (Scenes.Current != SceneName.Battle)
            return;

        var count = SpaceManager.Instance.ListSpace.Count;

        for (int i = 0; i < count; i++) {
            var planet = SpaceManager.Instance.ListSpace[i].Planet;

            if (planet != this && planet.IEShoot != null && !planet.isFlying) {
                planet.SpinAround();
                planet.IEShoot = null;
            }
        }
    }

    public void StartDrag(bool updateByPointPull = true) {
        if (Scenes.Current == SceneName.Battle ||
            Scenes.Current == SceneName.MeteorBelt ||
            Scenes.Current == SceneName.BattlePassGameplay) {

            if (!HomeBottomUI.isAutoShooting && owner == Owner.Player)
                RestoreSpinAroundOtherPlanetsWhenTapThis();

            if (IEShoot == null)
                IEShoot = Shoot();

            HomeBottomUI.Instance.ShootBtn.GetComponent<Button>().interactable = true;
            SpaceManager.Instance.SetFxBack(gameObject);

            startDragPosition = new Vector3(transform.position.x, transform.position.y, Camera.main.ScreenToWorldPoint(Input.mousePosition).z);
            endDragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (updateByPointPull) {
                PointPull.Instance.transform.position = transform.position;
                PointPull.SetActive(this);

                if (ShootPath.Instance)
                {
                    ShootPath.Instance.SetActivePath(true);
                }
            }

            UpdatePath(updateByPointPull);

            if (Type == TypePlanet.Antimatter) {
                Zone.gameObject.SetActive(true);
                Zone.SetZone(DataPlanet.LevelElement, ScaleSize);
            }

            LeanTween.cancel(gameObject);
        }
    }

    public void UpdatePath(bool updateByPointPull = true) {
        if (Popups.IsShowed) return;

        endDragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 direction = endDragPosition - startDragPosition;
        direction.Normalize();

        if (SceneManager.GetActiveScene().name != "MeteorBelt") {
            if (Utilities.GetPlayerPrefsBool("firstEnterBattle", true) || HomeBottomUI.isAutoShooting) {
                direction = transform.position - SpaceEnemyManager.Instance.MainPlanet.transform.position;
                direction.Normalize();
            }
        }

        transform.localEulerAngles = new Vector3(0, 0, -Vector2.Angle(direction, Vector2.right));
        Speed = 0.001f * DataPlanet.GetSpeed();
        dir = -direction * Speed;

        if (ShootPath.Instance && owner != Owner.Enemy) {
            ShootPath.Instance.DrawPath(transform.position, SpaceEnemyManager.EnemyPosition, -direction, SpaceEnemyManager.GravityRadius, Speed, SpaceManager.Instance.grav / 100f);
        }

        if (updateByPointPull) {
            PointPull.Instance.transform.position = new Vector3(endDragPosition.x, endDragPosition.y, transform.position.z);
        }
    }

    private IEnumerator InvertDirWhenAttackAirPlanet_CRT(Vector3 normal_dir) {
        for (; ; )
        {
            yield return null;
        }
    }

    private IEnumerator Shoot() {
        HPinBattle = DataPlanet.HP;
        SpaceManager.Instance.FxBack.SetActive(false);
        //GameStatics.IsAnimating = true;
        isFlying = true;
        if (Scenes.IsBattleScene()) {
            planetCollider.radius /= 2; //Minh.ho: tang gap doi vung cham khi vao battle cho user de cham, sau khi user cham roi thi giam lai vung cham nhu ban dau
        }
        if (SceneManager.GetActiveScene().name == "MeteorBelt") {
            MeteorBelt.Instance.ShootMeteorBeltStart(gameObject);
            MeteorBelt.Instance.shootingIndex = Manager.IndexSpace;
        }

        Vector2 targetPosition = Vector2.zero;

        if (owner == Owner.Player) {
            targetPosition = SpaceEnemyManager.EnemyPosition;

        } else if (owner == Owner.Enemy) {
            targetPosition = SpaceManager.Instance.transform.position + new Vector3(UnityEngine.Random.Range(-GameManager.battlePassAttackMissRange, GameManager.battlePassAttackMissRange),GameManager.BATTLE_PASS_ATTACK_Y_RANGE);
        }

        while (true) {
            elapsedTimeFly += UnityEngine.Time.deltaTime;

            float distanceToEnemy = Vector2.Distance(SpaceEnemyManager.EnemyPosition, transform.position);

            bool curveEnemySun = true;
            if (Scenes.Current == SceneName.MeteorBelt || owner == Owner.Enemy) {
                curveEnemySun = false;
            }

            // curve to enemy's sun
            if (curveEnemySun && distanceToEnemy < SpaceEnemyManager.GravityRadius && Scenes.Current != SceneName.MeteorBelt) {
                Vector3 pos = MonoHelper.GetCurve(
                    targetPosition,
                    transform.position,
                    dir,
                    dirpower: Vector2.Distance(SpaceEnemyManager.EnemyPosition, transform.position),
                    maxpower: SpaceEnemyManager.GravityRadius,
                    powermove: Speed,
                    SpaceManager.Instance.grav / 200f);

                dir = pos - transform.position;
            }

            //check remove target
            if (target != null && owner != Owner.Enemy) {

                PlanetController planet = target.GetComponent<PlanetController>();

                if (planet != null) {
                    int index = planet.Manager.IndexSpace;
                    if (DataGameSave.dataEnemy.ListPlanet[index].HP <= 0) {
                        //Debug.Log("Remove target!");
                        target = null;
                    }
                }
            }

            // curve to target, either sun or planet
            if (target != null && owner != Owner.Enemy)
            {
                Vector3 pos = MonoHelper.GetCurve(
                        target.transform.position,
                        transform.position,
                        dir,
                        dirpower: Vector2.Distance(target.transform.position, transform.position),
                        maxpower: target.GetComponent<CircleCollider2D>().radius,
                        powermove: Speed,
                        target.GetComponent<PlanetController>().Gravitation);

                dir = pos - transform.position;
            }

            if (SceneManager.GetActiveScene().name == "MeteorBelt") {
                Vector3 meteorGravity = new Vector3(0, 0);
                float minDistance = 9999f;
                Transform nearestMeteor = MeteorBelt.listMeteors[0].transform;

                int levelPlanet = (int)DataPlanet.Name + 1;
                float myGravityRange = MY_GRAVITY_RANGE_START + (levelPlanet - MY_GRAVITY_MIN_LEVEL) * MY_GRAVITY_RANGE_INCREASE_BY_LEVEL;
                float myGravityForce = MY_GRAVITY_FORCE_START + (levelPlanet - MY_GRAVITY_MIN_LEVEL) * MY_GRAVITY_FORCE_INCREASE;

                // find the nearest meteor 
                for (int i = 0; i < MeteorBelt.listMeteors.Count; i++) {
                    Transform meteor = MeteorBelt.listMeteors[i].transform;
                    if (meteor.gameObject.activeSelf == false)
                        continue;

                    float distanceToMeteor = Vector2.Distance(meteor.position, transform.position);

                    if (minDistance > distanceToMeteor) {
                        minDistance = distanceToMeteor;
                        nearestMeteor = meteor;
                    }

                    //apply gravity
                    if (distanceToMeteor < myGravityRange && levelPlanet >= MY_GRAVITY_MIN_LEVEL) {
                        Vector3 myGravity = transform.position - meteor.position;
                        myGravity = myGravity / Vector3.Distance(transform.position, meteor.position);
                        meteor.GetComponent<Rigidbody2D>().AddForce(myGravity * myGravityForce);
                    }
                }

                // if planet in this meteor gravity range, then do the gravity
                if (minDistance < nearestMeteor.GetComponent<MeteorBeltItem>().gravityRange) {
                    Vector3 pos = MonoHelper.GetCurve(nearestMeteor.position,
                    transform.position, dir, Vector2.Distance(nearestMeteor.position, transform.position),
                    nearestMeteor.GetComponent<MeteorBeltItem>().gravityRange, Speed, SpaceManager.Instance.grav / 450f);

                    meteorGravity = pos;
                }

                if (meteorGravity != Vector3.zero) {
                    dir = meteorGravity - transform.position;
                }
            }

            if (airInvertEffect)
            {
                invertedDirWhenAttackAirPlanet_State = -1;

                if (IEShoot != null)
                {
                    StopCoroutine(IEShoot);
                    IEShoot = null;
                }

                MonoManager.StartCoroutineWithManager(InvertDirWhenAttackAirPlanet_CRT(dir), "InvertDirWhenAttackAirPlanet_CRT", this);
                yield break;
            }
            else if (attackDirect)
            {
                Vector3 vectorToPlayer = transform.position - (Vector3)targetPosition;

                transform.position -= vectorToPlayer.normalized * UnityEngine.Time.deltaTime * BOSS_DIRECT_ATTACK_FORCE;
            }
            else {
                transform.position += dir;
            }

            yield return new WaitForEndOfFrame();

            if (IEShoot == null) {
                GameStatics.IsAnimating = false;
                isFlying = false;
                break;
            } else if (elapsedTimeFly > 20 && SceneManager.GetActiveScene().name != "MeteorBelt") {
                DataPlanet.Type = TypePlanet.Destroy;

                if (SceneManager.GetActiveScene().name == "Battle" || SceneManager.GetActiveScene().name == "MeteorBelt") {
                    PlayfabManager.GetServerTime(0, (result, index) => {
                        try {
                            DataGameSave.dataEnemy.ListPlanet[Manager.IndexSpace].ShootTime = result;
                        } catch { }
                    });
                }

                gameObject.SetActive(false);
                elapsedTimeFly = 0;
                isFlying = false;
                if (IEShoot != null) {
                    StopCoroutine(IEShoot);
                    IEShoot = null;
                }
                GameStatics.IsAnimating = false;

                break;
            }
        }
    }

    public void OnNewPlanet() {
        spriteAnim.Set(DataPlanet.skin.ToString());
    }

    public void UpdateSkin() {
        spriteAnim.Set(DataPlanet.skin.ToString());
    }

    public void SetData(DataPlanet _Data, int _OrbitIndex, bool isEnemy = false) {

        iceStopCount = -1;

        if (Scenes.Current == SceneName.Battle && !isEnemy) {
            planetCollider.radius *= 2;  //Minh.ho: tang vung cham cho user de cham
        }

        // olds

        if (spriteAnim) {
            spriteAnim.Set(_Data.skin.ToString());
        }

        if (_Data.Type == TypePlanet.Destroy) {
            LongTrail.startWidth = 0;
            LongTrail.gameObject.SetActive(false);

            _Data.SetDataToDefault(_Data.ShootTime);
            size = _Data.GetSize();
            speed = _Data.GetSpeed();
            Dame = _Data.GetDame();

            SetSize();

            if (Scenes.Current == SceneName.Gameplay)
                WaitFx.SetActive(true);

            if (spriteAnim)
                spriteAnim.spriteRenderer.enabled = false;

            planetCollider.enabled = false;
            if (isEnemy)
                go.GetComponent<CircleCollider2D>().enabled = false;

            if (collectCorou != null && Scenes.Current == SceneName.Gameplay) {
                StopCoroutine(collectCorou);
                collectCorou = null;
            }

            ReturnPlanet = StartCoroutine(RestorePlanet(_OrbitIndex, isEnemy));
        } else {

            isActive = true;

            try {
                LongTrail.colorGradient = TrailEffects[(int)(_Data.Type)];
            } catch { }
            
            var delayTime = 2.5f;
            if (Scenes.Current == SceneName.Battle) {
                delayTime = 0;
            } else {
                var Fx = LeanPool.Spawn(Manager.prefabTVHT);
                Fx.transform.position = transform.position;
            }

            LeanTween.delayedCall(delayTime,
                () => {
                    Type = _Data.Type;

                    size = _Data.GetSize();
                    speed = _Data.GetSpeed();
                    Dame = _Data.GetDame();

                    SetSize();
                    SpinAround();
                    Change();


                    if (collectCorou == null && Scenes.Current == SceneName.Gameplay) {
                        try {
                            collectCorou = StartCoroutine(CollectData());
                        } catch {
                            collectCorou = null;
                        }
                    }

                    if (spriteAnim)
                        spriteAnim.spriteRenderer.enabled = true;

                    if (Scenes.Current == SceneName.Gameplay) {
                        CheckCanUpgrade();
                    }

                    UpdateEffectOutside();
                });
        }
    }

    public void UpgradeChange(TypePlanet _Type) {
        if (_Type == TypePlanet.Antimatter) {
            int _level = DataPlanet.LevelElement /= 2;

            if (_level == 0) {
                LevelElement = 1;
                DataPlanet.LevelElement = 1;
            } else {
                LevelElement = _level;
                DataPlanet.LevelElement = _level;
            }
        } else if (Type == TypePlanet.Antimatter) {

            int _level = DataPlanet.LevelElement *= 2;
            LevelElement = _level;
            DataPlanet.LevelElement = _level;
        } else if (Type == TypePlanet.Default) {
            LevelElement = 1;
            DataPlanet.LevelElement = 1;
        }

        if (Type == TypePlanet.Gravity) {
            DataPlanet.MaterialPerSec = DataPlanet.GetCollectMaterial();
        }

        if (_Type == TypePlanet.Gravity) {
            DataPlanet.MaterialPerSec =
                DataPlanet.GetCollectMaterial() *
                DatabasePlanet.Instance.ListGravityPlanet[DataPlanet.LevelElement].UpMater;
        }

        Type = _Type;

        if (spriteAnim)
            spriteAnim.Set(DataPlanet.skin.ToString());

        LongTrail.colorGradient = TrailEffects[(int)_Type];

        DataPlanet.Type = Type;
        SetSize();
        EazySoundManager.PlaySound(Sounds.Instance.Upgrade);
    }

    IEnumerator CollectData() {
        float MoneyTemp = 0;
        MoneyCollect = 0;
        DateTime now = DateTime.Now;

        PlayfabManager.GetServerTime(
            0,
            (result, index) => {
                now = result;
                DateTime collectTime = DataPlanet.CollectTime;

                MoneyCollect = (int)DataPlanet.GetStartMoney(now, collectTime);

                if (MoneyCollect == 0) {
                    TxtCollect.text = "";
                } else {
                    TxtCollect.text = (MoneyCollect * DataGameSave.dataLocal.itemX2.multiplyNumber).ToString() + TextConstants.M_Mater;
                }

                TimeMaxCollect = GameConstants.MAX_TIME_COLLECT; //43200;
            });

        yield return new WaitForSecondsRealtime(.5f);

        while (true) {
            var timeCoollectTemp = Type == TypePlanet.Gravity ? 1 : 5;

            yield return new WaitForSecondsRealtime(timeCoollectTemp);
            TimeMaxCollect -= timeCoollectTemp;

            if (TimeMaxCollect <= 0) {
                collectCorou = null;
                break;
            }
            MoneyCollect += (int)DataPlanet.MaterialPerSec *
                DataGameSave.dataLocal.itemX2.multiplyNumber;

            if (MoneyCollect == 0) {
                TxtCollect.text = "";
            } else {
                TxtCollect.text = (MoneyCollect).ToString() + TextConstants.M_Mater;
            }
            if (PopupSelectPlanet.IsShowed && SpaceManager.Instance.PlanetSelect == this) {
                PopupSelectPlanet.UpdateMoney(MoneyCollect);

                if (PopupUpgradePlanet._Instance)
                    PopupUpgradePlanet._Instance.UpdateMoney(MoneyCollect);
            }

        }
    }

    public void StopToChange(int _newIndex) {
        LeanTween.cancel(gameObject);

        Index = _newIndex;

        //Data.IndexOrbit = Manager.IndexSpace;


        Orbit = Manager.Orbit.Line;

        transform.SetParent(Manager.transform);

        transform.position = (Orbit.GetPosition(Index));
        SpinAround();
    }

    IEnumerator RestorePlanet(int orbitIndex, bool isEnemy = false) {
        //Debug.Log("THIS IS RESTORE PLANT");

        var maxTime = 0;
        long tmpTime = 0;
        DateTime now = DateTime.Now;

        PlayfabManager.GetServerTime(
            0,
            (result, index) => {
                now = result;
            });

        yield return new WaitForSecondsRealtime(1);

        if (isEnemy)
            tmpTime = (long)(now - DataGameSave.dataEnemy.ListPlanet[Manager.IndexSpace].ShootTime).TotalSeconds;
        else
            tmpTime = (long)(now - DataPlanet.ShootTime).TotalSeconds;

        switch (orbitIndex) {
            case 0:
                maxTime = ReadDataRestorePlanet.Instance.ListDataRestore[0].Time;
                break;
            case 1:
                maxTime = ReadDataRestorePlanet.Instance.ListDataRestore[1].Time;
                break;
            case 2:
                maxTime = ReadDataRestorePlanet.Instance.ListDataRestore[2].Time;
                break;
            case 3:
                maxTime = ReadDataRestorePlanet.Instance.ListDataRestore[3].Time;
                break;
            case 4:
                maxTime = ReadDataRestorePlanet.Instance.ListDataRestore[4].Time;
                break;
            case 5:
                maxTime = ReadDataRestorePlanet.Instance.ListDataRestore[5].Time;
                break;
            case 6:
                maxTime = ReadDataRestorePlanet.Instance.ListDataRestore[6].Time;
                break;
            case 7:
                maxTime = ReadDataRestorePlanet.Instance.ListDataRestore[7].Time;
                break;
            case 8:
                maxTime = ReadDataRestorePlanet.Instance.ListDataRestore[8].Time;
                break;
            case 9:
                maxTime = ReadDataRestorePlanet.Instance.ListDataRestore[9].Time;
                break;
            case 10:
                maxTime = ReadDataRestorePlanet.Instance.ListDataRestore[10].Time;
                break;
        }

        while (tmpTime < maxTime) {
            tmpTime++;
            yield return new WaitForSecondsRealtime(1);
        }

        WaitFx.SetActive(false);
        planetCollider.enabled = true;

        if (isEnemy) {
            go.GetComponent<CircleCollider2D>().enabled = true;
            DataGameSave.dataEnemy.ListPlanet[Manager.IndexSpace].ShootTime = DateTime.MinValue;
            RestorePlanetType(DataGameSave.dataEnemy.ListPlanet[Manager.IndexSpace]);
            SetData(DataGameSave.dataEnemy.ListPlanet[Manager.IndexSpace], orbitIndex, isEnemy);

            if (spriteAnim)
                spriteAnim.spriteRenderer.enabled = true;
        } else {
            DataPlanet.ShootTime = DateTime.MinValue;
            RestorePlanetType(DataPlanet);
            SetData(DataPlanet, orbitIndex, isEnemy);
        }
    }

    private void RestorePlanetType(DataPlanet planet) {
        planet.type = TypePlanet.Default;
    }

    public void OffTrail() {
        EffectTrailGameplay.SetActive(false);
        if (ShowTrail != null) {
            StopCoroutine(ShowTrail);
        }
        ShowTrail = StartCoroutine(MonoHelper.DoSomeThing(2, () => {
            EffectTrailGameplay.SetActive(true);
        }));
    }

    public void InverseClockwise(bool clockwise) {
        if (DataPlanet.ClockWise == clockwise) {
            return;
        }
        DataPlanet.ClockWise = clockwise;
        DataGameSave.SaveToServer(null);
    }

    public void ResetCollect(bool saveAfterReset = true) {
        if (collectCorou != null)
            StopCoroutine(collectCorou);
        collectCorou = null;

        PlayfabManager.GetServerTime(
           0,
           (result, index) => {
               DateTime now = result;
               DataPlanet.MaxTimeCollect = TimeMaxCollect = 43200;
               DataPlanet.CollectTime = now;
               DataPlanet.IsCollect = true;
               DataPlanet.MaterialCollect = 0;

               TxtCollect.text = "";
               MoneyCollect = 0;

               if (collectCorou == null && Scenes.Current == SceneName.Gameplay) {
                   collectCorou = StartCoroutine(CollectData());
               }

               if (saveAfterReset) {
                   DataGameSave.SaveToServer();
               }
           });
    }

    public void CheckCanUpgrade() {
        try {
            bool CheckElement = false;

            if ((DataMaster.PlanetUpgradeMaterialNeeded_Get(DataPlanet.Name, LevelSpeed) <= DataGameSave.dataLocal.M_Material && LevelSpeed < 10)
                || (DataMaster.PlanetUpgradeMaterialNeeded_Get(DataPlanet.Name, LevelHeavier) <= DataGameSave.dataLocal.M_Material && LevelHeavier < 10)
                || (DataMaster.PlanetUpgradeMaterialNeeded_Get(DataPlanet.Name, LevelSize) <= DataGameSave.dataLocal.M_Material && LevelSize < 10)
                || CheckElement) {
                CanUpgrade = true;
            } else {
                CanUpgrade = false;
            }

            if (Type == TypePlanet.Destroy) {
                CanUpgrade = false;
            }
        } catch { }
    }

    public Transform effectOutsideFather = null;

    public void UpdateEffectOutside(bool add = true) {
        if (add) {
            if (!effectOutsideFather)
                return;

            if (effectOutsideFather.transform.childCount > 0) {
                Destroy(effectOutsideFather.GetChild(0).gameObject);
            }

            GameObject prefab = null;

            switch (DataPlanet.type) {
                case TypePlanet.Antimatter:
                    prefab = GameManager.Instance.effectOutside_Anti;
                    break;

                case TypePlanet.Gravity:
                    prefab = GameManager.Instance.effectOutside_Gravity;
                    break;

                case TypePlanet.Ice:
                    prefab = GameManager.Instance.effectOutside_Ice;
                    break;

                case TypePlanet.Fire:
                    prefab = GameManager.Instance.effectOutside_Fire;
                    break;

                case TypePlanet.Air:
                    prefab = GameManager.Instance.effectOutside_Air;
                    break;

                case TypePlanet.Default:
                    prefab = GameManager.Instance.effectOutside_Default;
                    break;
            }

            if (prefab == null)
                return;

            Instantiate(prefab, effectOutsideFather);
        } else // remove effect
          {
            if (!effectOutsideFather || effectOutsideFather.childCount == 0)
                return;

            Destroy(effectOutsideFather.GetChild(0).gameObject);
        }
    }

    public void OnPressed_UpgradePlanet() {
        _isTap = true;
        OnMouseDown();
        OnMouseUp();
    }
}
