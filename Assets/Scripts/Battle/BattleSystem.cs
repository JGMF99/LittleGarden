using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum BattleState
{
    Start, PlayerAction, PlayerSkill, TargetEnemy, TargetAlly, PlayerPosition, PlayerItem, EnemyMove, Busy
}

public class BattleSystem : MonoBehaviour
{

    
    BattleUnit playerUnit;
    [SerializeField] List<BattleUnit> playerGrid;
    BattleUnit enemyUnit;
    BattleUnit skillTarget;
    [SerializeField] List<BattleUnit> enemyGrid;

    [SerializeField] BattleDialogBox dialogBox;

    public event Action<bool, EnemyParty, Recruitment> OnBattleOver;

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

    private Recruitment recruitment;

    public void StartBattle(CharacterParty team, EnemyParty enemies, List<Item> items, Recruitment recruitment)
    {
        this.team = team;
        this.enemies = enemies;
        playerItems = items;
        this.recruitment = recruitment;

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

        StartCoroutine(DecideNextTurn());

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

    IEnumerator DecideNextTurn()
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
            }

            dialogBox.SetEnemyImages(enemyUnits);
            for (var i = 0; i < dialogBox.EnemyImages.Count; i++)
            {
                dialogBox.EnemyImages[i].GetComponent<Button>().onClick.RemoveAllListeners();
                var i2 = i;
                if (i < enemyUnits.Count)
                    dialogBox.EnemyImages[i].GetComponent<Button>().onClick.AddListener(() => WhatToDoTargetSkill(enemyUnits[i2]));
            }

            dialogBox.SetAllyImages(playerUnits);

            dialogBox.SetItemsMenu();

            dialogBox.ShowBattleInfo($"Friendly {nextTurn.Character.Base.Name} turn");
            yield return new WaitForSeconds(1f);

            //Reduce all cooldowns and status turns
            bool canRunMove = nextTurn.Character.OnBeforeMove(nextTurn);

