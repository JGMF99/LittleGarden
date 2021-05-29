using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Character/Create new character")]
public class CharactersBase : ScriptableObject
{

    [SerializeField] new string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite sideSprite;

    [SerializeField] CharacterSpecie specie;

    //Base Stats
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int speed;

    [SerializeField] int expYield;
    [SerializeField] GrowthRate growthRate;


    [SerializeField] List<LearnableSkill> learnableSkills;

    public int GetExpForLevel(int level)
    {
        if(growthRate == GrowthRate.Fast)
        {
            return 4 * (level * level * level) / 5;
        }else if(growthRate == GrowthRate.MediumFast)
        {
            return level * level * level;
        }

        return -1;
    }

    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public Sprite FrontSprite { get => frontSprite; set => frontSprite = value; }
    public Sprite SideSprite { get => sideSprite; set => sideSprite = value; }
    public CharacterSpecie Specie { get => specie; set => specie = value; }
    public int MaxHp { get => maxHp; set => maxHp = value; }
    public int Attack { get => attack; set => attack = value; }
    public int Defense { get => defense; set => defense = value; }
    public int Speed { get => speed; set => speed = value; }
    public List<LearnableSkill> LearnableSkills { get => learnableSkills; set => learnableSkills = value; }
    public int ExpYield { get => expYield; set => expYield = value; }
    public GrowthRate GrowthRate { get => growthRate; set => growthRate = value; }
}

[System.Serializable]
public class LearnableSkill
{

    [SerializeField] SkillBase skillBase;
    [SerializeField] int level;

    public SkillBase SkillBase { get => skillBase; set => skillBase = value; }
    public int Level { get => level; set => level = value; }
}

public enum CharacterSpecie
{
    Ant,
    Bee,
    Spider,
    Beetle,
    Firefly,
    Butterfly,
    Plant,
    Flower,
    MutantFlytrap
}

public enum Stat
{
    Attack,
    Defense,
    Speed,

    Accuracy,
    Evasion
}

public enum GrowthRate
{
    Fast,
    MediumFast
}