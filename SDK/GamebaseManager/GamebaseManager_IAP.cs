using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Framework.Core;
using Framework.UI;
using Toast.Gamebase;
using UnityEngine;

public partial class GamebaseManager : SingletonMonoDontDestroyBehaviour<GamebaseManager>
{
    // 소비되지 않은 구매 체크.
    public void CheckNotConsumed(Action<bool, List<GamebaseResponse.Purchase.PurchasableReceipt>> callback, bool allStores = true)
    {
        var configuration = new GamebaseRequest.Purchase.PurchasableConfiguration
        {
            allStores = allStores
        };
        
        Gamebase.Purchase.RequestItemListOfNotConsumed(configuration, (purchasableReceiptList, error) =>
        {
            if (Gamebase.IsSuccess(error))
            {
                GameLog.Log("GamebaseManager::CheckNotConsumed - Get list succeeded." + purchasableReceiptList.Count);
                callback?.Invoke(true, purchasableReceiptList);
                // TODO : Send Server.
                // Should Deal With This non-consumed Items.
                // Send this item list to the game(item) server for consuming item.
            }
            else
            {
                callback?.Invoke(false,null);
                if(error != null)
                    GameLog.Log($"RequestItemListOfNotConsumed failed. error is {error.message}");
                
            }
        });
    }
    
    // 아이템 구매 요청.
    public void RequestPurchase(string gamebaseProductId, Action<int, string, string, string> callback)
    {
        Gamebase.Purchase.RequestPurchase(gamebaseProductId, (purchasableReceipt, error) =>
            {
                var resultCode = Gamebase.IsSuccess(error) ? GamebaseErrorCode.SUCCESS : error.code;
                callback?.Invoke(resultCode, purchasableReceipt?.storeCode??"", purchasableReceipt?.paymentSeq??"", purchasableReceipt?.purchaseToken??"");
            });
    }
    // 구매 가능한 아이템 목록 요청.
    public void RequestItemListPurchasable(Action<List<GamebaseResponse.Purchase.PurchasableItem>> callback)
    {
        Gamebase.Purchase.RequestItemListPurchasable((purchasableItemList, error) =>
        {
            callback?.Invoke(purchasableItemList);
            if (Gamebase.IsSuccess(error))
            {
                GameLog.Log("GamebaseManager::RequestItemListPurchasable - succeeded.");
            }
            else
            {
                if(error != null)
                    GameLog.Log($"GamebaseManager::RequestItemListPurchasable - failed. {error.message}");
            }
        });
    }

}
