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
             OnStart = (BattleUnit battleUnit) =>
             {
                 //Stun for 1-2 turns
                 battleUnit.Character.PoisonTime = Random.Range(1,4);
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
             OnStart = (BattleUnit battleUnit) =>
             {
                 //Stun for 1-2 turns
                 battleUnit.Character.StunTime = Random.Range(1,3);
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
        }
    };

    public static Dictionary<ConditionID, Condition> Conditions { get => conditions; set => conditions = value; }
}

public enum ConditionID
{
    none, psn, stn
}