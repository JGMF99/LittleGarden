using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Position : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{

    [SerializeField] Canvas canvas;
    [SerializeField] Color color;
    [SerializeField] GameObject tmpGameObject;

    private bool dragStarted;

    private int position;
    private Character character;

    private Vector2 initPosition;
    private Vector2 tmpPosition;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    public event Action<Position> OnDragStart;
    public event Action OnDragEnd;

    public Character Character { get => character; set => character = value; }
    public Color Color { get => color; set => color = value; }

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
        dragStarted = false;
        if (Character != null)
        {
            Debug.Log("OnBeginDrag");
            initPosition = rectTransform.anchoredPosition;

            canvasGroup.alpha = .9f;
            canvasGroup.blocksRaycasts = false;

            int tmpSiblingIndex = transform.GetSiblingIndex();
            transform.SetSiblingIndex(tmpGameObject.transform.GetSiblingIndex());
            tmpGameObject.transform.SetSiblingIndex(tmpSiblingIndex);

            tmpPosition = initPosition;
            rectTransform.anchoredPosition = tmpPosition;

            dragStarted = true;

            OnDragStart(this);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(Character != null)
        {
            tmpPosition += eventData.delta / canvas.scaleFactor;
            rectTransform.anchoredPosition = tmpPosition;
        }
            
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
                else if(droppedCharacter.Character != null)
                {
                    int oldPositon = droppedCharacter.Character.Position;
                    droppedCharacter.Character.Position = character.Position;
                    character.Position = oldPositon;

                    Character tmpCharacter = character;
                    Setup(droppedCharacter.Character, position);
                    droppedCharacter.Setup(tmpCharacter, droppedCharacter.position);

                }


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
        if (dragStarted)
        {
            Debug.Log("OnEndDrag");

            rectTransform.anchoredPosition = initPosition;

            int tmpSiblingIndex = transform.GetSiblingIndex();
            transform.SetSiblingIndex(tmpGameObject.transform.GetSiblingIndex());
            tmpGameObject.transform.SetSiblingIndex(tmpSiblingIndex);

            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;

            OnDragEnd();

        }


    }
}
