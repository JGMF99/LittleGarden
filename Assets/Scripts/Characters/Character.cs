using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{

    CharactersBase _base;
    int level;

    public int HP { get; set; }

    public List<Skill> Skills { get; set; }

    public Character(CharactersBase cBase, int cLevel)
    {
        _base = cBase;
        level = cLevel;
        HP = MaxHp;

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
        get { return Mathf.FloorToInt((_base.Attack * level) / 100f) + 5; }
    }

    public int Defense
    {
        get { return Mathf.FloorToInt((_base.Defense * level) / 100f) + 5; }
    }

    public int Speed
    {
        get { return Mathf.FloorToInt((_base.Speed * level) / 100f) + 5; }
    }

    public CharactersBase Base { get => _base; set => _base = value; }
    public int Level { get => level; set => level = value; }


    public bool TakeDamage(Skill skill, Character attacker)
    {

        float modifiers = Random.Range(0.85f, 1f);
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * skill.Base.Damage * ((float)attacker.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;
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

}
