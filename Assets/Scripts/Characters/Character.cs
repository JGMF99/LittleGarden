using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Character
{

    [SerializeField] CharactersBase _base;
    [SerializeField] int level;
    [SerializeField] int position;
    private int xp;

    private Item helmet;
    private Item chestplate;
    private Item trinket;

    public int HP { get; set; }

    private Dictionary<Stat, int> stats;
    private Dictionary<Stat, int> statsBoosts;

    private List<Condition> status = new List<Condition>();
    private int poisonTime;
    private int stunTime;
    private int tauntTime;

    public List<Skill> Skills { get; set; }

    public void Init()
    {

        Xp = Base.GetExpForLevel(Level);

        //Generate Moves
        Skills = new List<Skill>();
        foreach(var skill in _base.LearnableSkills)
        {
            if(skill.Level <= level)
            {
                Skills.Add(new Skill(skill.SkillBase));
            }

            if (Skills.Count >= 4)
                break;
        }

        CalculateStats();

        HP = MaxHp;

        ResetStatBoost();
        ResetStatus();
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();

        Stats.Add(Stat.Attack, Mathf.FloorToInt((_base.Attack * level) / 100f) + 5 + AttackEquipment());
        Stats.Add(Stat.Defense, Mathf.FloorToInt((_base.Defense * level) / 100f) + 5 + DefenseEquipment());
        Stats.Add(Stat.Speed, Mathf.FloorToInt((_base.Speed * level) / 100f) + 5);

        MaxHp = Mathf.FloorToInt((_base.MaxHp * level) / 100f) + 10;
    }

    void ResetStatBoost()
    {
        StatsBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.Speed, 0},

            {Stat.Accuracy, 0},
            {Stat.Evasion, 0},
        };
    }

    void ResetStatus()
    {
        status = new List<Condition>();
        poisonTime = 0;
        stunTime = 0;
    }

    void ResetCooldowns()
    {
        foreach (Skill s in Skills)
            s.ResetCooldown();
    }

    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        //Apply boost
        int boost = StatsBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);

        return statVal;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach(var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatsBoosts[stat] = Mathf.Clamp(StatsBoosts[stat] + boost, -6,6);

            Debug.Log($"{stat} has been boosted to {StatsBoosts[stat]}");
        }
    }

    public void AddStatus(ConditionID conditionID, BattleUnit battleUnit, int minTurns, int maxTurns)
    {

        if (Status.Contains(ConditionsDB.Conditions[conditionID]))
            Status.Remove(ConditionsDB.Conditions[conditionID]);

        Status.Add(ConditionsDB.Conditions[conditionID]);
        Status.Last()?.OnStart?.Invoke(battleUnit, minTurns, maxTurns);
    }

    public void CureStatus(ConditionID conditionID)
    {
        Status.Remove(ConditionsDB.Conditions[conditionID]);
    }

    public void UpdateHP(int damage)
    {
        
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
    }

    public int MaxHp
    {
        get; set;
    }

    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }

    public int Defense
    {
        get { return GetStat(Stat.Defense); }
    }

    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }

    public CharactersBase Base { get => _base; set => _base = value; }
    public int Level { get => level; set => level = value; }
    public int Position { get => position; set => position = value; }
    public int Xp { get => xp; set => xp = value; }
    public Item Helmet { get => helmet; set => helmet = value; }
    public Item Chestplate { get => chestplate; set => chestplate = value; }
    public Item Trinket { get => trinket; set => trinket = value; }
    public Dictionary<Stat, int> Stats { get => stats; set => stats = value; }
    public Dictionary<Stat, int> StatsBoosts { get => statsBoosts; set => statsBoosts = value; }
    public List<Condition> Status { get => status; set => status = value; }
    public int PoisonTime { get => poisonTime; set => poisonTime = value; }
    public int StunTime { get => stunTime; set => stunTime = value; }
    public int TauntTime { get => tauntTime; set => tauntTime = value; }

    public bool CheckForLevelUp()
    {
        if (Xp > Base.GetExpForLevel(Level + 1))
        {
            var oldMaxHp = MaxHp;
            Level++;
            CalculateStats();
            HP = Mathf.Clamp(HP + MaxHp - oldMaxHp, 0, MaxHp);
            return true;
        }
            
        return false;
    }

    public LearnableSkill GetLearnableSkillAtCurrentLevel()
    {
        return Base.LearnableSkills.Where(x => x.Level == Level).FirstOrDefault();
    }

    public void LearnSkill(LearnableSkill skillToLearn)
    {
        Skills.Add(new Skill(skillToLearn.SkillBase));
    }

    public void Heal(Skill skill, Character healer)
    {
        AudioManager.instance.Play("SoundTwinkle02");
        float modifiers = Random.Range(0.85f, 1f);
        float a = (2 * healer.Level + 10) / 250f;
        float d = a * skill.Base.Damage * ((float)healer.Defense / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        UpdateHP(-damage);

    }

    public void TakeDamage(Skill skill, Character attacker, bool isPlayerUnit)
    {
        AudioManager.instance.Play("SoundMove04");
        float modifiers = Random.Range(0.85f, 1f);
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * skill.Base.Damage * ((float)attacker.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        //Take less damage on back row
        if( ( Position % 2 == 0 && isPlayerUnit ) ||  (Position % 2 == 1 && !isPlayerUnit))
        {
            UpdateHP(damage / 2);
        }
        else
        {
            UpdateHP(damage);
        }

    }

    public bool OnBeforeMove(BattleUnit battleUnit)
    {
        foreach (Skill s in Skills)
            if (s.CurrentTurnCooldown > 0)
                s.CurrentTurnCooldown--;

        for (var i = Status.Count - 1; i >= 0; i--)
            Status[i].OnBeforeTurn?.Invoke(battleUnit);

        foreach (Condition c in Status)
            if(c?.OnBeforeMove != null)
                return c.OnBeforeMove(battleUnit);

        return true;
    }

    public void OnAfterTurn(BattleUnit battleUnit)
    {
        for(var i = Status.Count - 1; i >= 0; i--)
            Status[i].OnAfterTurn?.Invoke(battleUnit);     
            
    }

    public void OnBattleOver()
    {
        ResetStatBoost();
        ResetStatus();
        ResetCooldowns();
    }


    public Skill GetRandomSkill(BattleUnit enemy)
    {
        Skill skill = Skills[Random.Range(0, Skills.Count)];
        while(!skill.IsValid(enemy))
            skill = Skills[Random.Range(0, Skills.Count)];
        return skill;
    }

    public void ApplyItem(Item item)
    {
        foreach(Skill skill in Skills)
        {
            skill.CurrentTurnCooldown -= item.Base.CooldownRedcution;
            if (skill.CurrentTurnCooldown < 0)
                skill.CurrentTurnCooldown = 0;
        }

        HP += item.Base.HealthRegen;

        if (HP > MaxHp)
            HP = MaxHp;
    }
    internal void ChangeHelmet(Item item)
    {
        Helmet = item;
        CalculateStats();
    }

    internal void ChangeChestplate(Item item)
    {
        Chestplate = item;
        CalculateStats();
    }

    internal void ChangeTrinket(Item item)
    {
        trinket = item;
        CalculateStats();
    }

    private int AttackEquipment()
    {
        return GetHelmetAttack() + GetChestplateAttack() + GetTrinketAttack();
    }

    private int DefenseEquipment()
    {
        return GetHelmetDefense() + GetChestplateDefense() + GetTrinketDefense();
    }

    private int GetHelmetDefense()
    {
        if (Helmet != null)
            return Helmet.Base.DefMod;
        else
            return 0;
    }

    private int GetHelmetAttack()
    {
        if (Helmet != null)
            return Helmet.Base.AttackMod;
        else
            return 0;
    }

    private int GetChestplateDefense()
    {
        if (Chestplate != null)
            return Chestplate.Base.DefMod;
        else
            return 0;
    }

    private int GetChestplateAttack()
    {
        if (Chestplate != null)
            return Chestplate.Base.AttackMod;
        else
            return 0;
    }

    private int GetTrinketDefense()
    {
        if (Trinket != null)
            return Trinket.Base.DefMod;
        else
            return 0;
    }

    private int GetTrinketAttack()
    {
        if (Trinket != null)
            return Trinket.Base.AttackMod;
        else
            return 0;
    }
}
