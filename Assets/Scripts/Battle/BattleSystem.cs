using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum BattleState
{
    Start, PlayerAction, PlayerSkill, TargetEnemy, EnemyMove, Busy
}

public class BattleSystem : MonoBehaviour
{

    
    BattleUnit playerUnit;
    [SerializeField] List<BattleUnit> playerGrid;
    BattleUnit enemyUnit;
    [SerializeField] List<BattleUnit> enemyGrid;

    [SerializeField] BattleDialogBox dialogBox;

    public event Action<bool> OnBattleOver;

    BattleState state;

    int currentAction;
    int currentSkill;
    int currentTarget;

    CharacterParty team;
    EnemyParty enemies;

    List<BattleUnit> playerUnits;
    List<BattleUnit> enemyUnits;
    List<BattleUnit> turnsQueue;


    public void StartBattle(CharacterParty team, EnemyParty enemies)
    {
        this.team = team;
        this.enemies = enemies;
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {

        foreach (var p in playerGrid)
            p.Reset();
        foreach (var e in enemyGrid)
            e.Reset();


        playerUnits = new List<BattleUnit>();
        enemyUnits = new List<BattleUnit>();
        turnsQueue = new List<BattleUnit>();

        foreach(var c in team.Team)
        {
            playerGrid[c.Position].Setup(c);

            turnsQueue.Add(playerGrid[c.Position]);
            playerUnits.Add(playerGrid[c.Position]);
        }

        //TODO - Player stats

        foreach (var e in enemies.Team)
        {
            enemyGrid[e.Position].Setup(e);

            turnsQueue.Add(enemyGrid[e.Position]);
            enemyUnits.Add(enemyGrid[e.Position]);
        }

        turnsQueue = turnsQueue.OrderByDescending(o => o.Character.Speed).ToList();

        yield return null;

        DecideNextTurn();

    }

    void DecideNextTurn()
    {
        var nextTurn = turnsQueue[0];

        turnsQueue.Remove(nextTurn);
        turnsQueue.Add(nextTurn);

        if (nextTurn.IsPlayerUnit)
        {
            dialogBox.SetSkillNames(nextTurn.Character.Skills);
            dialogBox.SetEnemyImages(enemyUnits);
            playerUnit = nextTurn;
            PlayerAction();
        }
        else
        {
            enemyUnit = nextTurn;
            StartCoroutine(EnemyMove());
        }
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;

        dialogBox.EnableActionSelector(true);
    }

    void PlayerSkill()
    {
        state = BattleState.PlayerSkill;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableSkillSelector(true);
        dialogBox.EnableSkillDetails(true);
    }

    void TargetEnemy()
    {
        state = BattleState.TargetEnemy;

        dialogBox.EnableSkillDetails(false);
        dialogBox.EnableEnemySelector(true);
    }

    IEnumerator PerformPlayerSkill()
    {
        state = BattleState.Busy;

        var skill = playerUnits[0].Character.Skills[currentSkill];

        yield return new WaitForSeconds(1f);

        bool isFainted = enemyUnit.Character.TakeDamage(skill, playerUnit.Character);

        enemyUnit.UpdateHP();

        if (isFainted)
        {
            Debug.Log("Enemy has died");

            yield return new WaitForSeconds(2f);

            enemyUnits.Remove(enemyUnit);
            turnsQueue.Remove(enemyUnit);
            enemyGrid[enemyUnit.Character.Position].BattleUnitDied();

        }

        if (enemyUnits.Count == 0)
        {
            OnBattleOver(true);
        }

        else
        {
            DecideNextTurn();
        }

    }

    IEnumerator EnemyMove()
    {
        Debug.Log("Enemy " + enemyUnit.Character.Base.Name + " attacked");

        state = BattleState.EnemyMove;

        var skill = enemyUnit.Character.GetRandomSkill();

        yield return new WaitForSeconds(1f);

        var playerUnitAttacked = playerUnits[Random.Range(0, playerUnits.Count)];

        bool isFainted = playerUnitAttacked.Character.TakeDamage(skill, enemyUnit.Character);

        playerUnitAttacked.UpdateHP();

        if (isFainted)
        {
            Debug.Log("Player has died");
            yield return new WaitForSeconds(2f);
            playerUnits.Remove(playerUnitAttacked);
            turnsQueue.Remove(playerUnitAttacked);
            playerGrid[playerUnitAttacked.Character.Position].BattleUnitDied();

        }


        if (playerUnits.Count == 0)
        {
            OnBattleOver(false);
        }

        else
        {
            DecideNextTurn();
        }
    }

    public void HandleUpdate()
    {
        if(state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }else if(state == BattleState.PlayerSkill)
        {
            HandleSkillSelection();
        }
        else if (state == BattleState.TargetEnemy)
        {
            HandleTargetEnemy();
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
            

            TargetEnemy();

            //StartCoroutine(PerformPlayerSkill());
        }
    }

    void HandleTargetEnemy()
    {
        if (Input.GetKeyDown(KeyCode.D) && currentTarget < enemies.Team.Count - 1)
        {
            ++currentTarget;
        }
        else if (Input.GetKeyDown(KeyCode.A) && currentTarget > 0)
        {
            --currentTarget;
        }

        dialogBox.UpdateEnemySelection(currentTarget);

        if (Input.GetKeyDown(KeyCode.E))
        {
            dialogBox.EnableSkillSelector(false);
            dialogBox.EnableEnemySelector(false);

            enemyUnit = enemyUnits.Where(x => x.Character == dialogBox.EnemyImages[currentTarget].Character).FirstOrDefault();

            StartCoroutine(PerformPlayerSkill());
        }

    }
}
