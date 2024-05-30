using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Hellmade.Sound;
using Facebook.Unity;
using SteveRogers;

public class MoneyManager : Singleton<MoneyManager>
{
    #region For Cheat
    [Header("Showing Material and Diamond")]
    public TextMeshProUGUI TxtMaterial;
    public TextMeshProUGUI TxtM_Air;
    public TextMeshProUGUI TxtM_Fire;
    public TextMeshProUGUI TxtM_Ice;
    public TextMeshProUGUI TxtM_Gravity;
    public TextMeshProUGUI TxtM_Antimatter;
    public TextMeshProUGUI TxtM_Diamond;
    #endregion

    [Header("Menu Animation")]
    public List<GameObject> menuItems;

    [Header("Anim")]
    public GameObject AirRT;
    public GameObject FireRT;
    public GameObject IceRT;
    public GameObject GravityRT;
    public GameObject AntimatterRT;
    public GameObject OjArrow;
    public GameObject DesMoney;
    public GameObject DesDiamond;

    [Header("Item")]
    public GameObject itemX2;
    public Text itemX2Txt;

    bool isOpen;

    private void Start()
    {
        isOpen = false;
        UpdateMoneyDisplay();
        ActiveItemX2(DataGameSave.dataLocal.itemX2.IsActived);
    }
    
    public void UpdateMoneyDisplay()
    {
        TxtMaterial.text = SteveRogers.Utilities.MoneyShorter(DataGameSave.dataLocal.M_Material, 1);
        TxtM_Air.text = DataGameSave.dataLocal.M_Air.ToString();
        TxtM_Fire.text =  DataGameSave.dataLocal.M_Fire.ToString();

        TxtM_Ice.text =  DataGameSave.dataLocal.M_Ice.ToString();
        TxtM_Gravity.text = DataGameSave.dataLocal.M_Gravity.ToString();
        TxtM_Antimatter.text =  DataGameSave.dataLocal.M_Antimatter.ToString();

        TxtM_Diamond.text = DataGameSave.dataLocal.Diamond.ToString();        
        SetData();
    }

    void SetData()
    {
        if (SteveRogers.Utilities.ActiveScene.name.Equals("GamePlay") && UIMultiScreenCanvasMan.modeExplore == UIMultiScreenCanvasMan.Mode.Upgrade)
        {
            PopupUpgradePlanet.Check();
        }
        
        SpaceManager.Instance.CheckAllPlanet();
    }

    void MoveMaterial(RectTransform rect, Action callback)
    {
        D2mTween.MoveAnchorX(rect, -85, 1, callback);
    }

    public void ActiveItemX2(bool isActive)
    {
//        if (isActive)
  //      {
    //        itemX2.SetActive(true);
      //      StartCoroutine(CountItemTime());
       // }
       // else
        //{
         //   itemX2Txt.text = "";
           // itemX2.SetActive(false);
        //}
    }

    IEnumerator CountItemTime()
    {
        long tmpTime = (long)(DateTime.Now - DataGameSave.dataLocal.itemX2.startTime).TotalSeconds;
        while (tmpTime < 86400) // 86400 = total second in 1 day
        {
            tmpTime++;
            itemX2Txt.text = TimeHelper.ConverSecondtoDate(86400 - tmpTime);
            yield return new WaitForSecondsRealtime(1);
        }

        DataGameSave.dataLocal.itemX2.IsActived = false;
    }