            if (canRunMove)
            {
                playerUnit = nextTurn;
                PlayerAction();
            }
            else
            {
                dialogBox.ShowBattleInfo($"Friendly {nextTurn.Character.Base.Name} can't move");
                yield return new WaitForSeconds(2f);
                CheckPoisonEffect(nextTurn);

                StartCoroutine(DecideNextTurn());
            }

            
        }
        else
        {
            dialogBox.EnableActionSelector(false);
            dialogBox.ShowBattleInfo($"Enemy {nextTurn.Character.Base.Name} turn");
            yield return new WaitForSeconds(1f);

            //Reduce all cooldowns and status turns
            bool canRunMove = nextTurn.Character.OnBeforeMove(nextTurn);

            if (canRunMove)
            {
                enemyUnit = nextTurn;
                StartCoroutine(EnemyMove());
            }
            else
            {
                dialogBox.ShowBattleInfo($"Enemy {nextTurn.Character.Base.Name} can't move");
                yield return new WaitForSeconds(2f);

                CheckPoisonEffect(nextTurn);

                StartCoroutine(DecideNextTurn());
            }

            
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

    public IEnumerator RunAwayCouroutine()
    {
        state = BattleState.Busy;

        var runSuccessful = Random.Range(0, 101) < 80;

        if (runSuccessful)
        {
            dialogBox.ShowBattleInfo($"Run away successfully");

            yield return new WaitForSeconds(2f);

            team.Team.ForEach(c => c.OnBattleOver());
            OnBattleOver(false, enemies, recruitment);
        }
        else
        {
            dialogBox.ShowBattleInfo($"Could not run away");
            yield return new WaitForSeconds(2f);
            StartCoroutine(DecideNextTurn());
        }
    }

    void RunAway()
    {
        dialogBox.EnableActionSelector(false);

        StartCoroutine(RunAwayCouroutine());

    }

    void TargetEnemy()
    {
        state = BattleState.TargetEnemy;

        dialogBox.EnableSkillSelector(false);
        dialogBox.EnableSkillDetails(false);
        dialogBox.EnableEnemySelector(true);

        currentTarget = 0;
    }

    void TargetAlly()
    {
        state = BattleState.TargetAlly;

        dialogBox.EnableSkillSelector(false);
        dialogBox.EnableSkillDetails(false);
        dialogBox.EnableAllySelector(true);

        currentTarget = 0;
    }

    void PlayerPosition()
    {
        state = BattleState.PlayerPosition;

        dialogBox.EnableActionSelector(false);

        int position = 0;
        foreach (BattleUnit bu in playerGrid)
        {
            bu.GetComponent<Button>().onClick.RemoveAllListeners();
            var positionFinal = position;
            if(bu.Character == null || bu.Character.Position != playerUnit.Character.Position)
                bu.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(SwitchPositions(positionFinal)));
            position++;
        }

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

        if (skill.Base.Area == SkillArea.Single)
            yield return RunSkill(playerUnit, skillTarget, skill);
        else
            yield return RunAoESkill(playerUnit, skill);

        yield return CheckPoisonEffect(playerUnit);

        if (playerUnits.Count == 0)
        {
            dialogBox.ShowBattleInfo($"Battle has been lost");

            yield return new WaitForSeconds(2f);

            team.Team.ForEach(c => c.OnBattleOver());
            OnBattleOver(false, enemies, recruitment);
        }else if(enemyUnits.Count == 0)
        {
            dialogBox.ShowBattleInfo($"Battle has been won");

            yield return new WaitForSeconds(2f);

            team.Team.ForEach(c => c.OnBattleOver());
            OnBattleOver(true, enemies, recruitment);
        }
        else
        {
            StartCoroutine(DecideNextTurn());
        }
    }

    public BattleUnit GetRandomEnemy()
    {
        foreach(BattleUnit bu in playerUnits)
        {
            if (bu.Character.TauntTime > 0)
                return bu;
        }
        return playerUnits[Random.Range(0, playerUnits.Count)];
    }

    IEnumerator EnemyMove()
    {
        Debug.Log("Enemy " + enemyUnit.Character.Base.Name + " attacked");

        state = BattleState.EnemyMove;

        var skill = enemyUnit.Character.GetRandomSkill(enemyUnit);

        BattleUnit targetUnit;

        if (skill.Base.Target == SkillTarget.FoeEnemy)
            targetUnit = GetRandomEnemy();
        else if (skill.Base.Target == SkillTarget.FoeAlly)
            targetUnit = enemyUnits[Random.Range(0, enemyUnits.Count)];
        else
            targetUnit = enemyUnit;

        if (skill.Base.Area == SkillArea.Single)
            yield return RunSkill(enemyUnit, targetUnit, skill);
        else
            yield return RunAoESkill(enemyUnit, skill);

        yield return CheckPoisonEffect(enemyUnit);

        if (playerUnits.Count == 0)
        {
            dialogBox.ShowBattleInfo($"Battle has been lost");

            yield return new WaitForSeconds(2f);

            team.Team.ForEach(c => c.OnBattleOver());
            OnBattleOver(false, enemies, recruitment);
        }
        else if (enemyUnits.Count == 0)
        {
            dialogBox.ShowBattleInfo($"Battle has been won");

            yield return new WaitForSeconds(2f);

            team.Team.ForEach(c => c.OnBattleOver());
            OnBattleOver(true, enemies, recruitment);
        }else
        {
            StartCoroutine(DecideNextTurn());
        }

    }


    IEnumerator RunSkill(BattleUnit sourceUnit, BattleUnit targetUnit, Skill skill)
    {
        dialogBox.ShowBattleInfo($"{sourceUnit.Character.Base.Name} used {skill.Base.Name}");

        yield return new WaitForSeconds(2f);

        //Put cooldown on skill
        skill.PutCooldown();

        if (!CheckIfSkillHits(skill, sourceUnit.Character, targetUnit.Character))
        {
            dialogBox.ShowBattleInfo($"{targetUnit.Character.Base.Name} dodged {skill.Base.Name}");
            yield return new WaitForSeconds(2f);
            yield break;
        }
            
        
        if (skill.Base.Category == SkillCategory.Status)
        {
            yield return RunSkillEffect(skill, sourceUnit, targetUnit);
        }else if(skill.Base.Category == SkillCategory.Heal)
        {
            targetUnit.Character.Heal(skill, sourceUnit.Character);

            targetUnit.UpdateHP();
        }
        else
        {
            targetUnit.Character.TakeDamage(skill, sourceUnit.Character, targetUnit.IsPlayerUnit);

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

    IEnumerator RunAoESkill(BattleUnit sourceUnit, Skill skill)
    {
        dialogBox.ShowBattleInfo($"{sourceUnit.Character.Base.Name} used {skill.Base.Name}");

        yield return new WaitForSeconds(2f);

        //Put cooldown on skill
        skill.PutCooldown();

        List<BattleUnit> targetUnits;

        if ((skill.Base.Target == SkillTarget.FoeAlly && sourceUnit.IsPlayerUnit) || skill.Base.Target == SkillTarget.FoeEnemy && !sourceUnit.IsPlayerUnit)
            targetUnits = playerUnits;
        else
            targetUnits = enemyUnits;    

        /*if (!CheckIfSkillHits(skill, sourceUnit.Character, targetUnit.Character))
        {
            StartCoroutine(dialogBox.ShowBattleInfo($"{targetUnit.Character.Base.Name} dodged {skill.Base.Name}"));
            yield break;
        }*/


        foreach(BattleUnit targetUnit in targetUnits)
        {
            if (skill.Base.Category == SkillCategory.Status)
            {
                yield return RunSkillEffect(skill, sourceUnit, targetUnit);
            }
            else if (skill.Base.Category == SkillCategory.Heal)
            {
                targetUnit.Character.Heal(skill, sourceUnit.Character);

                targetUnit.UpdateHP();
            }
            else
            {
                targetUnit.Character.TakeDamage(skill, sourceUnit.Character, targetUnit.IsPlayerUnit);

                targetUnit.UpdateHP();
            }
        }



        foreach (BattleUnit targetUnit in targetUnits)
        {
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

            
    }

    IEnumerator CheckPoisonEffect(BattleUnit sourceUnit)
    {
        //------------ Poison effect ----------------
        if(sourceUnit.Character.PoisonTime > 0)
        {
            dialogBox.ShowBattleInfo($"{sourceUnit.Character.Base.Name} suffered poison damage");
            yield return new WaitForSeconds(2f);
        }
        
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
            {
                sourceUnit.Character.ApplyBoosts(effects.Boosts);
            }

            else
            {
                targetUnit.Character.ApplyBoosts(effects.Boosts);
            }
                
        }

        //Status Condition
        if(effects.Status != ConditionID.none)
        {
            dialogBox.ShowBattleInfo($"{targetUnit.Character.Base.Name} {ConditionsDB.Conditions[effects.Status].StartMessage}");
            targetUnit.Character.AddStatus(effects.Status, targetUnit, skill.Base.MinTurns, skill.Base.MaxTurns);

            yield return new WaitForSeconds(2f);
        }

        yield return null;
    }

    IEnumerator CharacterDied(BattleUnit unitDied)
    {
        if (unitDied.IsPlayerUnit)
            Debug.Log($"{unitDied.Character.Base.Name} player has died");
        else
            Debug.Log($"{unitDied.Character.Base.Name} enemy has died");

        dialogBox.ShowBattleInfo($"{unitDied.Character.Base.Name} died");

        yield return new WaitForSeconds(2f);

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

                dialogBox.ShowBattleInfo($"{bu.Character.Base.Name} leveled up to level {bu.Character.Level}");
                yield return new WaitForSeconds(0.5f);
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


    IEnumerator SwitchPositions(int newPosition)
    {
        state = BattleState.Busy;

        foreach (BattleUnit pu in playerGrid)
        {
            pu.GetComponent<Button>().onClick.RemoveAllListeners();
        }

        var oldPosition = playerUnit.Character.Position;

        playerGrid[oldPosition].Swap(playerGrid[newPosition], newPosition);

        foreach (var pu in playerGrid)
        {
            pu.Setup(pu.Character);
        }

        if (playerGrid[oldPosition].Character == null)
        {
            playerUnits.Remove(playerGrid[oldPosition]);
            playerUnits.Add(playerGrid[newPosition]);

            turnsQueue.Remove(playerGrid[oldPosition]);
            turnsQueue.Add(playerGrid[newPosition]);

            dialogBox.ShowBattleInfo($"{playerGrid[newPosition].Character.Base.Name} has changed position");
            yield return new WaitForSeconds(2f);

        }
        else
        {
            turnsQueue = Swap(turnsQueue, turnsQueue.IndexOf(playerGrid[newPosition]), turnsQueue.IndexOf(playerGrid[oldPosition]));

            dialogBox.ShowBattleInfo($"{playerGrid[newPosition].Character.Base.Name} has swapped position with {playerGrid[oldPosition].Character.Base.Name}");
            yield return new WaitForSeconds(2f);
        }


        yield return CheckPoisonEffect(playerGrid[newPosition]);

        StartCoroutine(DecideNextTurn());
    }

    IEnumerator UseItem(BattleItem bi)
    {
        Debug.Log("Use Item");
        if(dialogBox.BattleItems[currentItem].Items.Count > 0)
        {
            state = BattleState.Busy;

            Item item = bi.Items.Last();

            playerItems.Remove(item);

            playerUnit.Character.ApplyItem(item);

            bi.Items.Remove(item);

            playerUnit.UpdateHP();

            dialogBox.ShowBattleInfo($"{playerUnit.Character.Base.Name} used {item.Base.Name}");
            yield return new WaitForSeconds(2f);

            yield return CheckPoisonEffect(playerUnit);

            dialogBox.EnableItemDetails(false);
            dialogBox.EnableItemSelector(false);
            StartCoroutine(DecideNextTurn());
        }
        
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
        }
        else if (state == BattleState.TargetAlly)
        {
            HandleTargetAlly();
        }
        else if(state == BattleState.PlayerPosition)
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

        if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && currentAction < 3)
        {
            ++currentAction;
        }
        else if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && currentAction > 0)
        {
            --currentAction;
        }
        else if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && currentAction < 2)
        {
            currentAction += 2;
        }
        else if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && currentAction > 1)
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
        if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && currentSkill < playerUnit.Character.Skills.Count - 1)
        {
            ++currentSkill;
        }
        else if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && currentSkill > 0)
        {
            --currentSkill;
        }
        else if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && currentSkill < playerUnit.Character.Skills.Count - 2)
        {
            currentSkill += 2;
        }
        else if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && currentSkill > 1)
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
            if (skill.Base.Target == SkillTarget.FoeEnemy && skill.Base.Area == SkillArea.Single)
            {
                selectedSkill = skill;

                foreach(BattleUnit bu in enemyGrid)
                {
                    bu.GetComponent<Button>().onClick.RemoveAllListeners();
                    if (bu.Character != null)
                    {
                        bu.GetComponent<Button>().onClick.AddListener(() => WhatToDoTargetSkill(bu));
                    }     
                }

                TargetEnemy();
            }else if(skill.Base.Target == SkillTarget.FoeAlly && skill.Base.Area == SkillArea.Single)
            {
                selectedSkill = skill;

                foreach (BattleUnit bu in playerGrid)
                {
                    bu.GetComponent<Button>().onClick.RemoveAllListeners();
                    if (bu.Character != null)
                    {
                        bu.GetComponent<Button>().onClick.AddListener(() => WhatToDoTargetSkill(bu));
                    }
                }

                TargetAlly();
            }
            else
            {
                dialogBox.EnableSkillSelector(false);
                dialogBox.EnableSkillDetails(false);

                if (skill.Base.Target == SkillTarget.Self)
                    skillTarget = battleUnit;

                StartCoroutine(PerformPlayerSkill(skill));
            }
        }
        else
        {
            dialogBox.ShowBattleInfo($"{skill.GetReason(battleUnit)}");
            
        }
        
    }

    void HandleTargetEnemy()
    {
        if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && currentTarget < enemyUnits.Count - 1)
        {
            ++currentTarget;
        }
        else if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && currentTarget > 0)
        {
            --currentTarget;
        }

        dialogBox.UpdateEnemySelection(currentTarget);
        UpdateEnemySelection(currentTarget);

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.M))
        {
            WhatToDoTargetSkill(enemyUnits[currentTarget]);
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            foreach (BattleUnit bu in enemyGrid)
            {
                bu.GetComponent<Button>().onClick.RemoveAllListeners();
            }

            dialogBox.EnableEnemySelector(false);
            PlayerSkill();
        }

    }

    void HandleTargetAlly()
    {
        if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && currentTarget < playerUnits.Count - 1)
        {
            ++currentTarget;
        }
        else if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && currentTarget > 0)
        {
            --currentTarget;
        }

        dialogBox.UpdateAllySelection(currentTarget);
        UpdateAllySelection(currentTarget);

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.M))
        {
            WhatToDoTargetSkill(playerUnits[currentTarget]);
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            foreach (BattleUnit bu in playerGrid)
            {
                bu.GetComponent<Button>().onClick.RemoveAllListeners();
            }


            dialogBox.EnableAllySelector(false);
            PlayerSkill();
        }

    }

    public void WhatToDoTargetSkill(BattleUnit targetUnit)
    {
        Debug.Log("WhatToDoTargetEnemy");
        if (targetUnit != null)
        {
            foreach (BattleUnit bu in enemyGrid)
            {
                bu.GetComponent<Button>().onClick.RemoveAllListeners();
            }
            foreach (BattleUnit bu in playerGrid)
            {
                bu.GetComponent<Button>().onClick.RemoveAllListeners();
            }

            dialogBox.EnableEnemySelector(false);

            skillTarget = targetUnit;

            if(selectedSkill.Base.Target == SkillTarget.FoeEnemy)
                foreach (BattleUnit bu in enemyUnits)
                {
                    if (bu.Character.TauntTime > 0)
                    {
                        skillTarget = bu;
                        break;
                    }
                }

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

    private void UpdateAllySelection(int currentTarget)
    {
        for (int i = 0; i < playerUnits.Count; i++)
        {
            if (i == currentTarget)
            {
                playerGrid.Where(x => x.Character == playerUnits[i].Character).FirstOrDefault().GetComponent<Image>().color = new Color(1f, 0, 0, 0.5f);
            }
            else
            {
                playerGrid.Where(x => x.Character == playerUnits[i].Character).FirstOrDefault().GetComponent<Image>().color = new Color(1, 1, 1, 1f);
            }
        }
    }

    void HandlePlayerPosition()
    {

        if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && currentPosition < playerGrid.Count - 1 && currentPosition + 1 != playerUnit.Character.Position)
        {
            ++currentPosition;
        }
        else if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && currentPosition > 0 && currentPosition - 1 != playerUnit.Character.Position)
        {
            --currentPosition;
        }
        else if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && currentPosition < playerGrid.Count - 2)
        {
            if(currentPosition + 2 == playerUnit.Character.Position)
            {
                if(currentPosition + 2 < playerGrid.Count)
                    currentPosition += 4;
            }else
                currentPosition += 2;
        }
        else if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && currentPosition > 1)
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
            StartCoroutine(SwitchPositions(currentPosition));
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            foreach (BattleUnit pu in playerGrid)
            {
                pu.Setup(pu.Character);
                pu.GetComponent<Button>().onClick.RemoveAllListeners();
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
        if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && currentItem  == 0)
        {
            ++currentItem;
        }
        else if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && currentItem == 1)
        {
            --currentItem;
        }

        dialogBox.UpdateItemSelection(currentItem);

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.M))
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
