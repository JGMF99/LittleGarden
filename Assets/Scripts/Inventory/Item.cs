using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Item
{

    [SerializeField] ItemBase _base;

    private bool isEquipped = false;

    public Item(ItemBase @base, bool isEquipped)
    {
        _base = @base;
        this.isEquipped = isEquipped;
    }

    public bool IsEquipped { get => isEquipped; set => isEquipped = value; }
    public ItemBase Base { get => _base; set => _base = value; }

}