    public void OnMenu()
    {
        if (isOpen && !GameStatics.IsAnimating)
        {
            GameStatics.IsAnimating = true;
            LeanTween.moveLocalX(menuItems[0], 155, 0.5f).setEaseInOutBack();
            LeanTween.delayedCall(0.15f, () => { LeanTween.moveLocalX(menuItems[1], 155, 0.5f).setEaseInOutBack(); });
            LeanTween.delayedCall(0.3f, () => { LeanTween.moveLocalX(menuItems[2], 155, 0.5f).setEaseInOutBack(); });
            LeanTween.delayedCall(0.45f, () => { LeanTween.moveLocalX(menuItems[3], 155, 0.5f).setEaseInOutBack(); });
            //LeanTween.delayedCall(0.6f, () => { LeanTween.moveLocalX(AirRT, 155, 0.5f).setEaseInOutBack(); });
            //LeanTween.delayedCall(1.1f, () => {
            //    OjArrow.SetActive(true);

            //    GameStatics.IsAnimating = false;
            //    isOpen = false;
            //});

            GameStatics.IsAnimating = false;
            isOpen = false;

            //Sound
            EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
        }
        else if (!isOpen && !GameStatics.IsAnimating)
        {
            OjArrow.SetActive(false);

            GameStatics.IsAnimating = true;
            LeanTween.moveLocalX(menuItems[0], -85, 0.5f).setEaseOutBack();
            LeanTween.delayedCall(0.15f, () => { LeanTween.moveLocalX(menuItems[1], -85, 0.5f).setEaseOutBack(); });
            LeanTween.delayedCall(0.3f, () => { LeanTween.moveLocalX(menuItems[2], -85, 0.5f).setEaseOutBack(); });
            LeanTween.delayedCall(0.45f, () => { LeanTween.moveLocalX(menuItems[3], -85, 0.5f).setEaseOutBack(); });
            //LeanTween.delayedCall(0.6f, () => { LeanTween.moveLocalX(AntimatterRT, -85, 0.5f).setEaseOutBack(); });
            //LeanTween.delayedCall(1.1f, () => {
            //    GameStatics.IsAnimating = false;

            //    isOpen = true;
            //});

            GameStatics.IsAnimating = false;

            isOpen = true;

            //Sound
            EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
        }

    }

    public void OnOpenListMaterial()
    {
        if (isOpen && !GameStatics.IsAnimating)
        {
            GameStatics.IsAnimating = true;
            LeanTween.moveLocalX(AntimatterRT, 155, 0.5f).setEaseInOutBack();
            LeanTween.delayedCall(0.15f, () => { LeanTween.moveLocalX(GravityRT, 155, 0.5f).setEaseInOutBack(); });
            LeanTween.delayedCall(0.3f, () => { LeanTween.moveLocalX(IceRT, 155, 0.5f).setEaseInOutBack(); });
            LeanTween.delayedCall(0.45f, () => { LeanTween.moveLocalX(FireRT, 155, 0.5f).setEaseInOutBack(); });
            LeanTween.delayedCall(0.6f, () => { LeanTween.moveLocalX(AirRT, 155, 0.5f).setEaseInOutBack(); });
            LeanTween.delayedCall(1.1f, () => {
                OjArrow.SetActive(true);

                GameStatics.IsAnimating = false;
                isOpen = false;
            });
            //Sound
            EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
        }
        else if (!isOpen && !GameStatics.IsAnimating)
        {
                OjArrow.SetActive(false);

            GameStatics.IsAnimating = true;
            LeanTween.moveLocalX(AirRT, -85, 0.5f).setEaseOutBack();
            LeanTween.delayedCall(0.15f, () => { LeanTween.moveLocalX(FireRT, -85, 0.5f).setEaseOutBack(); });
            LeanTween.delayedCall(0.3f, () => { LeanTween.moveLocalX(IceRT, -85, 0.5f).setEaseOutBack(); });
            LeanTween.delayedCall(0.45f, () => { LeanTween.moveLocalX(GravityRT, -85, 0.5f).setEaseOutBack(); });
            LeanTween.delayedCall(0.6f, () => { LeanTween.moveLocalX(AntimatterRT, -85, 0.5f).setEaseOutBack(); });
            LeanTween.delayedCall(1.1f, () => {
                GameStatics.IsAnimating = false;

                isOpen = true;
            });
            //Sound
            EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
        }
     
    }
    
    public void OnShop(bool isDiamond)
    {
        if (!TutMan.IsDone(TutMan.CAN_PRESS_BUTTON_IN_GAMEPLAY))
            return;

        if (GameStatics.IsAnimating)
            return;

        PopupShop.Show(isDiamond);
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
    }

