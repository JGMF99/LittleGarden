using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] GameObject NPCFace;
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;

    private QuestGiver quest;
    private RecruitmentGiver recruitment;
    public static DialogManager Instance { get; private set; }
    public QuestGiver Quest { get => quest; set => quest = value; }
    public RecruitmentGiver Recruitment { get => recruitment; set => recruitment = value; }

    private void Awake()
    {
        Instance = this;
    }

    Dialog dialog;
    int currentLine = 0;
    bool isTyping;

    public IEnumerator ShowDialog(Dialog dialog, QuestGiver qg, RecruitmentGiver rg)
    {
        yield return new WaitForEndOfFrame();

        Quest = null;
        Recruitment = null;

        OnShowDialog?.Invoke();

        if(qg != null && qg.quest.questState == QuestState.notStarted)
            Quest = qg;
        if (rg != null)
            Recruitment = rg;

        this.dialog = dialog;
        NPCFace.SetActive(true);
        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }
    public void HandleUpdate()
    {
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.M)) && !isTyping)
        {
            ++currentLine;
            if (currentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
            }
            else
            {
                currentLine = 0;
                dialogBox.SetActive(false);
                NPCFace.SetActive(false);
                OnCloseDialog?.Invoke();
            }
        }
    }

    public IEnumerator TypeDialog(string dialog)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
    }
}
