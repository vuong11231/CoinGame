using AppsFlyerSDK;
using Hellmade.Sound;
using SteveRogers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class PopupShop : Popups
{
    public static PopupShop Instance;

    [Header("UI")]
    public Sprite offImg;
    public Sprite onImg;
    public Sprite offBtn;
    public Sprite onBtn;
    public List<Image> ItemBtns;
    public Image DiamondBtn;
    public Image MaterialBtn;
    public Button X2Btn;

    [Header("Anim")]
    public List<GameObject> Lines;
    public Animation anim;

    [Header("Set Data")]
    public List<Text> DiamondTxt;
    public List<Text> ItemTxt;

    public GameObject btnDiamondFree;
    public GameObject btnAutoRecoverFree;
    public GameObject btnTotalAttackFree;

    public GameObject btnDiamondAds;
    public GameObject btnAutoRecoverAds;
    public GameObject btnTotalAttackAds;

    Action _onClose;
    bool IsDiamond;
    //private const string inappAndroidPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAyO4p433LMgm3kLyGuf01n15YiWOhXvPfGfRF0Gcx7R2CIv3LUiQ7b0YbyzdchgxdpI6WbHaZAuNOQgNbkDSmea0+FP4JQMp0bhF3kHO5hEFnk4KEbrnRm3Ka7W44uMqqkggnwqwkjcwOdiGGoiHls9RPDzSD/1iJw/an8fKsHNDwXks35Wf9QUZAd1L8F7dlkO7ABJ/6czwVoOd2ph1ToEX8fjSfcYAoML5deIuMZKM7N1LpNpizoNCptyFry2kZl2a7B3ie2AV5HZGlEp46QTFXo5HybJGv8zqH8k+k3lBiGAeNp9uqPKJ06tUsHbYqaINpXWgPnAveqeIgOb+uCwIDAQAB";

    public static void Show(bool isDiamond)
    {
        if (Instance == null)
        {
            Instance = Instantiate(
            Resources.Load<PopupShop>("Prefabs/Pop-ups/Shop/Popup Shop"),
            Popups.CanvasPopup.transform,
            false);
        }

        Instance.IsDiamond = isDiamond;
        Instance.Appear();

        if (MenuAnimation.Instance)
            MenuAnimation.Instance.CloseMenu();
    }

    public override void Appear()
    {
        base.Appear();
        SetShopUI();
        OnChooseShop(IsDiamond);
        UpdateWatchAdsButtonDisplay();
        anim.Play("Show_ShopPopUp");
        EazySoundManager.PlaySound(Sounds.Instance.Show_Popup);
    }

    public override void Disappear()
    {
        StartCoroutine(HidePopUp());
    }

    IEnumerator HidePopUp()
    {
        anim.Play("Hide_ShopPopUp");
        while (anim.IsPlaying("Hide_ShopPopUp"))
        {
            yield return null;
        }
        base.Disappear();
        if (_onClose != null)
        {
            _onClose.Invoke();
            _onClose = null;
        }
    }

    public void UpdateWatchAdsButtonDisplay()
    {
        int countDiamond = int.Parse(DataGameSave.GetMetaData(MetaDataKey.SHOP_GET_DIAMOND_COUNT));
        int countTotalAttack = int.Parse(DataGameSave.GetMetaData(MetaDataKey.SHOP_TOTAL_ATTACK_COUNT));
        int countAutoRestore = int.Parse(DataGameSave.GetMetaData(MetaDataKey.SHOP_AUTO_RESTORE_COUNT));

        int freeNumber = GameManager.MAX_WATCH_ADS_PER_DAY_LIMIT + 1;

        btnAutoRecoverFree.SetActive(countAutoRestore == freeNumber);
        btnAutoRecoverAds.SetActive(countAutoRestore != freeNumber);

        btnDiamondFree.SetActive(countDiamond == freeNumber);
        btnDiamondAds.SetActive(countDiamond != freeNumber);

        btnTotalAttackFree.SetActive(countTotalAttack == freeNumber);
        btnTotalAttackAds.SetActive(countTotalAttack != freeNumber);

        btnAutoRecoverAds.transform.GetChild(1).GetComponent<Text>().text = countAutoRestore + "/" + GameManager.MAX_WATCH_ADS_PER_DAY_LIMIT;
        btnDiamondAds.transform.GetChild(1).GetComponent<Text>().text = countDiamond + "/" + GameManager.MAX_WATCH_ADS_PER_DAY_LIMIT;
        btnTotalAttackAds.transform.GetChild(1).GetComponent<Text>().text = countTotalAttack + "/" + GameManager.MAX_WATCH_ADS_PER_DAY_LIMIT;
    }

    public override void Disable()
    {
        base.Disable();
    }

    public override void NextStep(object value = null)
    {

    }

    void CheckShowButton()
    {

    }

    void SetShopUI()
    {
        var count = DiamondTxt.Count;
        for (int i = 0; i < count; i++)
        {
            DiamondTxt[i].text = ShopData.Instance.DiamondValue[i].ToString();

            if (i == count - 1)
                continue;
            else
                ItemTxt[i].text = ShopData.Instance.Material[i].NumberItem.ToString();
        }
        SetItemButtonUI();
    }

    void SetItemButtonUI()
    {
        var count = ItemBtns.Count;

        for (int i = 0; i < count; i++)
        {
            if (DataGameSave.dataLocal.Diamond >= ShopData.Instance.Material[i].Cost)
                ItemBtns[i].sprite = onBtn;
            else
                ItemBtns[i].sprite = offBtn;
            // Special case
            if (i == count - 1)
            {
                if (DataGameSave.dataLocal.itemX2.IsActived)
                    X2Btn.interactable = false;
                else
                    X2Btn.interactable = true;
            }
        }
    }

    public void OnChooseShop(bool isDiamond)
    {
        if (isDiamond && !GameStatics.IsAnimating)
        {
            GameStatics.IsAnimating = true;
            DiamondBtn.sprite = onImg;
            MaterialBtn.sprite = offImg;
            //
            LeanTween.moveLocalX(Lines[0], 487, 0.5f).setEaseInOutBack();
            LeanTween.delayedCall(0.15f, () => { LeanTween.moveLocalX(Lines[1], 487, 0.5f).setEaseInOutBack(); });
            LeanTween.delayedCall(0.3f, () => { LeanTween.moveLocalX(Lines[2], 487, 0.5f).setEaseInOutBack(); });
            LeanTween.delayedCall(0.45f, () => { LeanTween.moveLocalX(Lines[3], 487, 0.5f).setEaseInOutBack(); });
            LeanTween.delayedCall(0.6f, () => { LeanTween.moveLocalX(Lines[4], 487, 0.5f).setEaseInOutBack(); });
            LeanTween.delayedCall(0.75f, () => { LeanTween.moveLocalX(Lines[5], 487, 0.5f).setEaseInOutBack(); });
            LeanTween.delayedCall(0.9f, () => { LeanTween.moveLocalX(Lines[6], 487, 0.5f).setEaseInOutBack(); });
            LeanTween.delayedCall(1.4f, () =>
            {
                GameStatics.IsAnimating = false;
            });
            //Sound
            EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
        }
        else if (!isDiamond && !GameStatics.IsAnimating)
        {
            GameStatics.IsAnimating = true;
            DiamondBtn.sprite = offImg;
            MaterialBtn.sprite = onImg;
            //
            LeanTween.moveLocalX(Lines[0], -487, 0.5f).setEaseOutBack();
            LeanTween.delayedCall(0.15f, () => { LeanTween.moveLocalX(Lines[1], -487, 0.5f).setEaseInOutBack(); });
            LeanTween.delayedCall(0.3f, () => { LeanTween.moveLocalX(Lines[2], -487, 0.5f).setEaseInOutBack(); });
            LeanTween.delayedCall(0.45f, () => { LeanTween.moveLocalX(Lines[3], -487, 0.5f).setEaseInOutBack(); });
            LeanTween.delayedCall(0.6f, () => { LeanTween.moveLocalX(Lines[4], -487, 0.5f).setEaseInOutBack(); });
            LeanTween.delayedCall(0.75f, () => { LeanTween.moveLocalX(Lines[5], -487, 0.5f).setEaseInOutBack(); });
            LeanTween.delayedCall(0.9f, () => { LeanTween.moveLocalX(Lines[6], -487, 0.5f).setEaseInOutBack(); });
            LeanTween.delayedCall(1.4f, () =>
            {
                GameStatics.IsAnimating = false;
            });
            //Sound
            EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
        }
    }

    public void OnBuy(int itemType)
    {
        if ((IAPItem)itemType == (IAPItem._x2_material_ios)) {
            IAPManager.Instance.Purchase(IAPItem._x2_material_ios,
               (product) =>
               {
                   PopupConfirm.ShowOK(TextMan.Get("Congratulations"), TextMan.Get("You activated X2 material in 24 hours."), "Great");

                   if (DataGameSave.x2MaterialEndTime < GameManager.Now.Ticks)
                   {
                       DataGameSave.x2MaterialEndTime = GameManager.Now.Ticks;
                   }

                   DateTime newEndTime = new DateTime(DataGameSave.x2MaterialEndTime).AddHours(24);
                   DataGameSave.x2MaterialEndTime = newEndTime.Ticks;
                   DataGameSave.SaveToServer();
               },
               () =>
               {
                   PopupConfirm.ShowOK("Error!!!", "Purchase failed, please try again.");
               });
        }
        else if (itemType >= 17)
        {
            IAPManager.Instance.Purchase((IAPItem)itemType,
                        (product) =>
                        {
                            PopupConfirm.ShowOK(TextMan.Get("Congratulations"), TextMan.Get("You activated auto restore for 2 hour."), "Great", () =>
                            {
                                if (!GameManager.IsAutoRestorePlanet)
                                {
                                    DataGameSave.autoRestoreEndTime = GameManager.Now.Ticks;
                                }

                                DateTime newEndTime = new DateTime(DataGameSave.autoRestoreEndTime).AddHours(2);
                                DataGameSave.autoRestoreEndTime = newEndTime.Ticks;
                                DataGameSave.SaveToServer();
                            });
                        },
                        () =>
                        {
                            PopupConfirm.ShowOK("Error!!!", "Purchase failed, please try again.");
                        });
        }
        else
        {
            var type = (TypeShopItem)itemType;

            switch (type)
            {
                case TypeShopItem.Antimatter:
                    if (DataGameSave.dataLocal.Diamond >= ShopData.Instance.Material[4].Cost)
                    {
                        DataGameSave.dataLocal.Diamond -= ShopData.Instance.Material[4].Cost;
                        DataGameSave.dataLocal.M_Antimatter += ShopData.Instance.Material[4].NumberItem;
                    }
                    else
                    {
                        OnChooseShop(true);
                    }
                    break;
                case TypeShopItem.Gravity:
                    if (DataGameSave.dataLocal.Diamond >= ShopData.Instance.Material[5].Cost)
                    {
                        DataGameSave.dataLocal.Diamond -= ShopData.Instance.Material[5].Cost;
                        DataGameSave.dataLocal.M_Gravity += ShopData.Instance.Material[5].NumberItem;
                    }
                    else
                    {
                        OnChooseShop(true);
                    }
                    break;
                case TypeShopItem.Ice:
                    if (DataGameSave.dataLocal.Diamond >= ShopData.Instance.Material[2].Cost)
                    {
                        DataGameSave.dataLocal.Diamond -= ShopData.Instance.Material[2].Cost;
                        DataGameSave.dataLocal.M_Ice += ShopData.Instance.Material[2].NumberItem;
                    }
                    else
                    {
                        OnChooseShop(true);
                    }
                    break;
                case TypeShopItem.Fire:
                    if (DataGameSave.dataLocal.Diamond >= ShopData.Instance.Material[1].Cost)
                    {
                        DataGameSave.dataLocal.Diamond -= ShopData.Instance.Material[1].Cost;
                        DataGameSave.dataLocal.M_Fire += ShopData.Instance.Material[1].NumberItem;
                    }
                    else
                    {
                        OnChooseShop(true);
                    }
                    break;
                case TypeShopItem.Air:
                    if (DataGameSave.dataLocal.Diamond >= ShopData.Instance.Material[3].Cost)
                    {
                        DataGameSave.dataLocal.Diamond -= ShopData.Instance.Material[3].Cost;
                        DataGameSave.dataLocal.M_Air += ShopData.Instance.Material[3].NumberItem;
                    }
                    else
                    {
                        OnChooseShop(true);
                    }
                    break;
                case TypeShopItem.Default:
                    if (DataGameSave.dataLocal.Diamond >= ShopData.Instance.Material[0].Cost)
                    {
                        DataGameSave.dataLocal.Diamond -= ShopData.Instance.Material[0].Cost;
                        DataGameSave.dataLocal.M_Material += ShopData.Instance.Material[0].NumberItem;
                    }
                    else
                    {
                        OnChooseShop(true);
                    }
                    break;
                case TypeShopItem.X2:
                    IAPManager.Instance.Purchase((IAPItem)itemType,
                       (product) =>
                       {
                           PopupConfirm.ShowOK(TextMan.Get("Congratulations"), TextMan.Get("You activated X2 material in 24 hours."), "Great");
                           DataGameSave.SaveMetaData(MetaDataKey.X2_MATERIAL_END_TIME, "1");
                           DataGameSave.SaveToServer();

                           if (DataGameSave.x2MaterialEndTime < GameManager.Now.Ticks)
                           {
                               DataGameSave.x2MaterialEndTime = GameManager.Now.Ticks;
                           }

                           DateTime newEndTime = new DateTime(DataGameSave.x2MaterialEndTime).AddHours(24);
                           DataGameSave.x2MaterialEndTime = newEndTime.Ticks;
                           DataGameSave.SaveToServer();
                       },
                       () =>
                       {
                           PopupConfirm.ShowOK("Error!!!", "Purchase failed, please try again.");
                       });
                    break;
                case TypeShopItem.Diamond_1:
                    IAPManager.Instance.Purchase(IAPItem.Diamond_1,
                        (product) =>
                        {
                            DataGameSave.dataLocal.Diamond += ShopData.Instance.DiamondValue[0];
                            TrackBuy("diamond_1", product);
                        },
                        () =>
                        {
                            PopupConfirm.ShowOK("Error!!!", "Purchase failed, please try again.");
                        });
                    break;
                case TypeShopItem.Diamond_2:
                    IAPManager.Instance.Purchase(IAPItem.Diamond_2,
                     (product) =>
                     {
                         DataGameSave.dataLocal.Diamond += ShopData.Instance.DiamondValue[1];
                         TrackBuy("diamond_2", product);
                     },
                     () =>
                     {
                         PopupConfirm.ShowOK("Error!!!", "Purchase failed, please try again.");
                     });
                    break;
                case TypeShopItem.Diamond_3:
                    IAPManager.Instance.Purchase(IAPItem.Diamond_3,
                        (product) =>
                        {
                            DataGameSave.dataLocal.Diamond += ShopData.Instance.DiamondValue[2];
                            TrackBuy("diamond_3", product);
                        },
                        () =>
                        {
                            PopupConfirm.ShowOK("Error!!!", "Purchase failed, please try again.");
                        });
                    break;
                case TypeShopItem.Diamond_4: // change diamond 4 to Battle Pass Vip

                    //IAPManager.Instance.Purchase(IAPItem.Diamond_4,
                    //    (product) => {
                    //        DataGameSave.dataLocal.Diamond += ShopData.Instance.DiamondValue[3];
                    //        TrackBuy("diamond_4", product);
                    //    },
                    //    () => {
                    //        PopupConfirm.ShowOK("Error!!!", "Purchase failed, please try again.");
                    //    });
                    //break;

                    if (DataGameSave.GetMetaData(MetaDataKey.BATTLE_PASS_VIP_ENABLED) == "1")
                    {
                        PopupConfirm.ShowOK(TextMan.Get("Information"), TextMan.Get("Battle Pass vip is already activated"));
                    }
                    else
                    {
                        BattlePass.BuyVipBattlePass();
                    }
                    break;
                case TypeShopItem.Diamond_5:
                    IAPManager.Instance.Purchase(IAPItem.Diamond_5,
                        (product) =>
                        {
                            DataGameSave.dataLocal.Diamond += ShopData.Instance.DiamondValue[4];
                            TrackBuy("diamond_5", product);
                        },
                        () =>
                        {
                            PopupConfirm.ShowOK("Error!!!", "Purchase failed, please try again.");
                        });
                    break;
                case TypeShopItem.Diamond_6:
                    IAPManager.Instance.Purchase(IAPItem.Diamond_6,
                        (product) =>
                        {
                            DataGameSave.dataLocal.Diamond += ShopData.Instance.DiamondValue[5];
                            TrackBuy("diamond_6", product);
                        },
                        () =>
                        {
                            PopupConfirm.ShowOK("Error!!!", "Purchase failed, please try again.");
                        });
                    break;
                case TypeShopItem.AutoRestoreIAP:
                    IAPManager.Instance.Purchase(IAPItem.Diamond_7,
                        (product) =>
                        {
                            PopupConfirm.ShowOK(TextMan.Get("Congratulations"), TextMan.Get("You activated auto restore for 2 hour."), "Great", () =>
                            {
                                if (!GameManager.IsAutoRestorePlanet)
                                {
                                    DataGameSave.autoRestoreEndTime = GameManager.Now.Ticks;
                                }

                                DateTime newEndTime = new DateTime(DataGameSave.autoRestoreEndTime).AddHours(2);
                                DataGameSave.autoRestoreEndTime = newEndTime.Ticks;
                                DataGameSave.SaveToServer();
                            });
                        },
                        () =>
                        {
                            PopupConfirm.ShowOK("Error!!!", "Purchase failed, please try again.");
                        });
                    break;
                case TypeShopItem.PlanetCore:
                    if (DataGameSave.dataLocal.Diamond >= ShopData.Instance.AutoRestoreDiamondPrice)
                    {
                        DataGameSave.dataLocal.Diamond -= ShopData.Instance.AutoRestoreDiamondPrice;
                        DataGameSave.dataLocal.M_ColorfulStone += 10;

                        PopupConfirm.ShowOK("Item Purchased!", "You purchased 10 Planet Cores");
                    }
                    else
                    {
                        PopupConfirm.ShowOK("Purchased Failed!", "You don't have enough diamond");
                        OnChooseShop(true);
                    }
                    break;
                case TypeShopItem.AutoRestore:
                    IAPManager.Instance.Purchase(IAPItem.AutoRestore,
                     (product) =>
                     {
                         //TODO: change code autorestore here!
                         DataGameSave.dataLocal.Diamond += ShopData.Instance.DiamondValue[1];
                         TrackBuy("auto_restore", product);
                     },
                     () =>
                     {
                         PopupConfirm.ShowOK("Error!!!", "Purchase failed, please try again.");
                     });
                    break;
                case TypeShopItem.BattlePass:
                    if (DataGameSave.GetMetaData(MetaDataKey.BATTLE_PASS_VIP_ENABLED) == "1")
                    {
                        PopupConfirm.ShowOK(TextMan.Get("Information"), TextMan.Get("Battle Pass vip is already activated"));
                    }
                    else
                    {
                        BattlePass.BuyVipBattlePass();
                    }
                    break;
            }
        }

        SetItemButtonUI();
        //Sound
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);
        DataGameSave.SaveToServer();
    }

    //public void OnBuyShopTye(TypeShopItem itemType) {
    //    if (itemType == TypeShopItem.BattlePass) {

    //    }
    //}

    public static void TrackBuy(string productName, Product product)
    {
        ServerSystem.Instance.TrackBuy(productName, product);
        //        float gPrice = (float)product.metadata.localizedPrice * 0.7f;

        //        string gCurrency = product.metadata.localizedPriceString;


        //#if UNITY_ANDROID
        //        var googleReceipt = GooglePurchase.FromJson(product.receipt);
        //#endif
        //#if UNITY_IOS
        //        var appleReceipt = ApplePurchase.FromJson(product.receipt);
        //#endif
        //        Dictionary<string, string> addparam = new Dictionary<string, string>(){
        //                                { AFInAppEvents.CONTENT_ID, product.definition.id},
        //                                { AFInAppEvents.REVENUE, gPrice.ToString() },
        //                                { AFInAppEvents.CURRENCY, gCurrency}
        //                            };



        //#if UNITY_ANDROID
        //        AppsFlyerAndroid.validateAndSendInAppPurchase(
        //        inappAndroidPublicKey,
        //        googleReceipt.PayloadData.signature,
        //        googleReceipt.PayloadData.json,
        //        gPrice.ToString(),
        //        gCurrency,
        //        addparam,
        //        this);
        //#endif

        //#if UNITY_IOS && !UNITY_EDITOR
        //            AppsFlyeriOS.validateAndSendInAppPurchase(
        //            product.definition.id,
        //            gPrice.ToString(),
        //            product.metadata.isoCurrencyCode,
        //            product.transactionID,
        //            addparam,
        //            this);
        //#endif
    }

    public void OnPressed_BuyAutoRestore()
    {
        //IAPManager.Instance.Purchase(IAPItem.AutoRestore,
        //          () =>
        //          {
        //              PopupConfirm.ShowOK(TextMan.Get("Item Purchased!!!"), TextMan.Get("You activated auto restore for 2 hour."));
        //              GameManager.IsAutoRestorePlanet = true;
        //              Utilities.SetPlayerPrefsBool("isAutoRestoreHours24h", true);
        //          },
        //          () =>
        //          {
        //              PopupConfirm.ShowOK("Error!!!", "Purchase failed, please try again.");
        //          });
    }

    public void OnPressed_WatchToAutoRestore5Minutes()
    {
        int count = int.Parse(DataGameSave.GetMetaData(MetaDataKey.SHOP_AUTO_RESTORE_COUNT));
        if (count <= 0)
        {
            PopupConfirm.ShowOK("Oops", TextMan.Get("You reach watch ads limit for today"));
            return;
        }

        count--;
        DataGameSave.SaveMetaData(MetaDataKey.SHOP_AUTO_RESTORE_COUNT, count.ToString());

        GoogleMobileAdsManager.Instance.ShowRewardedVideo(() =>
        {
            string message = string.Format(TextMan.Get("You activated auto restore for {0} mins."), GameManager.AUTO_RESTORE_WATCH_ADS_MINUTE);
            PopupConfirm.ShowOK(TextMan.Get("Congratulations"), message, "Great", () =>
            {
                if (!GameManager.IsAutoRestorePlanet)
                {
                    DataGameSave.autoRestoreEndTime = GameManager.Now.Ticks;
                }

                DateTime newEndTime = new DateTime(DataGameSave.autoRestoreEndTime).AddMinutes(GameManager.AUTO_RESTORE_WATCH_ADS_MINUTE);
                DataGameSave.autoRestoreEndTime = newEndTime.Ticks;
                DataGameSave.SaveToServer();
            });
        });
    }

    public void OnPressed_FreeAutoRestore5Minutes()
    {
        DataGameSave.SaveMetaData(MetaDataKey.SHOP_AUTO_RESTORE_COUNT, GameManager.MAX_WATCH_ADS_PER_DAY_LIMIT.ToString());

        PopupConfirm.ShowOK(TextMan.Get("Congratulations"), TextMan.Get("You activated auto restore for 5 mins."), "Great", () =>
        {
            if (!GameManager.IsAutoRestorePlanet)
            {
                DataGameSave.autoRestoreEndTime = GameManager.Now.Ticks;
            }

            DateTime newEndTime = new DateTime(DataGameSave.autoRestoreEndTime).AddMinutes(5);
            DataGameSave.autoRestoreEndTime = newEndTime.Ticks;
            DataGameSave.SaveToServer();
        });
    }

    public void Onclose()
    {
        Disappear();
        EazySoundManager.PlaySound(Sounds.Instance.ButtonClick);

    }

    public void CheckAndBuy10RandomStones()
    {
        if (DataGameSave.dataLocal.Diamond < 100)
            return;

        int _air = 0;
        int _gravity = 0;
        int _fire = 0;
        int _anti = 0;
        int _ice = 0;

        for (int i = 0; i < 10; i++)
        {
            var t = GetRandomStone();

            if (t == TypePlanet.Air)
                _air++;
            else if (t == TypePlanet.Fire)
                _fire++;
            else if (t == TypePlanet.Ice)
                _ice++;
            else if (t == TypePlanet.Gravity)
                _gravity++;
            else if (t == TypePlanet.Antimatter)
                _anti++;
        }

        PopupBattleResult2.Show(
            reward: new DataReward
            {
                material = 0,
                air = _air,
                fire = _fire,
                ice = _ice,
                gravity = _gravity,
                antimater = _anti
            },

            okFunction: () =>
            {
                DataGameSave.dataLocal.M_IceStone += _ice;
                DataGameSave.dataLocal.M_FireStone += _fire;
                DataGameSave.dataLocal.M_AirStone += _air;
                DataGameSave.dataLocal.M_GravityStone += _gravity;
                DataGameSave.dataLocal.M_AntimatterStone += _anti;

                DataGameSave.dataLocal.Diamond -= 100;

                DataGameSave.SaveToLocal();
                DataGameSave.SaveToServer();
            });
    }

    public void CheckAndBuySuccessRandom10Avatars()
    {
        SkinDataReader.TryBuyRandomSkinPlanet(10, 2000);
    }

    public void OnPressed_WatchToGet30Diamonds()
    {
        int count = int.Parse(DataGameSave.GetMetaData(MetaDataKey.SHOP_GET_DIAMOND_COUNT));
        if (count <= 0)
        {
            PopupConfirm.ShowOK("Oops", TextMan.Get("You reach watch ads limit for today"));
            return;
        }

        count--;
        DataGameSave.SaveMetaData(MetaDataKey.SHOP_GET_DIAMOND_COUNT, count.ToString());

        GoogleMobileAdsManager.Instance.ShowRewardedVideo(() =>
        {
            PopupConfirm.ShowOK(TextMan.Get("Congratulations"), TextMan.Get("Your reward") + "\n" + 50, "Great", () =>
            {
                DataGameSave.dataLocal.Diamond += 50;
                DataGameSave.SaveToServer();
            });

            PopupConfirm.ShowMatImg(PopupConfirm._Instance.diamondImg.sprite);
            DataGameSave.SaveToServer();
        });
    }

    public void OnPressed_Free30Diamonds()
    {
        DataGameSave.SaveMetaData(MetaDataKey.SHOP_GET_DIAMOND_COUNT, GameManager.MAX_WATCH_ADS_PER_DAY_LIMIT.ToString());
        UpdateWatchAdsButtonDisplay();

        PopupConfirm.ShowOK(TextMan.Get("Congratulations"), TextMan.Get("Your reward") + "\n" + 50, "Great", () =>
        {
            DataGameSave.dataLocal.Diamond += 50;
        });

        PopupConfirm.ShowMatImg(PopupConfirm._Instance.diamondImg.sprite);
        DataGameSave.SaveToServer();
    }

    public void OnPressed_WatchToAutoShootReset()
    {
        int count = int.Parse(DataGameSave.GetMetaData(MetaDataKey.SHOP_TOTAL_ATTACK_COUNT));
        if (count <= 0)
        {
            PopupConfirm.ShowOK("Oops", TextMan.Get("You reach watch ads limit for today"));
            return;
        }

        count--;
        DataGameSave.SaveMetaData(MetaDataKey.SHOP_TOTAL_ATTACK_COUNT, count.ToString());

        GoogleMobileAdsManager.Instance.ShowRewardedVideo(() =>
        {
            PopupConfirm.ShowOK(TextMan.Get("Congratulations"), TextMan.Get("Autoshoot Reset"), "Great", () =>
            {
                DataGameSave.autoShootCount += GameManager.MAX_AUTO_SHOOT_COUNT;
                DataGameSave.SaveToServer();
            });
            DataGameSave.SaveToServer();
        });
    }

    public void OnPressed_FreeAutoShootReset()
    {
        DataGameSave.SaveMetaData(MetaDataKey.SHOP_TOTAL_ATTACK_COUNT, GameManager.MAX_WATCH_ADS_PER_DAY_LIMIT.ToString());
        UpdateWatchAdsButtonDisplay();

        PopupConfirm.ShowOK(TextMan.Get("Congratulations"), TextMan.Get("Autoshoot Reset"), "Great", () =>
        {
            DataGameSave.autoShootCount += GameManager.MAX_AUTO_SHOOT_COUNT;
            DataGameSave.SaveToServer();
        });
        DataGameSave.SaveToServer();
    }

    public void CheckAndBuy10RandomEffectStones()
    {
        if (DataGameSave.dataLocal.Diamond < 500)
            return;

        DataReward reward = GetRewardRandomEffectStones(10);

        PopupBattleResult2.Show(
            reward: reward,

            okFunction: () =>
            {
                DataGameSave.dataLocal.M_Ice += reward.ice;
                DataGameSave.dataLocal.M_Fire += reward.fire;
                DataGameSave.dataLocal.M_Air += reward.air;
                DataGameSave.dataLocal.M_Gravity += reward.gravity;
                DataGameSave.dataLocal.M_Antimatter += reward.antimater;

                DataGameSave.dataLocal.Diamond -= 500;

                DataGameSave.SaveToLocal();
                DataGameSave.SaveToServer();
            });
    }

    public static DataReward GetRewardRandomEffectStones(int amount)
    {
        int _air = 0;
        int _gravity = 0;
        int _fire = 0;
        int _anti = 0;
        int _ice = 0;

        for (int i = 0; i < amount; i++)
        {
            var t = GetRandomStone();

            if (t == TypePlanet.Air)
                _air++;
            else if (t == TypePlanet.Fire)
                _fire++;
            else if (t == TypePlanet.Ice)
                _ice++;
            else if (t == TypePlanet.Gravity)
                _gravity++;
            else if (t == TypePlanet.Antimatter)
                _anti++;
        }

        return new DataReward()
        {
            isEffecStones = true,

            material = 0,
            air = _air,
            fire = _fire,
            ice = _ice,
            gravity = _gravity,
            antimater = _anti
        };
    }

    private static TypePlanet GetRandomStone()
    {
        return (TypePlanet)UnityEngine.Random.Range(0, (int)TypePlanet.Default);
    }
}

