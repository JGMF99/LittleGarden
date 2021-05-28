using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    private List<Item> playerItems;
    private List<Item> playerItemsNotEquipped;
    private List<Item> playerTrinketNotEquipped;

    private CharacterParty characterParty;

    public Button character1btn;
    public Button character2btn;
    public Button character3btn;
    public Button character4btn;

    private int selectedCharacter;

    [SerializeField] PlayerController pc;

    [SerializeField] CharacterInfo characterInfo;
    [SerializeField] List<PartyMember> partyMembers;
    [SerializeField] EquipmentMenu bodyAndHelmetMenu;
    [SerializeField] EquipmentMenu trinketMenu;
    [SerializeField] CharacterEquipment characterEquipment;

    [SerializeField] Text moneyTxt;
    [SerializeField] Text healthPotsTxt;
    [SerializeField] Text cooldownPotsTxt;

    [SerializeField] Button equipmentUpArrow;
    [SerializeField] Button equipmentDownArrow;
    [SerializeField] Button trinketUpArrow;
    [SerializeField] Button trinketDownArrow;

    [SerializeField] Button usePotBtn;

    private int equipmentIndexStart;
    private int trinketIndexStart;

    public CharacterParty CharacterParty { get => characterParty; set => characterParty = value; }

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Creating multiple Instances of Inventory");
            return;
        }
        instance = this;

        character1btn.onClick.AddListener(() => CharacterClicked(0));
        character2btn.onClick.AddListener(() => CharacterClicked(1));
        character3btn.onClick.AddListener(() => CharacterClicked(2));
        character4btn.onClick.AddListener(() => CharacterClicked(3));

        equipmentUpArrow.onClick.AddListener(OnClickEquipmentUpArrow) ;
        equipmentDownArrow.onClick.AddListener(OnClickEquipmentDownArrow);
        trinketUpArrow.onClick.AddListener(OnClickTrinketUpArrow);
        trinketDownArrow.onClick.AddListener(OnClickTrinketDownArrow);

        usePotBtn.onClick.AddListener(UseHealthPotion);
    }

   

    public void Setup(CharacterParty team, List<Item> playerItems)
    {
        CharacterParty = team;

        for(var i = 0; i < partyMembers.Count; i++)
        {
            if (i < CharacterParty.Team.Count)
                partyMembers[i].Setup(CharacterParty.Team[i]);
            else
                partyMembers[i].Setup(null);     
        }

        this.playerItems = playerItems;

        equipmentIndexStart = 0;
        trinketIndexStart = 0;

        GetNonEquippedItems();
        bodyAndHelmetMenu.Setup(playerItemsNotEquipped, equipmentIndexStart);
        GetNonEquippedTrinkets();
        trinketMenu.Setup(playerTrinketNotEquipped, trinketIndexStart);

        CheckArrows();

        moneyTxt.text = pc.Money.ToString();
        healthPotsTxt.text = "Health Potion x" + pc.GetItemQuantity("Health Potion").ToString();
        cooldownPotsTxt.text = "Cooldown Reduction Potion x" + pc.GetItemQuantity("Cooldown Reduction Potion").ToString();

        CharacterClicked(0);
    }

    

    void CharacterClicked(int characterNo)
    {
        Debug.Log("Character clicked = " + characterNo);

        selectedCharacter = characterNo;

        if(characterNo < CharacterParty.Team.Count)
        {
            characterInfo.UpdateSelectedCharacter(CharacterParty.Team[characterNo]);

            characterEquipment.UpdateSelectedCharacterEquipment(CharacterParty.Team[characterNo]);

            usePotBtn.gameObject.SetActive(true);
        }
        else
        {
            characterInfo.UpdateSelectedCharacter(null);

            characterEquipment.UpdateSelectedCharacterEquipment(null);

            usePotBtn.gameObject.SetActive(false);
        }
        
    }

    private void GetNonEquippedItems()
    {
        playerItemsNotEquipped = new List<Item>();

        foreach(var item in playerItems)
        {
            if (!item.IsEquipped && (item.Base.Type == ItemType.Helmet || item.Base.Type == ItemType.Body))
            {
                playerItemsNotEquipped.Add(item);
            }
                
        }
    }

    private void GetNonEquippedTrinkets()
    {
        playerTrinketNotEquipped = new List<Item>();

        foreach (var item in playerItems)
        {
            if (!item.IsEquipped && item.Base.Type == ItemType.Trinket )
            {
                playerTrinketNotEquipped.Add(item);
            }

        }
    }

    public void UpdateChestPieces()
    {
        GetNonEquippedItems();
        bodyAndHelmetMenu.Setup(playerItemsNotEquipped, equipmentIndexStart);
        GetNonEquippedTrinkets();
        trinketMenu.Setup(playerTrinketNotEquipped, trinketIndexStart);

        CheckArrows();
    }

    public void ChangeSelectedCharacterEquipment(Item item, ItemType type)
    {

        switch (type)
        {
            case ItemType.Helmet:
                CharacterParty.Team[selectedCharacter].ChangeHelmet(item);
                break;
            case ItemType.Body:
                CharacterParty.Team[selectedCharacter].ChangeChestplate(item);
                break;
            case ItemType.Trinket:
                CharacterParty.Team[selectedCharacter].ChangeTrinket(item);
                break;
        }

        characterInfo.UpdateSelectedCharacter(CharacterParty.Team[selectedCharacter]);


    }

    public void OnClickEquipmentUpArrow()
    {
        equipmentIndexStart -= 6;
        UpdateChestPieces();
    }

    public void OnClickEquipmentDownArrow()
    {
        equipmentIndexStart += 6;
        UpdateChestPieces();
    }

    public void OnClickTrinketUpArrow()
    {
        trinketIndexStart -= 6;
        UpdateChestPieces();
    }

    public void OnClickTrinketDownArrow()
    {
        trinketIndexStart += 6;
        UpdateChestPieces();
    }

    public void CheckArrows()
    {
        if (equipmentIndexStart == 0)
            equipmentUpArrow.gameObject.SetActive(false);
        else
            equipmentUpArrow.gameObject.SetActive(true);

        if (equipmentIndexStart + 6 < playerItemsNotEquipped.Count)
            equipmentDownArrow.gameObject.SetActive(true);
        else
            equipmentDownArrow.gameObject.SetActive(false);

        if (trinketIndexStart == 0)
            trinketUpArrow.gameObject.SetActive(false);
        else
            trinketUpArrow.gameObject.SetActive(true);

        if (trinketIndexStart + 6 < playerTrinketNotEquipped.Count)
            trinketDownArrow.gameObject.SetActive(true);
        else
            trinketDownArrow.gameObject.SetActive(false);
    }

    private void UseHealthPotion()
    {
        if(pc.GetItemQuantity("Health Potion") > 0)
        {
            Item healthPot = pc.GetHealthPotion();

            CharacterParty.Team[selectedCharacter].ApplyItem(healthPot);

            pc.Items.Remove(healthPot);

            characterInfo.UpdateSelectedCharacter(CharacterParty.Team[selectedCharacter]);
            healthPotsTxt.text = "Health Potion x" + pc.GetItemQuantity("Health Potion").ToString();
        }
    }
}

