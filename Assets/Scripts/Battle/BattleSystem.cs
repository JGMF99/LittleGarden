using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum BattleState
{
    Start, PlayerAction, PlayerSkill, TargetEnemy, PlayerPosition, PlayerItem, EnemyMove, Busy
}

public class BattleSystem : MonoBehaviour
{

    
    BattleUnit playerUnit;
    [SerializeField] List<BattleUnit> playerGrid;
    BattleUnit enemyUnit;
    [SerializeField] List<BattleUnit> enemyGrid;

    [SerializeField] BattleDialogBox dialogBox;

    public event Action<bool, EnemyParty> OnBattleOver;

    BattleState state;

    int currentAction;
    int currentSkill;
    int currentTarget;
    int currentPosition;
    int currentItem;

    Skill selectedSkill;

    CharacterParty team;
    EnemyParty enemies;

    BattleUnit nextTurn;
    int alphaCheck = 0;

    List<BattleUnit> playerUnits;
    List<BattleUnit> enemyUnits;
    List<BattleUnit> turnsQueue;

    List<Item> playerItems;

    public void StartBattle(CharacterParty team, EnemyParty enemies, List<Item> items)
    {
        this.team = team;
        this.enemies = enemies;
        playerItems = items;
        

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

        foreach (BattleItem bi in dialogBox.BattleItems)
            bi.Items = new List<Item>();

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


        foreach (Item i in playerItems)
        {
            if(i.Base.Type == ItemType.General)
            {
                BattleItem battleItem = dialogBox.BattleItems.Where(bi => bi.ItemBase == i.Base).FirstOrDefault();

                if (battleItem != null)
                {
                    battleItem.Items.Add(i);
                }
                    
            }
        }

        ButtonListeners();

        yield return null;

        DecideNextTurn();

    }

