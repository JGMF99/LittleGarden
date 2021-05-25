using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecruitmentGiver : MonoBehaviour
{
    [SerializeField] Text title;
    [SerializeField] new Text name;
    [SerializeField] Image img;
    [SerializeField] Button okBtn;

    public Recruitment recruitment;

    public GameObject newCharacterWindow;
    public event Action OnNewCharacterWindowClose;

    internal void OpenNewCharacterWindow(PlayerController playerController)
    {
        newCharacterWindow.SetActive(true);

        CharacterParty characterParty = playerController.GetComponent<CharacterParty>();

        if(characterParty.Team.Count < 4)
        {
            name.gameObject.SetActive(true);
            img.gameObject.SetActive(true);

            title.text = "New Team Element";
            name.text = recruitment.Character.name;
            img.sprite = recruitment.Character.FrontSprite;

            Character character = new Character();
            character.Base = recruitment.Character;
            character.Level = 1;
            character.Position = characterParty.GetFirstAvailablePosition();
            character.Init();
            characterParty.Team.Add(character);
        }
        else
        {
            name.gameObject.SetActive(false);
            img.gameObject.SetActive(false);

            title.text = "You already have 4 elements in the team! Can't add new element :(";
        }

        okBtn.onClick.AddListener(() => OnNewCharacterWindowClose.Invoke());
    }
}
