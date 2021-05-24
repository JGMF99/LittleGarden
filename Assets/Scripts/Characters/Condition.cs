using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition
{

    private string name;
    private string description;
    private string startMessage;

    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public string StartMessage { get => startMessage; set => startMessage = value; }

    public Action<BattleUnit> OnStart { get; set; }

    public Func<BattleUnit, bool> OnBeforeMove { get; set; }

    public Action<BattleUnit> OnAfterTurn { get; set; }
}
