using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{

    [SerializeField] Text nameText;
    [SerializeField] HPBar hpBar;

    Character _character;
    public void SetData(Character character)
    {
        _character = character;

        nameText.text = character.Base.Name;

        hpBar.SetHP((float)character.HP / character.MaxHp);
    }

    public void UpdateHP()
    {
        hpBar.SetHP((float)_character.HP / _character.MaxHp);
    }

}
