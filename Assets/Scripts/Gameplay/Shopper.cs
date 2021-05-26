using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shopper : MonoBehaviour
{

    [SerializeField] List<SellingItem> items;

    [SerializeField] ShopPanel shopPanel;

    public ShopPanel ShopPanel { get => shopPanel; set => shopPanel = value; }
}
