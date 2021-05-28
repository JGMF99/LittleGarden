using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentMenu : MonoBehaviour
{

    [SerializeField] List<ItemSlot> listItems;

    private List<Item> items;

    public void Setup(List<Item> items, int indexStart)
    {

        this.items = items;

        for(var i = 0; i < listItems.Count; i++)
        {
            if (i + indexStart < items.Count)
                listItems[i].Setup(items[i + indexStart]);
            else
                listItems[i].Setup(null);
        }


    }

}
