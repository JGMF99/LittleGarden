using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{

    private int currentTurnCooldown;

    public SkillBase Base { get; set; }

    public int CurrentTurnCooldown { get => currentTurnCooldown; set => currentTurnCooldown = value; }

    public Skill(SkillBase cBase)
    {
        Base = cBase;
    }

    internal void PutCooldown()
    {
        CurrentTurnCooldown = Base.TurnsCooldown;
    }

    internal void ResetCooldown()
    {
        CurrentTurnCooldown = 0;
    }

    internal bool IsValid(BattleUnit unit)
    {
        bool isValid = false;
        if (unit.IsPlayerUnit)
        {
            if (unit.Character.Position % 2 == 0 && Base.Position == SkillPosition.BackRow)
                isValid = true;
            else if (unit.Character.Position % 2 == 1 && Base.Position == SkillPosition.FrontRow)
                isValid = true;
            else if (Base.Position == SkillPosition.Both)
                isValid = true;
        }
        else
        {
            if (unit.Character.Position % 2 == 0 && Base.Position == SkillPosition.FrontRow)
                isValid = true;
            else if (unit.Character.Position % 2 == 1 && Base.Position == SkillPosition.BackRow)
                isValid = true;
            else if (Base.Position == SkillPosition.Both)
                isValid = true;
        }

        return isValid && CurrentTurnCooldown == 0;
    }

    public string GetReason(BattleUnit unit)
    {
        if (currentTurnCooldown > 0)
            return "Skill is on cooldown";

        if (unit.IsPlayerUnit)
        {
            if (!(unit.Character.Position % 2 == 0 && Base.Position == SkillPosition.BackRow))
                return "Skill is only available in front row";
            else if (!(unit.Character.Position % 2 == 1 && Base.Position == SkillPosition.FrontRow))
                return "Skill is only available in back row";
        }

        return "";
    }
}
