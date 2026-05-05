#if UNITY_PURCHASING
using UnityEngine;
using UnityEngine.Purchasing;

namespace MarioGame.Components.Monetization.IAP
{
    public sealed class UnityIapImpl : MonoBehaviour, IStoreListener
    {
        private IStoreController _controller;
        private IExtensionProvider _extensions;
        private IapManager _parent;

        public void Construct(IapManager parent)
        {
            _parent = parent;
        }

        public void InitializePurchasing()
        {
            if (_controller != null)
                return;

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProduct(ProductIds.ExtraLivesSmall, ProductType.Consumable);
            builder.AddProduct(ProductIds.CoinPackSmall, ProductType.Consumable);
            builder.AddProduct(ProductIds.UnlockAllLevels, ProductType.NonConsumable);

            UnityPurchasing.Initialize(this, builder);
        }

        public void Purchase(string productId)
        {
            if (_controller == null)
            {
                _parent.FailPurchase(productId, "IAP not initialized");
                return;
            }

            var product = _controller.products.WithID(productId);
            if (product == null || !product.availableToPurchase)
            {
                _parent.FailPurchase(productId, "Product not available");
                return;
            }

            _controller.InitiatePurchase(product);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _controller = controller;
            _extensions = extensions;
            _parent.MarkInitialized(true);
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            _parent.MarkInitialized(false);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            _parent.MarkInitialized(false);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            _parent.GrantEntitlement(e.purchasedProduct.definition.id);
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            _parent.FailPurchase(product.definition.id, failureReason.ToString());
        }
    }
}
#endif

