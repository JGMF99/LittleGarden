using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecruitmentCompleted : MonoBehaviour
{
    [SerializeField] Image img;
    [SerializeField] Text characterName;
    [SerializeField] Button okBtn;

    public Button OkBtn { get => okBtn; set => okBtn = value; }

    internal void CompletedRecruitment(Recruitment r)
    {
        gameObject.SetActive(true);

        img.GetComponent<Image>().sprite = r.Character.FrontSprite;
        characterName.text = r.Character.Name;
    }
}
