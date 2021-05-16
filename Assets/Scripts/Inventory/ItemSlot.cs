using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ItemSlot : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    private Item item;

    [SerializeField] Canvas canvas;

    [SerializeField] Image itemImg;
    [SerializeField] Image background;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector2 initPosition;
    private bool droppedOnSlot;

    public bool DroppedOnSlot { get => droppedOnSlot; set => droppedOnSlot = value; }
    public Image ItemImg { get => itemImg; set => itemImg = value; }
    public Item Item { get => item; set => item = value; }

    public void Setup(Item item)
    {
        this.Item = item;
        if(item != null)
        {
            ItemImg.GetComponent<Image>().color = Color.white;
            ItemImg.GetComponent<Image>().sprite = item.Base.Sprite;
        }
        else
        {
            ItemImg.GetComponent<Image>().color = Color.clear;
            ItemImg.GetComponent<Image>().sprite = null;
        }

        
    }

    private void Awake()
    {
        rectTransform = ItemImg.GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Item != null)
        {
            Debug.Log("OnBeginDrag");
            initPosition = rectTransform.anchoredPosition;
            canvasGroup.alpha = .6f;
            canvasGroup.blocksRaycasts = false;
            eventData.pointerDrag.GetComponent<ItemSlot>().DroppedOnSlot = false;
        }
            
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        rectTransform.anchoredPosition = initPosition;
        
        if(DroppedOnSlot)
        {
            item.IsEquipped = true;

            Inventory.instance.UpdateChestPieces();
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(Item != null)
            Debug.Log("OnPointerDown");
    }

}
