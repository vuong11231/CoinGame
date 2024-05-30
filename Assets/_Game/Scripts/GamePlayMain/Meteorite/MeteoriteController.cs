using UnityEngine;
using Lean.Pool;
using SteveRogers;
using System.Collections;
using TMPro;

public class MeteoriteController : MonoBehaviour
{
    public SpriteRenderer ImgMain;
    public GameObject TrailObject;
    public TypeMaterial Type;
    public TypeSize TypeSize;
    public GameObject PrefabCollect;
    public TrailRenderer Trail;
    public CircleCollider2D circleCollider2D = null;

    int Value;
    public System.Action actiondown = null;
    public bool isDiamond = false;

    private void Start()
    {
        actiondown = OnMouseDown;
        circleCollider2D = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if (UIMultiScreenCanvasMan.modeExplore != UIMultiScreenCanvasMan.Mode.Gameplay)
        {
            if (ImgMain.enabled)
            {
                circleCollider2D.enabled = ImgMain.enabled = Trail.enabled = false;
            }
        }
        else // gameplay mode
        {
            if (!ImgMain.enabled)
            {
                circleCollider2D.enabled = ImgMain.enabled = Trail.enabled = true;
            }
        }
    }

    public void Move(float Time, Vector3 Pos)
    {
        LeanTween.move(gameObject, Pos, Time)
            .setOnComplete(() =>
            {
                LeanPool.Despawn(gameObject);
                TrailObject.SetActive(false);
            });

        StartCoroutine(MonoHelper.DoSomeThing(3f, () =>
        {
            TrailObject.SetActive(true);
        }));
    }

    public void Change(Sprite Spr)
    {
        ImgMain.sprite = Spr;
    }

    public void SetData(TypeMaterial _Type, TypeSize Scale, int _Value, bool isDiamond = false)
    {
        this.isDiamond = isDiamond;
        TypeSize = Scale;
        SetTrail();
        if (_Type == TypeMaterial.Default)
        {
            Value = _Value * DataGameSave.dataServer.level;
        }
        else
        {
            Value = 1;
        }
        Type = _Type;

        float mulsize = 1;

        if (Type != TypeMaterial.Default)
            mulsize = 0.3f;

        if (Scale == TypeSize.Big)
        {
            transform.localScale = Vector3.one * 3 * mulsize;
        }
        else if (Scale == TypeSize.Normal)
        {
            transform.localScale = Vector3.one * 2.5f * mulsize;
        }
        else
        {
            transform.localScale = Vector3.one * 2 * mulsize;
        }
    }

    private void OnMouseDown()
    {
        if (GameStatics.IsAnimating || Popups.IsShowed)
            return;

        if (!TutMan.IsDone(TutMan.CAN_PRESS_METEOR_IN_GAMEPLAY))
            return;

        if (!isDiamond && Type == TypeMaterial.Default)
        {
            var s = SpaceManager.Instance.ListSpace.Get(0);

            if (s)
                s.ColectMeteorite(Type, Value);

            LeanTween.cancel(gameObject);

            StartCoroutine(MonoHelper.DoSomeThing(.1f, () =>
            {
                GameObject temp = LeanPool.Spawn(PrefabCollect, transform.position, Quaternion.identity);
                temp.GetComponent<CollectMaterialFx>().SetFx(Value, (TypePlanet)(int)Type);
                TrailObject.SetActive(false);
                LeanPool.Despawn(gameObject);
            }));

            Sounds.Instance.PlayCollect();
        }
        else
        {
            TrailObject.SetActive(false);
            ImgMain.color = Color.clear;

            //LeanTween.cancel(gameObject);
            Destroy(gameObject);

            var value = isDiamond ? Value : DataGameSave.dataLocal.itemX2.multiplyNumber;

            if (isDiamond)
            {
                DataGameSave.dataLocal.Diamond += 20;
                //GameObject isDiamond = LeanPool.Spawn(PrefabCollect, transform.position, Quaternion.identity);
                //isDiamond.GetComponent<CollectMaterialFx>().SetFx(Value, (TypePlanet.Diamond)(int)Type);
                ////TrailObject.SetActive(false);
                //ImgMain.color = Color.white;
                //LeanPool.Despawn(gameObject);

                //Vector3 pos = transform.position + new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-2f, 2f));
                SpawnDiamondCoroutine(transform.position, value);
            }
            else
            {
                SpaceManager.Instance.ListSpace[0].ColectMeteorite(Type, Value);

                StartCoroutine(MonoHelper.DoSomeThing(.1f, () =>
                {
                    GameObject temp = LeanPool.Spawn(PrefabCollect, transform.position, Quaternion.identity);
                    temp.GetComponent<CollectMaterialFx>().SetFx(Value, (TypePlanet)(int)Type);
                    //TrailObject.SetActive(false);
                    ImgMain.color = Color.white;
                    LeanPool.Despawn(gameObject);
                }));

                // Achievement
                DataGameSave.dataLocal.WatchAds++;
            }

            DataGameSave.SaveToServer();
/*            WatchAdsClaimMat.Show(value, Type, () =>
            {
                GoogleMobileAdsManager.Instance.ShowRewardedVideo(
                    () =>
                    {

                    });
            },
            () =>
            {

                ImgMain.color = Color.white;
                LeanPool.Despawn(gameObject);
            },
            isDiamond);*/

            // Analytics
            AnalyticsManager.Instance.TrackSelectMeteor(Type);
        }
        //Sound
        Sounds.Instance.PlayCollect();
    }

    void SetTrail()
    {

        Trail.startWidth = 1 + 0.5f * ((int)TypeSize);
        Trail.time = .7f * ((int)TypeSize + 1);
    }

    public void SpawnDiamondCoroutine(Vector3 pos, int amount)
    {
        //yield return new WaitForSeconds(time);
        amount = 20;

        GameObject spawn = Instantiate(Resources.Load<GameObject>("Diamond Collect"), pos, Quaternion.identity);
        spawn.transform.localPosition = new Vector3(pos.x, pos.y, -10);
        spawn.GetComponentInChildren<TextMeshPro>().text = amount.ToString();
        // spawn.transform.LeanScale(Vector3.one, 0);
        LeanTween.cancel(spawn);

        LeanTween.moveLocal(spawn, MoneyManager.Instance.DesDiamond.transform.position, 3f).setEaseInCubic().setOnComplete(() =>
        {
            LeanPool.Despawn(spawn);
        });
    }
}
