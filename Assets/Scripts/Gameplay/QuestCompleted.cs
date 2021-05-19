using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestCompleted : MonoBehaviour
{

    [SerializeField] Text title;
    [SerializeField] Text description;
    [SerializeField] Text money;
    [SerializeField] Text xp;
    [SerializeField] List<ItemReceived> items;

    [SerializeField] Button okBtn;

    public Button OkBtn { get => okBtn; set => okBtn = value; }

    internal void CompletedQuest(Quest q)
    {
        gameObject.SetActive(true);

        title.text = q.title;
        description.text = q.description;
        money.text = q.moneyReward.ToString();
        xp.text = q.experienceReward.ToString();

        List<ItemBase> tmpItems = new List<ItemBase>();
        List<int> tmpNitems = new List<int>();

        foreach (var item in q.itemsReward)
        {
            if (tmpItems.Contains(item.Base))
            {
                tmpNitems[tmpItems.IndexOf(item.Base)]++;
            }
            else
            {
                tmpItems.Add(item.Base);
                tmpNitems.Add(1);
            }
        }

        for (var i = 0; i < items.Count; i++)
        {
            if (q.itemsReward != null && i < tmpItems.Count)
                items[i].Setup(tmpItems[i], tmpNitems[i]);
            else
                items[i].Setup(null, 0);
        }
    }
}
