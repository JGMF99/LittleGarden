using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        }
    }
}
