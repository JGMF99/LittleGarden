using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleItem : MonoBehaviour
{
    [SerializeField] ItemBase itemBase;
    private List<Item> items = new List<Item>();

    [SerializeField] Text quantityTxt;
    [SerializeField] Image image;

    public ItemBase ItemBase { get => itemBase; set => itemBase = value; }
    public Image Image { get => image; set => image = value; }
    public List<Item> Items { get => items; set => items = value; }
    public Text QuantityTxt { get => quantityTxt; set => quantityTxt = value; }
}
