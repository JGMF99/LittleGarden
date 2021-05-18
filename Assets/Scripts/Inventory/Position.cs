using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Position : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{

    [SerializeField] Canvas canvas;

    private int position;
    private Character character;

    private Vector2 initPosition;

    private bool droppedOnSlot;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    public Character Character { get => character; set => character = value; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    internal void Setup(Character character, int position)
    {

        this.position = position;

        this.Character = character;

        if (character != null)
        {
            GetComponent<Image>().color = Color.white;
            GetComponent<Image>().sprite = character.Base.SideSprite;
        }
        else
        {
            GetComponent<Image>().color = Color.clear;
            GetComponent<Image>().sprite = null;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Character != null)
        {
            Debug.Log("OnBeginDrag");
            initPosition = rectTransform.anchoredPosition;
            canvasGroup.alpha = .6f;
            canvasGroup.blocksRaycasts = false;
            droppedOnSlot = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");


        if (eventData.pointerDrag != null)
        {

            Position droppedCharacter = eventData.pointerDrag.GetComponent<Position>();

            if(droppedCharacter.position != position)
            {

                if(this.Character == null)
                {
                    droppedCharacter.Character.Position = position;

                    Setup(droppedCharacter.Character, position);
                    droppedCharacter.Setup(null, droppedCharacter.position);
                }
                else
                {
                    int oldPositon = droppedCharacter.Character.Position;
                    droppedCharacter.Character.Position = character.Position;
                    character.Position = oldPositon;

                    Character tmpCharacter = character;
                    Setup(droppedCharacter.Character, position);
                    droppedCharacter.Setup(tmpCharacter, droppedCharacter.position);

                }

                droppedCharacter.droppedOnSlot = true;

            }

        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if(Character != null)
            Debug.Log("OnPointerDown");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
   
        Debug.Log("OnEndDrag");
        rectTransform.anchoredPosition = initPosition;

        if (droppedOnSlot)
        {
            
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
    }
}
