using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{

    public SkillBase Base { get; set; }
    public int TurnsCooldown { get ; set; }

    public Skill(SkillBase cBase)
    {
        Base = cBase;
        TurnsCooldown = cBase.TurnsCooldown;
    }

}
