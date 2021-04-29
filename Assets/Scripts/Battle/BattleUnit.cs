using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{


    [SerializeField] bool isPlayerUnit;
    [SerializeField] HPBar hpBar;

    public Character Character{get; set; }
    public bool IsPlayerUnit { get => isPlayerUnit; set => isPlayerUnit = value; }

    public void Reset()
    {
        GetComponent<Image>().color = Color.clear;
        hpBar.gameObject.SetActive(false);
    }

    public void Setup(Character character)
    {
        if(character != null)
        {
            Character = character;

            GetComponent<Image>().sprite = Character.Base.SideSprite;
            GetComponent<Image>().color = Color.white;

            hpBar.gameObject.SetActive(true);
            UpdateHP();
        }
        
    }

    public void SetupMenu(Character character)
    {
        if (character != null)
        {
            Character = character;

            GetComponent<Image>().sprite = Character.Base.FrontSprite;
            this.gameObject.SetActive(true);
            //GetComponent<Image>().color = Color.white;
        }
        else
        {
            this.gameObject.SetActive(false);
        }

    }

    public void BattleUnitDied()
    {
        GetComponent<Image>().color = Color.clear;
        hpBar.gameObject.SetActive(false);
    }

    public void UpdateHP()
    {
        hpBar.SetHP((float)Character.HP / Character.MaxHp);
    }
}