    public void OnUpMater(int value)
    {
        if (value > 0)
            DataGameSave.dataLocal.M_Material *= value;
        TxtMaterial.text = SteveRogers.Utilities.MoneyShorter(DataGameSave.dataLocal.M_Material, 1);
        SetData();
        //if(PopupUpgradeSun.Isshow)
        //{
        //    PopupUpgradeSun.CheckUp();
        //}
    }

    public void OnUpAir(int value)
    {
        if (value > 0)
        DataGameSave.dataLocal.M_Air += value;
        TxtM_Air.text = DataGameSave.dataLocal.M_Air.ToString();
        SetData();
    }

    public void OnUpFire(int Value)
    {
        if (Value > 0)
            DataGameSave.dataLocal.M_Fire += Value;
        TxtM_Fire.text = DataGameSave.dataLocal.M_Fire.ToString();
        SetData();
    }

    public void OnUpIce(int value)
    {
        if (value > 0)
            DataGameSave.dataLocal.M_Ice += value;
        TxtM_Ice.text =  DataGameSave.dataLocal.M_Ice.ToString();
        SetData();
    }

    public void OnUpGravity(int value)
    {
        if (value > 0)
            DataGameSave.dataLocal.M_Gravity += value;
        TxtM_Gravity.text = DataGameSave.dataLocal.M_Gravity.ToString();
        SetData();
    }

    public void OnUpAntimat(int value)
    {
        if (value > 0)
            DataGameSave.dataLocal.M_Antimatter += value;
        TxtM_Antimatter.text =  DataGameSave.dataLocal.M_Antimatter.ToString();
        SetData();
    }


    public void OnPressed_Pause()
    {
        if (!PlayScenesManager.Instance)
            return;

        PlayScenesManager.Instance.pausingSpins = !PlayScenesManager.Instance.pausingSpins;

        if (!PlayScenesManager.Instance.pausingSpins)
        {
            foreach (var i in SpaceManager.Instance.ListSpace)
                i.Planet.SpinAround();
        }
    }

    #region For Cheat
    public void OnUpMaterC(int value)
    {
        if (!TutMan.IsDone(TutMan.CAN_PRESS_BUTTON_IN_GAMEPLAY))
            return;

#if TEST
        if (value > 0)
            DataGameSave.dataLocal.M_Material *= value;
        TxtMaterial.text = DataGameSave.dataLocal.M_Material.ToString();
        SetData();
        if (PopupUpgradeSun.Isshow)
        {
            PopupUpgradeSun.CheckUp();
        }
#endif
    }

    public void OnUpAirC(int value)
    {
#if TEST
        if (value > 0)
            DataGameSave.dataLocal.M_Air += value;
        TxtM_Air.text = DataGameSave.dataLocal.M_Air.ToString();
        SetData();
#endif
    }

    public void OnUpFireC(int Value)
    {
#if TEST
        if (Value > 0)
            DataGameSave.dataLocal.M_Fire += Value;
        TxtM_Fire.text = DataGameSave.dataLocal.M_Fire.ToString();
        SetData();
#endif
    }

    public void OnUpIceC(int value)
    {
#if TEST
        if (value > 0)
            DataGameSave.dataLocal.M_Ice += value;
        TxtM_Ice.text = DataGameSave.dataLocal.M_Ice.ToString();
        SetData();
#endif
    }

    public void OnUpGravityC(int value)
    {
#if TEST
        if (value > 0)
            DataGameSave.dataLocal.M_Gravity += value;
        TxtM_Gravity.text = DataGameSave.dataLocal.M_Gravity.ToString();
        SetData();
#endif
    }

    public void OnUpAntimatC(int value)
    {
#if TEST
        if (value > 0)
            DataGameSave.dataLocal.M_Antimatter += value;
        TxtM_Antimatter.text = DataGameSave.dataLocal.M_Antimatter.ToString();
        SetData();
#endif
    }
    #endregion
}
