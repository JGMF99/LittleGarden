using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellingItem : MonoBehaviour
{

    [SerializeField] ItemBase item;
    [SerializeField] int price;

    [SerializeField] new Text name;
    [SerializeField] Image img;
    [SerializeField] Text cost;

    [SerializeField] Button buy;

    private ShopPanel shopPanel;

    internal void Setup(PlayerController playerController, ShopPanel shopPanel)
    {
        name.text = item.Name;
        img.sprite = item.Sprite;
        cost.text = price.ToString();

        this.shopPanel = shopPanel;

        buy.onClick.AddListener(() => OnBuyClickListener(playerController));
    }

    private void OnBuyClickListener(PlayerController playerController)
    {
        if(price <= playerController.Money)
        {
            playerController.Money -= price;

            playerController.Items.Add(new Item(item, false));

            shopPanel.UpdateMoneyText(playerController.Money);
        }
    }
}
