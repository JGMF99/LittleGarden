using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestGiver : MonoBehaviour
{
    [SerializeField] Button acceptButton;
    [SerializeField] Button declineButton;

    public Quest quest;

    public PlayerController player;

    public GameObject questWindow;
    public Text titleText;
    public Text descriptionText;
    //public Text experienceText;
    //public Text moneyText;
    //List items

    public event Action OnQuestWindowClose;

    public void OpenQuestWindow()
    {
        questWindow.SetActive(true);

        titleText.text = quest.title;
        descriptionText.text = quest.description;
        //experienceText.text = quest.experienceReward.ToString();
        //moneyText.text = quest.moneyReward.ToString();

        acceptButton.onClick.RemoveAllListeners();
        acceptButton.onClick.AddListener(() => AcceptQuest());
        declineButton.onClick.AddListener(() => DeclineQuest());

    }

    public void AcceptQuest()
    {
        questWindow.SetActive(false);

        quest.questState = QuestState.OnGoing;

        player.Quests.Add(quest);

        AudioManager.instance.Play("SoundConfirm01");

        OnQuestWindowClose.Invoke();
    }

    public void DeclineQuest()
    {
        questWindow.SetActive(false);

        OnQuestWindowClose.Invoke();
    }

}
