using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState { FreeRoam, Battle, Dialog, Inventory}
public class GameController : MonoBehaviour
{

    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;

    GameState state;

    private void Start()
    {
        playerController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        playerController.OnInventoryOpen += OpenInventory;

        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };

        DialogManager.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
            {
                state = GameState.FreeRoam;
            }
        };
    }

    

    void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var team = playerController.GetComponent<CharacterParty>();
        var enemy = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomEnemy();

        battleSystem.StartBattle(team,enemy);
    }

    private void EndBattle(bool obj)
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    private void OpenInventory()
    {
        var team = playerController.GetComponent<CharacterParty>();
        var items = playerController.Items;

        bool isOpen = InventoryUI.Instance.Open(team,items);

        if (isOpen)
            state = GameState.Inventory;
        else
            state = GameState.FreeRoam;
    }

    private void Update()
    {
        
        if(state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();

        }
        else if(state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }else if(state == GameState.Inventory)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                OpenInventory();
            }
        }
    }

}
