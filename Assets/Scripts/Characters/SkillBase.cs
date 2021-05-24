using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Character/Create new skill")]
public class SkillBase : ScriptableObject
{

    [SerializeField] new string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] int damage;
    [SerializeField] int accuracy;
    [SerializeField] bool alwaysHits;
    [SerializeField] int turnsCooldown;

    [SerializeField] SkillPosition position;

    [SerializeField] SkillCategory category;
    [SerializeField] SkillEffects effects;
    [SerializeField] SkillTarget target;

    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public int Damage { get => damage; set => damage = value; }
    public int Accuracy { get => accuracy; set => accuracy = value; }
    public int TurnsCooldown { get => turnsCooldown; set => turnsCooldown = value; }
    public SkillEffects Effects { get => effects; set => effects = value; }
    public SkillTarget Target { get => target; set => target = value; }
    public SkillCategory Category { get => category; set => category = value; }
    public SkillPosition Position { get => position; set => position = value; }
    public bool AlwaysHits { get => alwaysHits; set => alwaysHits = value; }
}

[System.Serializable]
public class SkillEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID status;

    public List<StatBoost> Boosts { get => boosts; set => boosts = value; }
    public ConditionID Status { get => status; set => status = value; }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

public enum SkillTarget
{
    Foe, Self
}

public enum SkillCategory
{
    Physical, Status
}

public enum SkillPosition
{
    BackRow, FrontRow, Both
}