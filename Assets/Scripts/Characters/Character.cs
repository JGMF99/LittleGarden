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
    

    public List<Skill> Skills { get; set; }

    public void Init()
    {

        HP = MaxHp;

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
    }

    public int MaxHp
    {
        get { return Mathf.FloorToInt((_base.MaxHp * level) / 100f) + 10; }
    }

    public int Attack
    {
        get { return Mathf.FloorToInt((_base.Attack * level) / 100f) + 5 + AttackEquipment(); }
    }

    

    public int Defense
    {
        get { return Mathf.FloorToInt((_base.Defense * level) / 100f) + 5 + DefenseEquipment(); }
    }

    public int Speed
    {
        get { return Mathf.FloorToInt((_base.Speed * level) / 100f) + 5; }
    }

    public CharactersBase Base { get => _base; set => _base = value; }
    public int Level { get => level; set => level = value; }
    public int Position { get => position; set => position = value; }
    public int Xp { get => xp; set => xp = value; }
    public Item Helmet { get => helmet; set => helmet = value; }
    public Item Chestplate { get => chestplate; set => chestplate = value; }
    public Item Trinket { get => trinket; set => trinket = value; }

    public bool CheckForLevelUp()
    {
        if (Xp > Base.GetExpForLevel(Level + 1))
        {
            Level++;
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

    public bool TakeDamage(Skill skill, Character attacker, bool isPlayerUnit)
    {

        float modifiers = Random.Range(0.85f, 1f);
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * skill.Base.Damage * ((float)attacker.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        //Take less damage on back row
        if( ( Position % 2 == 0 && isPlayerUnit ) ||  (Position % 2 == 1 && !isPlayerUnit))
        {
            HP -= damage/2;
        }
        else
        {
            HP -= damage;
        }

        
        if(HP <= 0)
        {
            HP = 0;
            return true;
        }

        return false;
    }


    public Skill GetRandomSkill()
    {
        int r = Random.Range(0, Skills.Count);
        return Skills[r];
    }

    internal void ChangeHelmet(Item item)
    {
        Helmet = item;
    }

    internal void ChangeChestplate(Item item)
    {
        Chestplate = item;
    }

    internal void ChangeTrinket(Item item)
    {
        trinket = item;
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
