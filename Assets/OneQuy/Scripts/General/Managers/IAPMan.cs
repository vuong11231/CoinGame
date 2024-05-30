#if SteveIAP

using System;
using UnityEngine;
using UnityEngine.Purchasing;

namespace SteveRogers
{
    /// <summary>
    /// main
    /// </summary>
    public partial class IAPMan : SingletonPersistentStatic<IAPMan>, IStoreListener
    {
        [Serializable]
        public class ItemStore
        {
            public string name;
            public ProductType type;
        }

        private static IStoreController m_StoreController;
        private static IExtensionProvider m_StoreExtensionProvider;

        [SerializeField]
        private ItemStore[] items = null;

        // unity api

        private void Start()
        {
            if (m_StoreController == null)
                InitializePurchasing();
        }

        public void InitializePurchasing()
        {
            if (IsInitialized())
                return;

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            if (items.IsNullOrEmpty())
                throw new Exception("iap is empty items");

            foreach (var ele in items)
                builder.AddProduct(ele.name, ele.type);

            UnityPurchasing.Initialize(this, builder);
        }


        private static bool IsInitialized()
        {
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            m_StoreController = controller;
            m_StoreExtensionProvider = extensions;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            throw new Exception("OnInitializeFailed: " + error);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            OnPurchasedSuccess.SafeCall(args.purchasedProduct.definition.id);
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            OnPurchasedFail.SafeCall(product.definition.id, failureReason);
            //productid.text = "[" + product.definition.id + "]" + " & [" + product.definition.storeSpecificId + "]";
        }

        // my api (public)

        public static Action<string> OnPurchasedSuccess { get; set; } = null;
        public static Action<string, PurchaseFailureReason> OnPurchasedFail { get; set; } = null;

        public static void Buy(string productId)
        {
            if (IsInitialized())
            {
                Product product = m_StoreController.products.WithID(productId);

                if (product != null && product.availableToPurchase)
                    m_StoreController.InitiatePurchase(product);
                else
                    throw new Exception("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase: " + productId);
            }
            else
                throw new Exception("BuyProductID FAIL. Not initialized: " + productId);
        }
        public static string GetPrice(string productId)
        {
            if (IsInitialized())
            {
                Product product = m_StoreController.products.WithID(productId);
                return product?.metadata?.localizedPriceString;
            }
            else
                return null;
        }
    }
}
#endif // SteveIAP