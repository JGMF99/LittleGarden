using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestBox : MonoBehaviour
{
    [SerializeField] Text title;
    [SerializeField] Text description;
    [SerializeField] Text goal;

    private Quest quest;

    internal void Setup(Quest quest)
    {
        if(quest != null)
        {
            gameObject.SetActive(true);
            title.text = quest.title;
            description.text = quest.description;
            goal.text = quest.goal.currentAmount.ToString() + "/" + quest.goal.requiredAmount.ToString();
        }else
            gameObject.SetActive(false);

    }
}
