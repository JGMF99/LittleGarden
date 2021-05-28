using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestPanel : MonoBehaviour
{

    [SerializeField] List<QuestBox> questBoxes;
    [SerializeField] Text noQuestsAvailable;

    internal void Setup(List<Quest> quests)
    {
        noQuestsAvailable.gameObject.SetActive(false);

        bool onGoingQuests = false;
        for (var i = 0; i < questBoxes.Count; i++)
        {
            if (i < quests.Count && quests[i].questState == QuestState.OnGoing)
            {
                onGoingQuests = true;
                questBoxes[i].Setup(quests[i]);
            }
            else
                questBoxes[i].Setup(null);
        }
        
        if(!onGoingQuests)
            noQuestsAvailable.gameObject.SetActive(true);
    }
}
