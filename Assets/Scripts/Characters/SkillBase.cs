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
    [SerializeField] int turnsCooldown;

    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public int Damage { get => damage; set => damage = value; }
    public int Accuracy { get => accuracy; set => accuracy = value; }
    public int TurnsCooldown { get => turnsCooldown; set => turnsCooldown = value; }
}
