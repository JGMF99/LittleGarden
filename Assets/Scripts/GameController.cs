using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum GameState { FreeRoam, Battle, Dialog, Inventory, PopUp, Options}
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
        playerController.OnOptionsOpen += OpenOptions;

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
                else if(DialogManager.Instance.Recruitment != null)
                {
                    if (!DialogManager.Instance.Recruitment.recruitment.isDone)
                    {
                        StartRecruitmentBattle(DialogManager.Instance.Recruitment.recruitment, new EnemyParty
                        {
                            Team = DialogManager.Instance.Recruitment.recruitment.TeamToDefeat
                        });
                    }
                    else
                    {
                        DialogManager.Instance.Recruitment.OpenNewCharacterWindow(playerController);

                        DialogManager.Instance.Recruitment.OnNewCharacterWindowClose += () =>
                        {
                            state = GameState.FreeRoam;
                        };
                    }
                    
                }else if(DialogManager.Instance.Shop != null)
                {
                    DialogManager.Instance.Shop.ShopPanel.OpenShop(playerController);

                    DialogManager.Instance.Shop.ShopPanel.OnShopWindowClose += () =>
                    {
                        state = GameState.FreeRoam;
                    };
                }
                else if (DialogManager.Instance.Boss != null)
                {
                    StartBossBattle(new EnemyParty
                    {
                        Team = DialogManager.Instance.Boss.EnemyTeam
                    });
                    
                }
                else
                {
                    state = GameState.FreeRoam;
                }

                
            }
        };
    }

    void StartRecruitmentBattle(Recruitment recruitment, EnemyParty enemy)
    {
        var team = playerController.GetComponent<CharacterParty>();

        if(team.Team.Count > 0)
        {
            state = GameState.Battle;
            battleSystem.gameObject.SetActive(true);
            worldCamera.gameObject.SetActive(false);



            for (var i = 0; i < enemy.Team.Count; i++)
            {
                enemy.Team[i].Init();
            }

            enemy.Team = enemy.Team.OrderBy(o => o.Position).ToList();
            AudioManager.instance.StopPlaying("RoamMusic");
            AudioManager.instance.Play("BattleMusic");


            battleSystem.StartBattle(team, enemy, playerController.items, recruitment);
        }
        else
        {
            state = GameState.FreeRoam;
        }


    }

    void StartBossBattle(EnemyParty enemy)
    {
        var team = playerController.GetComponent<CharacterParty>();

        if (team.Team.Count > 0)
        {
            state = GameState.Battle;
            battleSystem.gameObject.SetActive(true);
            worldCamera.gameObject.SetActive(false);

            for (var i = 0; i < enemy.Team.Count; i++)
            {
                enemy.Team[i].Init();
            }

            enemy.Team = enemy.Team.OrderBy(o => o.Position).ToList();
            AudioManager.instance.StopPlaying("RoamMusic");
            AudioManager.instance.Play("BattleMusic");


            battleSystem.StartBattle(team, enemy, playerController.items, null);
        }
        else
        {
            state = GameState.FreeRoam;
        }


    }

    void StartBattle()
    {
        var team = playerController.GetComponent<CharacterParty>();

        if(team.Team.Count > 0)
        {
            state = GameState.Battle;
            battleSystem.gameObject.SetActive(true);
            worldCamera.gameObject.SetActive(false);


            var enemy = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomEnemy();
            AudioManager.instance.StopPlaying("RoamMusic");
            AudioManager.instance.Play("BattleMusic");

            battleSystem.StartBattle(team, enemy, playerController.items, null);
        }
 
    }

    private void EndBattle(bool obj, EnemyParty enemies, Recruitment recruitment)
    {
        state = GameState.FreeRoam; 
        AudioManager.instance.StopPlaying("BattleMusic");
        AudioManager.instance.Play("RoamMusic");

        playerController.CheckKillQuests(enemies);

        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);

        if (obj && recruitment != null)
            playerController.CompletedRecruitment(recruitment);
            //Debug.Log($"Recruitment battle won! {recruitment.Character.Name} is now available!");
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

    private void OpenOptions()
    {
        bool isOpen = OptionsMenu.instance.Open();

        if (isOpen)
            state = GameState.Options;
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
        }
        else if(state == GameState.Inventory)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                OpenInventory();
            }
        }
        else if (state == GameState.Options)
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                OpenOptions();
            }
        }
    }

}
