using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyParty
{
    List<Character> team;

    public List<Character> Team { get => team; set => team = value; }


    public Character GetPositionCharacter(int position)
    {
        return team.Where(x => x.Position == position).FirstOrDefault();
    }

    public string ToString()
    {
        var s = "";
        foreach (var c in Team)
            s += c.Base.Name + " " + c.Position + "\n ";
        return s;
    }
}
