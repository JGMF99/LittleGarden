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
    [SerializeField] GameObject enemySelector;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> skillText;

    [SerializeField] List<BattleUnit> enemyImages;

    [SerializeField] Text descriptionText;

    public List<BattleUnit> EnemyImages { get => enemyImages; set => enemyImages = value; }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableSkillSelector(bool enabled)
    {
        skillSelector.SetActive(enabled);
    }

    public void EnableSkillDetails(bool enabled)
    {
        skillDetails.SetActive(enabled);
    }

    public void EnablePlayerHud(bool enabled)
    {
        playerHud.SetActive(enabled);
    }

    public void EnableEnemySelector(bool enabled)
    {
        enemySelector.SetActive(enabled);
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

    public void UpdateEnemySelection(int selectedEnemy)
    {
        for (int i = 0; i < enemyImages.Count; i++)
        {
            if (i == selectedEnemy)
            {
                enemyImages[i].GetComponent<Image>().color = new Color(enemyImages[i].GetComponent<Image>().color.r, 
                    enemyImages[i].GetComponent<Image>().color.g, enemyImages[i].GetComponent<Image>().color.b, 0.5f);
            }
            else if(enemyImages[i].GetComponent<Image>().color != Color.clear)
            {
                enemyImages[i].GetComponent<Image>().color = new Color(enemyImages[i].GetComponent<Image>().color.r, 
                    enemyImages[i].GetComponent<Image>().color.g, enemyImages[i].GetComponent<Image>().color.b, 1f);
            }
        }

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

    public void SetEnemyImages(List<BattleUnit> enemies)
    {
        Debug.Log(enemies.Count);
        for (int i = 0; i < 4; i++)
        {
            if(i < enemies.Count)
                enemyImages[i].SetupMenu(enemies[i].Character);
            else
                enemyImages[i].SetupMenu(null);

        }


    }
}
