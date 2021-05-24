using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPanel : MonoBehaviour
{

    [SerializeField] List<QuestBox> questBoxes;

    internal void Setup(List<Quest> quests)
    {
        for(var i = 0; i < questBoxes.Count; i++)
        {
            if (i < quests.Count && quests[i].questState == QuestState.OnGoing)
                questBoxes[i].Setup(quests[i]);
            else
                questBoxes[i].Setup(null);
        }
    }
}
