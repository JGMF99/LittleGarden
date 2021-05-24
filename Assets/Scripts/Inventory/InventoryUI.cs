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
    [SerializeField] QuestPanel questPanel;

    public bool Open(CharacterParty team, List<Item> playerItems, List<Quest> quests)
    {
        if (!Inventory.activeSelf)
        {            
            teamInventory.Setup(team, playerItems);
            battlePosition.Setup(team);
            questPanel.Setup(quests);

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