    public void ButtonListeners()
    {
        dialogBox.ActionTexts[0].GetComponent<Button>().onClick.RemoveAllListeners();
        dialogBox.ActionTexts[0].GetComponent<Button>().onClick.AddListener(PlayerSkill);
        dialogBox.ActionTexts[1].GetComponent<Button>().onClick.RemoveAllListeners();
        dialogBox.ActionTexts[1].GetComponent<Button>().onClick.AddListener(PlayerPosition);
        dialogBox.ActionTexts[2].GetComponent<Button>().onClick.RemoveAllListeners();
        dialogBox.ActionTexts[2].GetComponent<Button>().onClick.AddListener(PlayerItem);
        dialogBox.ActionTexts[3].GetComponent<Button>().onClick.RemoveAllListeners();
        dialogBox.ActionTexts[3].GetComponent<Button>().onClick.AddListener(RunAway);

        foreach (BattleItem bi in dialogBox.BattleItems)
        {
            bi.GetComponent<Button>().onClick.RemoveAllListeners();
            bi.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(UseItem(bi)));
        }
            
    }

    void DecideNextTurn()
    {
        nextTurn = turnsQueue[0];

        turnsQueue.Remove(nextTurn);
        turnsQueue.Add(nextTurn);

        if (nextTurn.IsPlayerUnit)
        {
            dialogBox.SetSkillNames(nextTurn.Character.Skills);
            for(var i = 0; i < dialogBox.SkillText.Count; i++)
            {
                dialogBox.SkillText[i].GetComponent<Button>().onClick.RemoveAllListeners();
                var i2 = i;
                if (i < nextTurn.Character.Skills.Count)
                    dialogBox.SkillText[i].GetComponent<Button>().onClick.AddListener(() => DecideWhatToDoSkill(nextTurn.Character.Skills[i2], nextTurn));
                else
                    dialogBox.SkillText[i].GetComponent<Button>().onClick.AddListener(() => DecideWhatToDoSkill(null, nextTurn));
            }

            dialogBox.SetEnemyImages(enemyUnits);
            for (var i = 0; i < dialogBox.EnemyImages.Count; i++)
            {
                dialogBox.EnemyImages[i].GetComponent<Button>().onClick.RemoveAllListeners();
                var i2 = i;
                if (i < enemyUnits.Count)
                    dialogBox.EnemyImages[i].GetComponent<Button>().onClick.AddListener(() => WhatToDoTargetEnemy(enemyUnits[i2]));
                else
                    dialogBox.EnemyImages[i].GetComponent<Button>().onClick.AddListener(() => WhatToDoTargetEnemy(null));
            }

            dialogBox.SetItemsMenu();
            playerUnit = nextTurn;
            PlayerAction();
        }
        else
        {
            dialogBox.EnableActionSelector(false);
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
        {
            team.Team.ForEach(c => c.OnBattleOver());
            OnBattleOver(false, enemies);
        }  
        else
            DecideNextTurn();

    }

    void TargetEnemy()
    {
        state = BattleState.TargetEnemy;

        dialogBox.EnableSkillSelector(false);
        dialogBox.EnableSkillDetails(false);
        dialogBox.EnableEnemySelector(true);

        currentTarget = 0;
    }

    void PlayerPosition()
    {
        state = BattleState.PlayerPosition;

        dialogBox.EnableActionSelector(false);

        if (playerUnit.Character.Position != 0)
            currentPosition = 0;
        else
            currentPosition = 1;
    }

    void PlayerItem()
    {
        state = BattleState.PlayerItem;

        dialogBox.EnableActionSelector(false);
        dialogBox.EnableItemDetails(true);
        dialogBox.EnableItemSelector(true);

        currentItem = 0;
    }

    IEnumerator PerformPlayerSkill(Skill skill)
    {
        state = BattleState.Busy;

        yield return new WaitForSeconds(1f);

        yield return RunSkill(playerUnit, enemyUnit, skill);

        yield return new WaitForSeconds(1f);

        yield return CheckPoisonEffect(playerUnit);

        if (playerUnits.Count == 0)
        {
            team.Team.ForEach(c => c.OnBattleOver());
            OnBattleOver(false, enemies);
        }else if(enemyUnits.Count == 0)
        {
            team.Team.ForEach(c => c.OnBattleOver());
            OnBattleOver(true, enemies);
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

        var skill = enemyUnit.Character.GetRandomSkill(enemyUnit);

        var playerUnitAttacked = playerUnits[Random.Range(0, playerUnits.Count)];

        yield return new WaitForSeconds(3f);

        yield return RunSkill(enemyUnit, playerUnitAttacked, skill);

        yield return new WaitForSeconds(1f);

        yield return CheckPoisonEffect(enemyUnit);

        if (playerUnits.Count == 0)
        {
            team.Team.ForEach(c => c.OnBattleOver());
            OnBattleOver(false, enemies);
        }
        else if (enemyUnits.Count == 0)
        {
            team.Team.ForEach(c => c.OnBattleOver());
            OnBattleOver(true, enemies);
        }else
        {
            DecideNextTurn();
        }

    }

    IEnumerator RunSkill(BattleUnit sourceUnit, BattleUnit targetUnit, Skill skill)
    {

        //Reduce all cooldowns and status turns
        bool canRunMove = sourceUnit.Character.OnBeforeMove(sourceUnit);

        //Put cooldown on skill
        skill.PutCooldown();

        if (!canRunMove)
            yield break;

        if (!CheckIfSkillHits(skill, sourceUnit.Character, targetUnit.Character))
            yield break;
        
        if (skill.Base.Category == SkillCategory.Status)
        {
            yield return RunSkillEffect(skill, sourceUnit, targetUnit);
        }
        else
        {
            targetUnit.Character.TakeDamage(skill, sourceUnit.Character, false);

            targetUnit.UpdateHP();
        }



        if (targetUnit != null && targetUnit.Character.HP <= 0)
        {
            yield return CharacterDied(targetUnit);

            yield return new WaitForSeconds(2f);

            if (!targetUnit.IsPlayerUnit)
            {
                yield return GainExperience(targetUnit);
            }

        }
    }

    IEnumerator CheckPoisonEffect(BattleUnit sourceUnit)
    {
        //------------ Poison effect ----------------
        sourceUnit.Character.OnAfterTurn(sourceUnit);

        sourceUnit.UpdateHP();

        if (sourceUnit != null && sourceUnit.Character.HP <= 0)
        {
            yield return CharacterDied(sourceUnit);

            yield return new WaitForSeconds(2f);

            if (!sourceUnit.IsPlayerUnit)
            {
                yield return GainExperience(sourceUnit);
            }

        }
        //-----------------------------------------------
    }

    IEnumerator RunSkillEffect(Skill skill, BattleUnit sourceUnit, BattleUnit targetUnit)
    {
        var effects = skill.Base.Effects;

        //Stat boosting
        if (effects.Boosts != null)
        {
            if (skill.Base.Target == SkillTarget.Self)
                sourceUnit.Character.ApplyBoosts(effects.Boosts);
            else
                targetUnit.Character.ApplyBoosts(effects.Boosts);
        }

        //Status Condition
        if(effects.Status != ConditionID.none)
        {
            targetUnit.Character.AddStatus(effects.Status, targetUnit);
        }

        yield return null;
    }

    IEnumerator CharacterDied(BattleUnit unitDied)
    {
        if (unitDied.IsPlayerUnit)
            Debug.Log($"{unitDied.Character.Base.Name} player has died");
        else
            Debug.Log($"{unitDied.Character.Base.Name} enemy has died");

        yield return new WaitForSeconds(2f);

        if (unitDied.IsPlayerUnit)
        {
            playerUnits.Remove(unitDied);
            playerGrid[unitDied.Character.Position].BattleUnitDied();

            team.Team.Remove(unitDied.Character); //Character dies forever
        }
        else
        {
            enemyUnits.Remove(unitDied);
            enemyGrid[unitDied.Character.Position].BattleUnitDied();
        }

        turnsQueue.Remove(unitDied);

        yield return null;
    }

    IEnumerator GainExperience(BattleUnit deadEnemyUnit)
    {
        //Exp gain
        int expYield = deadEnemyUnit.Character.Base.ExpYield;
        int enemyLevel = deadEnemyUnit.Character.Level;

        int expGain = Mathf.FloorToInt(expYield * enemyLevel / 7);
        foreach (BattleUnit bu in playerUnits)
            bu.Character.Xp += expGain;

        //Check level up
        foreach (BattleUnit bu in playerUnits)
        {
            while (bu.Character.CheckForLevelUp())
            {
                bu.UpdateHP();
                var newSkill = bu.Character.GetLearnableSkillAtCurrentLevel();

                if (newSkill != null)
                    bu.Character.LearnSkill(newSkill);
            }
        }

        yield return null;

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

        //Reduce all cooldowns and status turns
        bool canRunMove = playerUnit.Character.OnBeforeMove(playerUnit);

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

        yield return CheckPoisonEffect(playerGrid[currentPosition]);

        yield return new WaitForSeconds(1f);

        DecideNextTurn();
    }

    IEnumerator UseItem(BattleItem bi)
    {
        Debug.Log("Use Item");
        state = BattleState.Busy;

        //Reduce all cooldowns and status turns
        bool canRunMove = playerUnit.Character.OnBeforeMove(playerUnit);

        Item item = bi.Items.Last();

        playerItems.Remove(item);

        playerUnit.Character.ApplyItem(item);

        bi.Items.Remove(item);

        playerUnit.UpdateHP();

        yield return new WaitForSeconds(2f);

        yield return CheckPoisonEffect(playerUnit);

        yield return new WaitForSeconds(1f);

        dialogBox.EnableItemDetails(false);
        dialogBox.EnableItemSelector(false);
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
        }else if(state == BattleState.PlayerItem)
        {
            HandleItemSelection();
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
                PlayerItem();
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

        Skill skill = playerUnit.Character.Skills[currentSkill];

        if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.M)))
        {
            DecideWhatToDoSkill(skill, playerUnit);
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            dialogBox.EnableSkillSelector(false);
            dialogBox.EnableSkillDetails(false);
            PlayerAction();
        }
    }

    void DecideWhatToDoSkill(Skill skill, BattleUnit battleUnit)
    {
        Debug.Log("DecideWhatToDoSkill");
        if(skill != null && skill.IsValid(battleUnit))
        {
            if (skill.Base.Target == SkillTarget.Foe)
            {
                selectedSkill = skill;
                TargetEnemy();
            } 
            else
            {
                dialogBox.EnableSkillSelector(false);
                dialogBox.EnableSkillDetails(false);

                StartCoroutine(PerformPlayerSkill(skill));
            }
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
            WhatToDoTargetEnemy(enemyUnits[currentTarget]);
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            dialogBox.EnableEnemySelector(false);
            PlayerSkill();
        }

    }

    public void WhatToDoTargetEnemy(BattleUnit targetUnit)
    {
        Debug.Log("WhatToDoTargetEnemy");
        if (targetUnit != null)
        {
            dialogBox.EnableEnemySelector(false);

            enemyUnit = targetUnit;

            StartCoroutine(PerformPlayerSkill(selectedSkill));
        }
        
    }

    private void UpdateEnemySelection(int currentTarget)
    {
        for(int i = 0; i < enemyUnits.Count; i++)
        {
            if(i == currentTarget)
            {
                enemyGrid.Where(x => x.Character == enemyUnits[i].Character).FirstOrDefault().GetComponent<Image>().color = new Color(1f, 0, 0, 0.5f);
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
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            foreach (var pu in playerGrid)
            {
                pu.Setup(pu.Character);
            }
            PlayerAction();
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

    void HandleItemSelection()
    {
        if (Input.GetKeyDown(KeyCode.D) && currentItem  == 0)
        {
            ++currentItem;
        }
        else if (Input.GetKeyDown(KeyCode.A) && currentItem == 1)
        {
            --currentItem;
        }

        dialogBox.UpdateItemSelection(currentItem);

        if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.M)) && dialogBox.BattleItems[currentItem].Items.Count > 0)
        {
            StartCoroutine(UseItem(dialogBox.BattleItems[currentItem]));
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            currentItem = 0;
            dialogBox.EnableItemDetails(false);
            dialogBox.EnableItemSelector(false);
            PlayerAction();
        }
    }

    bool CheckIfSkillHits(Skill skill, Character source, Character target)
    {
        if (skill.Base.AlwaysHits)
            return true;

        float skillAccuracy = skill.Base.Accuracy;

        int accuracy = source.StatsBoosts[Stat.Accuracy];
        int evasion = target.StatsBoosts[Stat.Evasion];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if (accuracy > 0)
            skillAccuracy *= boostValues[accuracy];
        else
            skillAccuracy /= boostValues[-accuracy];

        if (evasion > 0)
            skillAccuracy /= boostValues[evasion];
        else
            skillAccuracy *= boostValues[-evasion];

        return Random.Range(1, 101) <= skillAccuracy;
    }
}
