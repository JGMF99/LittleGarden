using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePosition : MonoBehaviour
{

    [SerializeField] List<Position> battlePositions;

    private CharacterParty characterParty;



    internal void Setup(CharacterParty team)
    {
        characterParty = team;

        for(var i = 0; i < battlePositions.Count; i++)
        {
            battlePositions[i].Setup(characterParty.GetPositionCharacter(i), i);

            battlePositions[i].OnDragStart += HighlightPositions;
            battlePositions[i].OnDragEnd += UnhighlightPositions;
        }
    }

    private void HighlightPositions(Position position)
    {
        foreach(Position p in battlePositions)
        {
            if(p != position)
                p.GetComponent<Image>().color = new Color(p.Color.r, p.Color.g, p.Color.b, 0.8f);
        }
    }

    private void UnhighlightPositions()
    {
        foreach (Position p in battlePositions)
        {
            if (p.Character != null)
            {
                p.GetComponent<Image>().color = Color.white;
            }
            else
            {
                p.GetComponent<Image>().color = Color.clear;
            }
        }
    }
}
