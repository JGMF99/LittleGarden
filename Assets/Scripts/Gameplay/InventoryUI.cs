using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] GameObject Inventory;

    public void Open()
    {
        if (!Inventory.active)
        {
            Inventory.SetActive(true);
        }
        else
        {
            Inventory.SetActive(false);
        }
       
    }
}
