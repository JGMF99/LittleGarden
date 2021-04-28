using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{


    [SerializeField] bool isPlayerUnit;

    public Character Character{get; set; }
    public bool IsPlayerUnit { get => isPlayerUnit; set => isPlayerUnit = value; }

    public void Setup(Character character)
    {
        if(character != null)
        {
            Character = character;

            GetComponent<Image>().sprite = Character.Base.SideSprite;
            GetComponent<Image>().color = Color.white;
        }
        else
        {
            GetComponent<Image>().color = Color.clear;
        }
        
    }

    public void SetupMenu(Character character)
    {
        if (character != null)
        {
            Character = character;

            GetComponent<Image>().sprite = Character.Base.FrontSprite;
        }
        else
        {
            GetComponent<Image>().color = Color.clear;
        }

    }

    public void BattleUnitDied()
    {
        GetComponent<Image>().color = Color.clear;
    }
}
