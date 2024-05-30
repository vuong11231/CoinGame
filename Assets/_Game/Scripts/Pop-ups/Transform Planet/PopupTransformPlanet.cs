using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Hellmade.Sound;

public class PopupTransformPlanet : Popups
{
    static PopupTransformPlanet _Instance;
    Action _onClose;
    public Image ImgMain;
    public Animator AnimMain;
    public TextMeshProUGUI TxtPrice;
    public Text TxtContent;
    int IdPlanet = 1;
    public List<Sprite> SprPlanet;

    // steve

    public Transform planetParticleEffectFather = null;
    
    public GameObject planetParticleEffect_Fire = null;
    public GameObject planetParticleEffect_Air = null;
    public GameObject planetParticleEffect_Gravity = null;
    public GameObject planetParticleEffect_Ice = null;
    public GameObject planetParticleEffect_Antimatter = null;

    // steve.

    public Button BtnTransform;

    public static bool IsShow = false;

    static void CheckInstance()
    {
        if (_Instance == null)
        {
            _Instance = Instantiate(
            Resources.Load<PopupTransformPlanet>("Prefabs/Pop-ups/Transform Planet/Popup Transform Planet"),
            Popups.CanvasPopup.transform,
            false);
        }
    }

    public static void Show()
    {
        CheckInstance();
        // _Instance.ImgMain.sprite = Spr;
        _Instance.Appear();
    }

    void SetData()
    {
        IsShow = true;

        OnChange(0);
    }

    #region Overrive Methods
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
        //Sound
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
    #endregion

