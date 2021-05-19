using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;

    public void Interact()
    {

        QuestGiver qg = GetComponent<QuestGiver>();

        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, qg));
    }
}
