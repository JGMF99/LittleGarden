using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum BattleState
{
    Start, PlayerAction, PlayerSkill, TargetEnemy, PlayerPosition, EnemyMove, Busy
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
    int currentPosition;

    CharacterParty team;
    EnemyParty enemies;

    BattleUnit nextTurn;
    int alphaCheck = 0;

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
        nextTurn = turnsQueue[0];

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

        currentAction = 0;
    }

    void PlayerSkill()
    {
        state = BattleState.PlayerSkill;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableSkillSelector(true);
        dialogBox.EnableSkillDetails(true);

        currentSkill = 0;
    }

    void RunAway()
    {
        state = BattleState.Busy;

        var runSuccessful = Random.Range(0, 101) < 80;

        if (runSuccessful)
            OnBattleOver(false);
        else
            DecideNextTurn();

    }

    void TargetEnemy()
    {
        state = BattleState.TargetEnemy;

        dialogBox.EnableSkillDetails(false);
        dialogBox.EnableEnemySelector(true);

        currentTarget = 0;
    }

    void PlayerPosition()
    {
        state = BattleState.PlayerPosition;

        if (playerUnit.Character.Position != 0)
            currentPosition = 0;
        else
            currentPosition = 1;
    }

    IEnumerator PerformPlayerSkill()
    {
        state = BattleState.Busy;

        var skill = playerUnit.Character.Skills[currentSkill];

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

        yield return new WaitForSeconds(3f);

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

            team.Team.Remove(playerUnitAttacked.Character); //Character dies forever
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



    public List<BattleUnit> Swap(List<BattleUnit> list, int indexA, int indexB)
    {
        var tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
        return list;
    }


    IEnumerator SwitchPositions()
    {
        state = BattleState.Busy;

        var oldPosition = playerUnit.Character.Position;

        playerGrid[oldPosition].Swap(playerGrid[currentPosition], currentPosition);

        foreach (var pu in playerGrid)
        {
            pu.Setup(pu.Character);
        }

        if (playerGrid[oldPosition].Character == null)
        {
            playerUnits.Remove(playerGrid[oldPosition]);
            playerUnits.Add(playerGrid[currentPosition]);

            turnsQueue.Remove(playerGrid[oldPosition]);
            turnsQueue.Add(playerGrid[currentPosition]);

        }
        else
        {
            turnsQueue = Swap(turnsQueue, turnsQueue.IndexOf(playerGrid[currentPosition]), turnsQueue.IndexOf(playerGrid[oldPosition]));
        }


        yield return new WaitForSeconds(1f);

        DecideNextTurn();
    }

    

    public void HandleUpdate()
    {

        if(nextTurn != null)
        {
            HighlightCharacter();
        }

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
        }else if(state == BattleState.PlayerPosition)
        {
            HandlePlayerPosition();
        }

    }

    private void HighlightCharacter()
    {
        foreach (var bu in turnsQueue)
        {
            if (bu.Equals(nextTurn))
            {
                Color temp = bu.GetComponent<Image>().color;
                if (temp.a <= 0.4)
                    alphaCheck = 1;
                else if (temp.a >= 1)
                    alphaCheck = -1;
                temp.a += alphaCheck * Time.deltaTime;
                nextTurn.GetComponent<Image>().color = temp;
            }
            else
                bu.GetComponent<Image>().color = Color.white;
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

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.M))
        {
            if(currentAction == 0)
            {
                PlayerSkill();
            }
            else if(currentAction == 1){
                PlayerPosition();
            }
            else if (currentAction == 2)
            {
                //Items
            }
            else if (currentAction == 3)
            {
                RunAway();
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

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.M))
        {
            
            TargetEnemy();

        }
    }

    void HandleTargetEnemy()
    {
        if (Input.GetKeyDown(KeyCode.D) && currentTarget < enemyUnits.Count - 1)
        {
            ++currentTarget;
        }
        else if (Input.GetKeyDown(KeyCode.A) && currentTarget > 0)
        {
            --currentTarget;
        }

        dialogBox.UpdateEnemySelection(currentTarget);
        UpdateEnemySelection(currentTarget);

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.M))
        {
            dialogBox.EnableSkillSelector(false);
            dialogBox.EnableEnemySelector(false);

            enemyUnit = enemyUnits[currentTarget];

            StartCoroutine(PerformPlayerSkill());
        }

    }

    private void UpdateEnemySelection(int currentTarget)
    {
        for(int i = 0; i < enemyUnits.Count; i++)
        {
            if(i == currentTarget)
            {
                enemyGrid.Where(x => x.Character == enemyUnits[i].Character).FirstOrDefault().GetComponent<Image>().color = new Color(1f, 0, 0, 0.5f);
                Debug.Log(enemyGrid.Where(x => x.Character == enemyUnits[currentTarget].Character).FirstOrDefault().GetComponent<Image>().color);
            }
            else
            {
                enemyGrid.Where(x => x.Character == enemyUnits[i].Character).FirstOrDefault().GetComponent<Image>().color = new Color(1, 1, 1, 1f);
            }
        }
    }

    void HandlePlayerPosition()
    {

        if (Input.GetKeyDown(KeyCode.D) && currentPosition < playerGrid.Count - 1 && currentPosition + 1 != playerUnit.Character.Position)
        {
            ++currentPosition;
        }
        else if (Input.GetKeyDown(KeyCode.A) && currentPosition > 0 && currentPosition - 1 != playerUnit.Character.Position)
        {
            --currentPosition;
        }
        else if (Input.GetKeyDown(KeyCode.S) && currentPosition < playerGrid.Count - 2)
        {
            if(currentPosition + 2 == playerUnit.Character.Position)
            {
                if(currentPosition + 2 < playerGrid.Count)
                    currentPosition += 4;
            }else
                currentPosition += 2;
        }
        else if (Input.GetKeyDown(KeyCode.W) && currentPosition > 1)
        {
            if (currentPosition - 2 == playerUnit.Character.Position)
            {
                if (currentPosition - 2 > 0)
                    currentPosition -= 4;
            }
            else
                currentPosition -= 2;

        }

        UpdatePlayerPositionSelection();

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.M))
        {

            StartCoroutine(SwitchPositions());

        }
    }

    private void UpdatePlayerPositionSelection()
    {
        for(var i = 0; i < playerGrid.Count; i++)
        {
            if (i == currentPosition)
                playerGrid[i].GetComponent<Image>().color = new Color(0, 1, 0, 0.5f);
            else if(i == playerUnit.Character.Position)
                playerGrid[i].GetComponent<Image>().color = new Color(1, 0, 0, 0.5f);
            else if(playerGrid[i].Character != null)
                playerGrid[i].GetComponent<Image>().color = Color.white;
            else
                playerGrid[i].GetComponent<Image>().color = Color.clear;

        }
    }
}
