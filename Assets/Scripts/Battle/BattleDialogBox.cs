using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{

    [SerializeField] Color highlightedColor;

    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject skillSelector;
    [SerializeField] GameObject skillDetails;
    [SerializeField] GameObject playerHud;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> skillText;

    [SerializeField] Text descriptionText;

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableSkillSelector(bool enabled)
    {
        skillSelector.SetActive(enabled);
        skillDetails.SetActive(enabled);
    }

    public void EnablePlayerHud(bool enabled)
    {
        playerHud.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for(int i = 0; i < actionTexts.Count; i++)
        {
            if(i == selectedAction)
            {
                actionTexts[i].color = highlightedColor;
            }
            else
            {
                actionTexts[i].color = Color.black;
            }
        }
    }

    public void UpdateSkillSelection(int selectedSkill, Skill skill)
    {
        for (int i = 0; i < skillText.Count; i++)
        {
            if (i == selectedSkill)
            {
                skillText[i].color = highlightedColor;
            }
            else
            {
                skillText[i].color = Color.black;
            }
        }

        descriptionText.text = skill.Base.Description;
    }

    public void SetSkillNames(List<Skill> skills)
    {

        for(int i = 0; i < skillText.Count; i++)
        {

            if (i < skills.Count)
                skillText[i].text = skills[i].Base.Name;
            else
                skillText[i].text = "-";

        }

    }
}
