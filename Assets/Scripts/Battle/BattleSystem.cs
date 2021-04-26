using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BattleState
{
    Start, PlayerAction, PlayerSkill, EnemyMove, Busy
}

public class BattleSystem : MonoBehaviour
{

    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHud playerHud;

    [SerializeField] BattleUnit enemyUnit;

    [SerializeField] BattleDialogBox dialogBox;

    BattleState state;

    int currentAction;
    int currentSkill;

    private void Start()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {

        playerUnit.Setup();
        playerHud.SetData(playerUnit.Character);

        enemyUnit.Setup();

        dialogBox.SetSkillNames(playerUnit.Character.Skills);

        yield return null;

        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;

        dialogBox.EnableActionSelector(true);
        dialogBox.EnablePlayerHud(true);
    }

    void PlayerSkill()
    {
        state = BattleState.PlayerSkill;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnablePlayerHud(false);
        dialogBox.EnableSkillSelector(true);
    }

    IEnumerator PerformPlayerSkill()
    {
        state = BattleState.Busy;

        var skill = playerUnit.Character.Skills[currentSkill];

        yield return new WaitForSeconds(1f);

        bool isFainted = enemyUnit.Character.TakeDamage(skill, playerUnit.Character);

        if (isFainted)
        {
            Debug.Log("Enemy has died");
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;

        var skill = enemyUnit.Character.GetRandomSkill();

        yield return new WaitForSeconds(1f);

        bool isFainted = playerUnit.Character.TakeDamage(skill, enemyUnit.Character);
        playerHud.UpdateHP();

        if (isFainted)
        {
            Debug.Log("Player has died");
        }
        else
        {
            PlayerAction();
        }
    }

    private void Update()
    {
        if(state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }else if(state == BattleState.PlayerSkill)
        {
            HandleSkillSelection();
        }
    }

 

    void HandleActionSelection()
    {

        if (Input.GetKeyDown(KeyCode.D) && currentAction < 3)
        {
            ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.A) && currentAction > 0)
        {
            --currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.S) && currentAction < 2)
        {
            currentAction += 2;
        }
        else if (Input.GetKeyDown(KeyCode.W) && currentAction > 1)
        {
            currentAction -= 2;
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if(currentAction == 0)
            {
                PlayerSkill();
            }
            else if(currentAction == 1){
                //Items
            }
            else if (currentAction == 2)
            {
                //Position
            }
            else if (currentAction == 3)
            {
                //Run
            }
        }
    }

    void HandleSkillSelection()
    {
        if (Input.GetKeyDown(KeyCode.D) && currentSkill < playerUnit.Character.Skills.Count - 1)
        {
            ++currentSkill;
        }
        else if (Input.GetKeyDown(KeyCode.A) && currentSkill > 0)
        {
            --currentSkill;
        }
        else if (Input.GetKeyDown(KeyCode.S) && currentSkill < playerUnit.Character.Skills.Count - 2)
        {
            currentSkill += 2;
        }
        else if (Input.GetKeyDown(KeyCode.W) && currentSkill > 1)
        {
            currentSkill -= 2;
        }

        dialogBox.UpdateSkillSelection(currentSkill, playerUnit.Character.Skills[currentSkill]);

        if (Input.GetKeyDown(KeyCode.E))
        {
            dialogBox.EnableSkillSelector(false);

            StartCoroutine(PerformPlayerSkill());
        }
    }
}
