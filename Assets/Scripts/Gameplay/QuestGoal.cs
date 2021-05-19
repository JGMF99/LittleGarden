using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestGoal
{

    public GoalType goalType;

    public int requiredAmount;
    public int currentAmount;

    public CharactersBase enemyToKill;

    public bool IsReached()
    {
        return currentAmount >= requiredAmount;
    }

    public void EnemyKilled(CharactersBase enemyKilled)
    {
        if (goalType == GoalType.Kill && enemyKilled == enemyToKill)
            currentAmount++;
    }

    public void ItemCollected()
    {
        if (goalType == GoalType.Get)
            currentAmount++;
    }

}

public enum GoalType
{
    Talk,
    Kill,
    GoTo,
    Get
}
