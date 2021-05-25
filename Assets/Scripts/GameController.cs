using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState { FreeRoam, Battle, Dialog, Inventory, PopUp}
public class GameController : MonoBehaviour
{

    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;

    public GameState state;

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

                if(DialogManager.Instance.Quest != null && DialogManager.Instance.Quest.quest.questState == QuestState.notStarted)
                {
                    DialogManager.Instance.Quest.OpenQuestWindow();

                    DialogManager.Instance.Quest.OnQuestWindowClose += () =>
                    {
                        state = GameState.FreeRoam;
                    };
                }
                else
                {
                    state = GameState.FreeRoam;
                }

                
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
        AudioManager.instance.StopPlaying("RoamMusic");
        AudioManager.instance.Play("BattleMusic");

        battleSystem.StartBattle(team,enemy, playerController.items);
    }

    private void EndBattle(bool obj, EnemyParty enemies)
    {
        state = GameState.FreeRoam; 
        AudioManager.instance.StopPlaying("BattleMusic");
        AudioManager.instance.Play("RoamMusic");

        playerController.CheckKillQuests(enemies);

        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    private void OpenInventory()
    {
        var team = playerController.GetComponent<CharacterParty>();
        var items = playerController.Items;
        var quests = playerController.Quests;

        bool isOpen = InventoryUI.Instance.Open(team,items, quests);

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
