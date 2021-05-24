using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfo : MonoBehaviour
{

    [SerializeField] GameObject image;

    [SerializeField] new Text name;

    [SerializeField] List<Text> skills;

    [SerializeField] Text level;
    [SerializeField] Text xp;
    [SerializeField] Text hp;
    [SerializeField] Text attack;
    [SerializeField] Text defense;
    [SerializeField] Text speed;

    public void UpdateSelectedCharacter(Character character)
    {
        image.GetComponent<Image>().sprite = character.Base.SideSprite;

        name.text = character.Base.name;

        for(var i = 0; i < skills.Count; i++)
        {
            if (i < character.Skills.Count)
                skills[i].text = character.Skills[i].Base.Name;
            else
                skills[i].text = "";
        }

        level.text = "Lvl: " + character.Level.ToString();
        xp.text = "XP: " + character.Xp.ToString() + "/" + character.Base.GetExpForLevel(character.Level + 1);
        hp.text = "HP: " + character.HP.ToString() + "/" + character.MaxHp.ToString();
        attack.text = "Attack: " + character.Attack.ToString();
        defense.text = "Defense: " + character.Defense.ToString();
        speed.text = "Speed: " + character.Speed.ToString();
    }
}
