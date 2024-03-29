using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Helmet, Body, Trinket, General
}

[CreateAssetMenu(fileName = "Item", menuName = "Character/Create new item")]
public class ItemBase : ScriptableObject
{
    [SerializeField] new string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite sprite;

    [SerializeField] int healthRegen;
    [SerializeField] int attackMod;
    [SerializeField] int defMod;
    [SerializeField] int cooldownRedcution;

    [SerializeField] ItemType type;

    public ItemType Type { get => type; set => type = value; }
    public Sprite Sprite { get => sprite; set => sprite = value; }
    public int AttackMod { get => attackMod; set => attackMod = value; }
    public int DefMod { get => defMod; set => defMod = value; }
    public int CooldownRedcution { get => cooldownRedcution; set => cooldownRedcution = value; }
    public int HealthRegen { get => healthRegen; set => healthRegen = value; }
    public string Description { get => description; set => description = value; }
    public string Name { get => name; set => name = value; }
}
