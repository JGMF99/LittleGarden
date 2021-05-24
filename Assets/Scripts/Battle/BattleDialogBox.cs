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
    [SerializeField] GameObject itemSelector;
    [SerializeField] GameObject itemDetails;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> skillText;

    [SerializeField] List<BattleUnit> enemyImages;

    [SerializeField] Text descriptionText;
    [SerializeField] Text cooldownText;

    [SerializeField] List<BattleItem> battleItems;
    [SerializeField] Text itemName;
    [SerializeField] Text itemDescription;

    public List<BattleUnit> EnemyImages { get => enemyImages; set => enemyImages = value; }
    public List<BattleItem> BattleItems { get => battleItems; set => battleItems = value; }
    public List<Text> ActionTexts { get => actionTexts; set => actionTexts = value; }
    public List<Text> SkillText { get => skillText; set => skillText = value; }

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

    public void EnableItemSelector(bool enabled)
    {
        itemSelector.SetActive(enabled);
    }

    public void EnableItemDetails(bool enabled)
    {
        itemDetails.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for(int i = 0; i < ActionTexts.Count; i++)
        {
            if(i == selectedAction)
            {
                ActionTexts[i].color = highlightedColor;
            }
            else
            {
                ActionTexts[i].color = Color.black;
            }
        }
    }

    public void UpdateSkillSelection(int selectedSkill, Skill skill)
    {
        for (int i = 0; i < SkillText.Count; i++)
        {
            if (i == selectedSkill)
            {
                SkillText[i].color = highlightedColor;
            }
            else
            {
                SkillText[i].color = Color.black;
            }
        }

        descriptionText.text = skill.Base.Description;
        cooldownText.text = "Cooldown: " + skill.CurrentTurnCooldown.ToString();
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

    public void UpdateItemSelection(int item)
    {
        for(var i = 0; i < BattleItems.Count; i++)
        {
            BattleItems[i].Image.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }

        BattleItems[item].Image.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);

        itemName.text = BattleItems[item].ItemBase.Name;
        itemDescription.text = BattleItems[item].ItemBase.Description;

    }
    public void SetSkillNames(List<Skill> skills)
    {

        for(int i = 0; i < SkillText.Count; i++)
        {

            if (i < skills.Count)
                SkillText[i].text = skills[i].Base.Name;
            else
                SkillText[i].text = "-";

        }

    }

    public void SetEnemyImages(List<BattleUnit> enemies)
    {

        for (int i = 0; i < 4; i++)
        {
            if(i < enemies.Count)
                enemyImages[i].SetupMenu(enemies[i].Character);
            else
                enemyImages[i].SetupMenu(null);

        }


    }

    public void SetItemsMenu()
    {
        foreach(BattleItem bi in battleItems)
        {
            bi.QuantityTxt.text = "x" + bi.Items.Count.ToString();
        }
    }
}
