using UnityEngine;
using UnityEngine.Purchasing;
using System;
using System.Collections.Generic;

public enum IAPItem
{
    Diamond_1,
    Diamond_2,
    Diamond_3,
    Diamond_4,
    Diamond_5,
    Diamond_6,
    Diamond_7,
    AutoRestore,
    BattlePass,
    X2Material,
    _1h = 17,
    _6h = 18,
    _12h = 19,
    _1d = 20,
    _2d = 21,
    _3d = 22,
    _7d = 23,
    _x2_material_ios = 24
}

public class IAPManager : Singleton<IAPManager>, IStoreListener
{
    public bool IsDebug = true;

    public string[] IDs_Consumable;
    public string[] IDs_Consumable_Ios;
    public string[] IDs_NonComsumable;
    public float[] Prices_Comsumable;
    public float[] Prices_NonConsumable;
    public string[] IDs_Subcrible;

    IAPItem GetType(string id)
    {
        // Only Non-consumeable items
        if (id == IDs_NonComsumable[0]) return IAPItem.Diamond_1;

        return IAPItem.Diamond_1;
    }


    string GetID(IAPItem item)
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (item == IAPItem._x2_material_ios)
            {
                return IDs_Subcrible[8];
            }
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (item >= IAPItem._1h)
            {
                return IDs_Subcrible[(int)item - 17];
            }
            else
            {
                return IDs_Consumable_Ios[(int)item];
            }
        } else
            return IDs_Consumable[(int)item];
    }

    //
    //
    static IStoreController m_StoreController;
    static IExtensionProvider m_StoreExtensionProvider;

    bool _IsRestore;
    bool _IsRunRestoreCoroutine;
    List<IAPItem> _RestoreIDs;

    Action<Product> _PurchaseSuccess;
    Action _PurchaseFail;

    Action<IAPItem[]> _RestoreSuccess;
    Action _RestoreFail;
    Action _RestoreNothing;

    void Start()
    {
        if (m_StoreController == null)
        {
            InitializePurchasing();

            _RestoreIDs = new List<IAPItem>();
        }
    }

    #region Initalize
    void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            // Consumable
            for (int i = 0; i < IDs_Consumable_Ios.Length; i++)
            {
                builder.AddProduct(IDs_Consumable_Ios[i], ProductType.Consumable);
            }
        } else
        {
            // Consumable
            for (int i = 0; i < IDs_Consumable.Length; i++)
            {
                builder.AddProduct(IDs_Consumable[i], ProductType.Consumable);
            }
        }
        
        // Non-consumable
        for (int i = 0; i < IDs_NonComsumable.Length; i++)
        {
            builder.AddProduct(IDs_NonComsumable[i], ProductType.NonConsumable);
        }

        // Subcrible
        for (int i = 0; i < IDs_Subcrible.Length; i++)
        {
            builder.AddProduct(IDs_Subcrible[i], ProductType.Subscription);
        }

        // Initalize
        UnityPurchasing.Initialize(this, builder);
    }

    bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        //if (IsDebug) Debug.LogError("IAPManager: OnInitialized");
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        if (IsDebug) Debug.LogError("IAPManager: OnInitializeFailed InitializationFailureReason:" + error);
    }
    #endregion

    #region Purchase
    public void Purchase(IAPItem item, Action<Product> purchaseSuccess, Action purchaseFail)
    {
        string id = GetID(item);

        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError("IAPManager: Fail Development Here");
        }
        else
        {
#if !TEST
            PopupConfirm.ShowLoading("", "Please wait!");
            _PurchaseSuccess = purchaseSuccess;
            _PurchaseFail = purchaseFail;

            PurchaseID(id);

            // Analytics
            // Add To Cart
            //AnalyticsManager.Instance.TrackIapAddToCart(
            //idProduct,
            //iapName,
            //type,
            //price,
            //isFromShop);
#else
            purchaseSuccess.Invoke();
#endif
        }
    }

    public void Purchase(string item, Action<Product> purchaseSuccess, Action purchaseFail)
    {
        string id = item;

        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError("IAPManager: Fail Development Here");
        }
        else
        {
#if !TEST
            PopupConfirm.ShowLoading("", "Please wait!");
            _PurchaseSuccess = purchaseSuccess;
            _PurchaseFail = purchaseFail;

            PurchaseID(id);

            // Analytics
            // Add To Cart
            //AnalyticsManager.Instance.TrackIapAddToCart(
            //idProduct,
            //iapName,
            //type,
            //price,
            //isFromShop);
#else
            purchaseSuccess.Invoke();
#endif
        }
    }

    void PurchaseID(string iapID)
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(iapID);
            if (product != null && product.availableToPurchase)
            {
                if (IsDebug) Debug.LogError(string.Format("IAPManager: Purchasing product asychronously: '{0}'", product.definition.id));

                m_StoreController.InitiatePurchase(product);
                PopupConfirm.HideLoading();

                // Analytics
                // Start Checkout
                //AnalyticsManager.Instance.TrackIapStartToCheckOut(
                //productId,
                //iapName,
                //type,
                //price,
                //isFromShop);
            }
            else
            {
                PopupConfirm.HideLoading();
                Debug.LogError("IAPManager: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            PopupConfirm.HideLoading();
            Debug.LogError("IAPManager: FAIL. Not initialized.");
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (!_IsRestore)
        {
            if (IsDebug) Debug.LogError("IAPManager: Purchase Success -> args.purchasedProduct.definition.id");

            PopupConfirm.HideLoading();
            _PurchaseSuccess.Invoke(args.purchasedProduct);
        }
        else
        {
            RestoreSuccess(args.purchasedProduct.definition.id);

            // Usual Call Pop-up
            if (!_IsRunRestoreCoroutine)
            {
                _IsRunRestoreCoroutine = true;

                LeanTween.delayedCall(0.25f, () =>
                {
                    _IsRunRestoreCoroutine = false;
                    _IsRestore = false;

                    _RestoreSuccess.Invoke(_RestoreIDs.ToArray());
                });
            }
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        var log = (string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));

        _PurchaseFail.Invoke();
    }

    #endregion

    #region Restore
    public void Restore(Action<IAPItem[]> restoreSuccess, Action restoreFail, Action restoreNothing)
    {
#if UNITY_IOS
        if (!IsInitialized())
        {
            if(IsDebug) Debug.LogError("IAPManager: RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            if (IsDebug) Debug.LogError("IAPManager: RestorePurchases started ...");

            // Clear all
            _RestoreIDs.Clear();

            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();

            apple.RestoreTransactions((result) =>
            {
                _IsRestore = false;

                if (result)
                {
                    // No previous purchases to restore, so disable Restore button and re-enable Purchase button
                    _RestoreNothing.Invoke();
                }
                else
                {
                    // Restore Fail Annoucement
                    _RestoreFail.Invoke();
                }
            });
        }
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            if(IsDebug) Debug.LogError("IAPManager: RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
#endif
    }

    void RestoreSuccess(string productID)
    {
        if (_IsRestore)
        {
            _RestoreIDs.Add(GetType(productID));
        }
    }
    #endregion
}
