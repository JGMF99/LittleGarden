using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{

    [SerializeField] CharactersBase _base;
    [SerializeField] int level;


    [SerializeField] bool isPlayerUnit;

    public Character Character{get; set; }

    public void Setup()
    {

        Character = new Character(_base, level);

        GetComponent<Image>().sprite = Character.Base.SideSprite;
    }
}