    public void OnClose()
    {
        if (GameStatics.IsAnimating)
            return;
        _onClose = () =>
        {
            SpaceManager.Instance.PlanetSelect = null;
            SpaceManager.Instance.FxBack.SetActive(false);
            //Disappear();
        };
        Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnChange(int value)
    {
        if (GameStatics.IsAnimating) return;
        IdPlanet += value;

        if (IdPlanet == -1)
        {
            IdPlanet = 4;
        }
        else if (IdPlanet == 5)
        {
            IdPlanet = 0;
        }
        
        StartCoroutine(MonoHelper.DoSomeThing(.02f, () =>
        {
            // steve

            //Debug.LogError(((TypePlanet)IdPlanet).ToString());

            var type = (TypePlanet)IdPlanet;
            AnimMain.Play((type).ToString());

            foreach (Transform t in planetParticleEffectFather)
                t.gameObject.SetActive(false);

            if (type == TypePlanet.Fire)
            {
                planetParticleEffect_Fire.SetActive(true);
                //ImgMain.transform.localScale = Vector3.one * 0.8f;
            }
            else if (type == TypePlanet.Ice)
            {
                planetParticleEffect_Ice.SetActive(true);
            }
            else if (type == TypePlanet.Gravity)
            {
                planetParticleEffect_Gravity.SetActive(true);
            }
            else if (type == TypePlanet.Air)
            {
                planetParticleEffect_Air.SetActive(true);
            }
            else if (type == TypePlanet.Antimatter)
            {
                planetParticleEffect_Antimatter.SetActive(true);
            }
            //else
            //{
            //    ImgMain.transform.localScale = Vector3.one;
            //}

            // steve.
        }));
        if (SpaceManager.Instance.PlanetSelect.Type==TypePlanet.Default)
        {
            TxtPrice.text = "<sprite=\"meteorite\" index=" + IdPlanet + "> x1";
            BtnTransform.gameObject.SetActive(true);
            switch (IdPlanet)
            {
                case 0:
                    {
                        TxtContent.text = TextConstants.ContentAntimatter;
                        BtnTransform.interactable = DataGameSave.dataLocal.M_Antimatter > 0;
                        break;
                    }

                case 1:
                    {
                        TxtContent.text = TextConstants.ContentGravity;
                        BtnTransform.interactable = DataGameSave.dataLocal.M_Gravity > 0;
                        break;
                    }

                case 2:
                    {
                        TxtContent.text = TextConstants.ContentIce;
                        BtnTransform.interactable = DataGameSave.dataLocal.M_Ice > 0;
                        break;
                    }

                case 3:
                    {
                        TxtContent.text = TextConstants.ContentFire;
                        BtnTransform.interactable = DataGameSave.dataLocal.M_Fire > 0;
                        break;
                    }

                case 4:
                    {
                        TxtContent.text = TextConstants.ContentAir;
                        BtnTransform.interactable = DataGameSave.dataLocal.M_Air > 0;
                        break;
                    }
            }
        }
        else
        {
            switch (IdPlanet)
            {
                case 0:
                    {
                        TxtContent.text = TextConstants.ContentAntimatter;
                        break;
                    }

                case 1:
                    {
                        TxtContent.text = TextConstants.ContentGravity;
                        break;
                    }

                case 2:
                    {
                        TxtContent.text = TextConstants.ContentIce;
                        break;
                    }

                case 3:
                    {
                        TxtContent.text = TextConstants.ContentFire;
                        break;
                    }

                case 4:
                    {
                        TxtContent.text = TextConstants.ContentAir;
                        break;
                    }
            }

            if (IdPlanet != (int)SpaceManager.Instance.PlanetSelect.Type)
            {
               // BtnTransform.interactable = DataGameSave.dataLocal.Diamond  >= PopupConstants.TRANSFORM_PLANET;
                TxtPrice.text = PopupConstants.TRANSFORM_PLANET + TextConstants.M_Money;
                BtnTransform.gameObject.SetActive(true);
            }
            else
            {
                TxtPrice.text = "USING";
                BtnTransform.gameObject.SetActive(false);
                //BtnTransform.gameObject.SetActive()
            }

        }
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public static void Check()
    {
        bool Check = true;
        if (SpaceManager.Instance.PlanetSelect.Type == TypePlanet.Default)
        {
            switch (_Instance.IdPlanet)
            {

                case 0:
                    {
                        Check = DataGameSave.dataLocal.M_Antimatter > 0;
                        break;
                    }

                case 1:
                    {
                        Check = DataGameSave.dataLocal.M_Gravity > 0;
                        break;
                    }

                case 2:
                    {
                        Check = DataGameSave.dataLocal.M_Ice > 0;
                        break;
                    }

                case 3:
                    {
                        Check = DataGameSave.dataLocal.M_Fire > 0;
                        break;
                    }

                case 4:

                    {
                        Check = DataGameSave.dataLocal.M_Air > 0;
                        break;
                    }

            }
        }
        _Instance.BtnTransform.interactable = Check;
    }

    public void OnTransform()
    {
        if (GameStatics.IsAnimating)
            return;
        _onClose = () =>
        {
            if (SpaceManager.Instance.PlanetSelect.Type == TypePlanet.Default)
            {
                switch (IdPlanet)
                {
                    case 0:
                        {
                            DataGameSave.dataLocal.M_Antimatter -= 1;
                            break;
                        }
                    case 1:
                        {
                            DataGameSave.dataLocal.M_Gravity -= 1;
                            break;
                        }
                    case 2:
                        {
                            DataGameSave.dataLocal.M_Ice -= 1;
                            break;
                        }
                    case 3:
                        {
                            DataGameSave.dataLocal.M_Fire -= 1;
                            break;
                        }
                    case 4:
                        {
                            DataGameSave.dataLocal.M_Air -= 1;
                            break;
                        }


                }
                SpaceManager.Instance.PlanetSelect.UpgradeChange((TypePlanet)IdPlanet);
                SpaceManager.Instance.FxBack.SetActive(false);

                // Achievement
                DataGameSave.dataLocal.TransformPlanet++;
                // Daily mission
                DataGameSave.dataLocal.dailyMissions[(int)DailyQuests.TransformPlanet].currentProgress++;
                if (BtnDailyMission.Instance)
                {
                    BtnDailyMission.Instance.CheckDoneQuest();
                }
                DataGameSave.SaveToServer();
                // Analytics
                AnalyticsManager.Instance.TrackTransformPlanet((TypePlanet)IdPlanet);
            }
            else
            {
                if(DataGameSave.dataLocal.Diamond >= PopupConstants.TRANSFORM_PLANET)
                {
                    DataGameSave.dataLocal.Diamond -= PopupConstants.TRANSFORM_PLANET;

                    SpaceManager.Instance.PlanetSelect.UpgradeChange((TypePlanet)IdPlanet);
                    SpaceManager.Instance.FxBack.SetActive(false);

                    // Achievement
                    DataGameSave.dataLocal.TransformPlanet++;
                    // Daily mission
                    DataGameSave.dataLocal.dailyMissions[(int)DailyQuests.TransformPlanet].currentProgress++;
                    if (BtnDailyMission.Instance)
                    {
                        BtnDailyMission.Instance.CheckDoneQuest();
                    }
                    DataGameSave.SaveToServer();
                }
                else
                {
                    PopupShop.Show(true);
                }
            }
        };

        Disappear();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }
}


