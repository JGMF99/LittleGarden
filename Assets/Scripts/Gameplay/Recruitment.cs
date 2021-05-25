using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Recruitment
{
    [SerializeField] CharactersBase character;
    [SerializeField] List<Character> teamToDefeat;

    public bool isDone;

    public List<Character> TeamToDefeat { get => teamToDefeat; set => teamToDefeat = value; }
    public CharactersBase Character { get => character; set => character = value; }

    internal void Completed()
    {
        isDone = true;
    }
}
