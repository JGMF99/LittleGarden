using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{


    [SerializeField] bool isPlayerUnit;
    [SerializeField] HPBar hpBar;
    [SerializeField] GameObject stun;
    [SerializeField] GameObject poison;

    public Character Character{get; set; }
    public bool IsPlayerUnit { get => isPlayerUnit; set => isPlayerUnit = value; }

    public void Reset()
    {
        GetComponent<Image>().sprite = null;
        GetComponent<Image>().color = Color.clear;
        hpBar.gameObject.SetActive(false);
        Character = null;

        ShowPoison(false);
        ShowStun(false);
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

            ShowPoison(character.Status.Contains(ConditionsDB.Conditions[ConditionID.psn]));
            ShowStun(character.Status.Contains(ConditionsDB.Conditions[ConditionID.stn]));
        }
        else
        {
            GetComponent<Image>().color = Color.clear;

            ShowPoison(false);
            ShowStun(false);
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
        ShowPoison(false);
        ShowStun(false);
    }

    public void UpdateHP()
    {
        hpBar.SetHP((float)Character.HP / Character.MaxHp);
    }

    public void Swap(BattleUnit bu, int newPosition)
    {
        if(bu.Character == null)
        {
            Character.Position = newPosition;
            bu.Setup(Character);
            Reset();
        }
        else
        {
            var temp = Character;

            Character = bu.Character;
            bu.Character = temp;

            Character.Position = bu.Character.Position;
            bu.Character.Position = newPosition;

            Setup(Character);
            bu.Setup(bu.Character);

        }  

    }

    public void ShowStun(bool showStun)
    {
        stun.SetActive(showStun);
    }

    public void ShowPoison(bool showPoiston)
    {
        poison.SetActive(showPoiston);
    }
}
