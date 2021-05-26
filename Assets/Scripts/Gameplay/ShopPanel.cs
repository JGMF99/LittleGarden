using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : MonoBehaviour
{

    [SerializeField] Text moneyTxt;
    [SerializeField] Button closeBtn;

    [SerializeField] List<SellingItem> items;

    public event Action OnShopWindowClose;

    public void OpenShop(PlayerController playerController)
    {
        gameObject.SetActive(true);

        UpdateMoneyText(playerController.Money);

        foreach (SellingItem si in items)
            si.Setup(playerController, this);

        closeBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.AddListener(() => OnShopWindowClose.Invoke());
    }

    public void UpdateMoneyText(int money)
    {
        moneyTxt.text = money.ToString();
    }

}