// The following classes are used to deserialize JSON results provided by IAP Service
// Please, note that Json fields are case-sensetive and should remain fields to support Unity Deserialization via JsonUtilities
public class JsonData
{
    // Json Fields, ! Case-sensetive

    public string orderId;
    public string packageName;
    public string productId;
    public long purchaseTime;
    public int purchaseState;
    public string purchaseToken;
}

public class PayloadData
{
    public JsonData JsonData;

    // Json Fields, ! Case-sensetive
    public string signature;
    public string json;

    public static PayloadData FromJson(string json)
    {
        var payload = JsonUtility.FromJson<PayloadData>(json);
        payload.JsonData = JsonUtility.FromJson<JsonData>(payload.json);
        return payload;
    }
}

public class GooglePurchase
{
    public PayloadData PayloadData;

    // Json Fields, ! Case-sensetive
    public string Store;
    public string TransactionID;
    public string Payload;

    public static GooglePurchase FromJson(string json)
    {
        var purchase = JsonUtility.FromJson<GooglePurchase>(json);
        purchase.PayloadData = PayloadData.FromJson(purchase.Payload);
        return purchase;
    }
}
public class ApplePurchase
{

    // Json Fields, ! Case-sensetive
    public string Store;
    public string TransactionID;
    public string Payload;

    public static ApplePurchase FromJson(string json)
    {
        var purchase = JsonUtility.FromJson<ApplePurchase>(json);
        return purchase;
    }
}