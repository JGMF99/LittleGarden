using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum QuestState {notStarted, OnGoing, Completed }


[System.Serializable]
public class Quest
{
    public QuestState questState;

    public string title;
    public string description;

    public int experienceReward;
    public int moneyReward;
    public List<Item> itemsReward;

    public QuestGoal goal;

    public void Complete()
    {
        AudioManager.instance.Play("SoundConfirm02");
        questState = QuestState.Completed;
        Debug.Log(title + " was completed");
    }
}
