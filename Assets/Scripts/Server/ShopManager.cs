using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class CartProduct
{
    public string name;
    public int coinPrice;
    public int diamondPrice;
    public GameObject model;
    public bool isPurchased;
}

public class ShopManager : MonoBehaviour
{
    private List<ItemInstance> userInven;
    
    public void RequestInventory()
    {
        var request = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(request, GetInventorySuccess, OnError);
    }

    public void GetInventorySuccess(GetUserInventoryResult result)
    {
        userInven = result.Inventory;
        RequestDataToServer();
    }
    
    public void RequestDataToServer()
    {
        var request = new GetCatalogItemsRequest { CatalogVersion = "cart" };
        PlayFabClientAPI.GetCatalogItems(request, GetCatalogSuccess, OnError);
    }

    private void GetCatalogSuccess(GetCatalogItemsResult result)
    {
        List<CartProduct> products = new List<CartProduct>();
        
        foreach (CatalogItem item in result.Catalog)
        {
            var customData = JsonUtility.FromJson<Dictionary<string, string>>(item.CustomData);
            
            CartProduct product = new CartProduct()
            {
                name = item.DisplayName,
                coinPrice = Convert.ToInt32(item.VirtualCurrencyPrices["CN"]),
                diamondPrice = Convert.ToInt32(item.VirtualCurrencyPrices["DM"]),
                model = Resources.Load<GameObject>(""),
                isPurchased = CheckIsPurchased(item.DisplayName)
            };
            
            products.Add(product);
        }
        
        PrintProducts(products);
    }

    public void PrintProducts(List<CartProduct> products)
    {
        // UI에 연결
        foreach (CartProduct product in products)
        {
            Debug.LogFormat("아이템 {0} ({1}원) / 보유 여부 {2}", product.name, product.coinPrice, product.isPurchased);
        }
    }

    private bool CheckIsPurchased(string itemName)
    {
        // 인벤 리스트에서 검색 후 반환
        foreach (ItemInstance item in userInven)
            if (item.DisplayName == itemName) return true;

        return false;
    }

    private void OnError(PlayFabError error)
    {
        Debug.Log(error.ErrorMessage);
    }
}