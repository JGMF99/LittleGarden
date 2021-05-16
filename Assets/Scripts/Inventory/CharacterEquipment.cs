using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEquipment : MonoBehaviour
{

    [SerializeField] EquipmentSlot helmet;
    [SerializeField] EquipmentSlot chestplate;
    [SerializeField] EquipmentSlot trinket;

    internal void UpdateSelectedCharacterEquipment(Character character)
    {
        helmet.Setup(character.Helmet);
        chestplate.Setup(character.Chestplate);
        trinket.Setup(character.Trinket);
    }
}
