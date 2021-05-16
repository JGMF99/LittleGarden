using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentMenu : MonoBehaviour
{

    [SerializeField] List<ItemSlot> listItems;

    private List<Item> items;

    public void Setup(List<Item> items)
    {

        this.items = items;

        for(var i = 0; i < listItems.Count; i++)
        {
            if (i < items.Count)
                listItems[i].Setup(items[i]);
            else
                listItems[i].Setup(null);
        }


    }

}
