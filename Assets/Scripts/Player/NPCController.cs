using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;

    public void Interact()
    {

        QuestGiver qg = GetComponent<QuestGiver>();
        RecruitmentGiver rg = GetComponent<RecruitmentGiver>();
        Shopper s = GetComponent<Shopper>();
        Boss b = GetComponent<Boss>();

        AudioManager.instance.Play("SoundCancel01");

        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, qg, rg, s, b));
    }
}
