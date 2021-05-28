using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{


    private static Dictionary<ConditionID, Condition> conditions = new Dictionary<ConditionID, Condition>()
    {
        {ConditionID.psn,
        new Condition()
        {
             Name = "Poison",
             StartMessage = "has been poisoned",
             OnStart = (BattleUnit battleUnit, int minTurns, int maxTurns) =>
             {
                 battleUnit.Character.PoisonTime = Random.Range(minTurns,maxTurns);
                 battleUnit.ShowPoison(true);
                 Debug.Log($"Poison for {battleUnit.Character.PoisonTime} turns");
             },
             OnAfterTurn = (BattleUnit battleUnit) =>
             {
                 if(battleUnit.Character.PoisonTime <= 0)
                 {
                     battleUnit.ShowPoison(false);
                     battleUnit.Character.CureStatus(ConditionID.psn);
                 }
                 else
                 {
                     battleUnit.Character.PoisonTime--;

                     battleUnit.Character.UpdateHP(battleUnit.Character.MaxHp / 8);
                 }

                 
             }
        }
        },
        {ConditionID.stn,
        new Condition()
        {
             Name = "Stun",
             StartMessage = "has been stunned",
             OnStart = (BattleUnit battleUnit, int minTurns, int maxTurns) =>
             {
                 battleUnit.Character.StunTime = Random.Range(minTurns,maxTurns);
                 battleUnit.ShowStun(true);
                 Debug.Log($"Stunned for {battleUnit.Character.StunTime} turns");
             },
             OnBeforeMove = (BattleUnit battleUnit) =>
             {
                 if(battleUnit.Character.StunTime <= 0)
                 {
                     battleUnit.Character.CureStatus(ConditionID.stn);
                     battleUnit.ShowStun(false);
                     return true;
                 }

                 battleUnit.Character.StunTime--;

                 return false;
             }
        }
        },
        {ConditionID.tnk,
        new Condition()
        {
             Name = "Taunt",
             StartMessage = "has taunted enemy team",
             OnStart = (BattleUnit battleUnit, int minTurns, int maxTurns) =>
             { 
                 battleUnit.Character.TauntTime = Random.Range(minTurns,maxTurns);
                 Debug.Log($"Taunt for {battleUnit.Character.TauntTime} turns");
             },
             OnBeforeTurn = (BattleUnit battleUnit) =>
             {
                 if(battleUnit.Character.TauntTime <= 0)
                 {
                     battleUnit.Character.CureStatus(ConditionID.tnk);
                 }
                 else
                 {
                     battleUnit.Character.TauntTime--;
                 }


             }
        }
        }
    };

    public static Dictionary<ConditionID, Condition> Conditions { get => conditions; set => conditions = value; }
}

public enum ConditionID
{
    none, psn, stn, tnk
}