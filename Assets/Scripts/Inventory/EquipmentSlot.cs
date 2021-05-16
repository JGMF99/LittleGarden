using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour, IPointerDownHandler, IDropHandler
{

    [SerializeField] Image itemImg;

    public ItemType type;

    private Item item;

    public void Setup(Item item)
    {
        if(item != null)
        {
            itemImg.GetComponent<Image>().color = Color.white;
            itemImg.GetComponent<Image>().sprite = item.Base.Sprite;

            this.item = item;
        }
        else
        {
            itemImg.GetComponent<Image>().color = Color.clear;
            itemImg.GetComponent<Image>().sprite = null;

            this.item = null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");


        if(eventData.pointerDrag != null){

            ItemSlot itemSlot = eventData.pointerDrag.GetComponent<ItemSlot>();

            if(itemSlot.Item.Base.Type == type)
            {
                if(item != null)
                {
                    item.IsEquipped = false;
                }

                Setup(itemSlot.Item);

                Inventory.instance.ChangeSelectedCharacterEquipment(item, type);

                itemSlot.DroppedOnSlot = true;
            }

        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && item != null)
        {
            item.IsEquipped = false;

            Setup(null);

            Inventory.instance.ChangeSelectedCharacterEquipment(null, type);

            Inventory.instance.UpdateChestPieces();

        }
    }
}
