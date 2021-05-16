using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMember : MonoBehaviour
{

    [SerializeField] new Text name;
    [SerializeField] Image img;

    public void Setup(Character character)
    {

        if(character != null){
            name.text = character.Base.Name;

            img.GetComponent<Image>().color = Color.white;
            img.GetComponent<Image>().sprite = character.Base.FrontSprite;

            GetComponent<Button>().enabled = true;

        }
        else
        {
            name.text = "";

            img.GetComponent<Image>().color = Color.clear;

            GetComponent<Button>().enabled = false;
        }


    }

}
