using System;
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

    [SerializeField] Inventory teamInventory;
    [SerializeField] BattlePosition battlePosition;

    public bool Open(CharacterParty team, List<Item> playerItems)
    {
        if (!Inventory.activeSelf)
        {            
            teamInventory.Setup(team, playerItems);
            Debug.Log(battlePosition);
            battlePosition.Setup(team);

            Inventory.SetActive(true);
            return true;
        }
        else
        {
            Inventory.SetActive(false);
            return false;
        }
       
    }

}
