using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemReceived : MonoBehaviour
{

    [SerializeField] Image img;
    [SerializeField] Text nameAndQuantity;

    internal void Setup(ItemBase item, int quantity)
    {
        if(item != null)
        {
            gameObject.SetActive(true);

            img.GetComponent<Image>().sprite = item.Sprite;

            nameAndQuantity.text = item.name + " x" + quantity;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
