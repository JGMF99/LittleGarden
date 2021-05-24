using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    private List<Item> playerItems;
    private List<Item> playerItemsNotEquipped;

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
    [SerializeField] CharacterEquipment characterEquipment;

    [SerializeField] Text moneyTxt;
    [SerializeField] Text healthPotsTxt;
    [SerializeField] Text cooldownPotsTxt;

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

        GetNonEquippedItems();
        bodyAndHelmetMenu.Setup(playerItemsNotEquipped);

        moneyTxt.text = pc.Money.ToString();
        healthPotsTxt.text = "Health Potion x" + pc.GetItemQuantity("Health Potion").ToString();
        cooldownPotsTxt.text = "Cooldown Reduction Potion x" + pc.GetItemQuantity("Cooldown Reduction Potion").ToString();

        CharacterClicked(0);
    }

    

    void CharacterClicked(int characterNo)
    {
        Debug.Log("Character clicked = " + characterNo);

        selectedCharacter = characterNo;

        characterInfo.UpdateSelectedCharacter(CharacterParty.Team[characterNo]);

        characterEquipment.UpdateSelectedCharacterEquipment(CharacterParty.Team[characterNo]);
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

    public void UpdateChestPieces()
    {
        GetNonEquippedItems();
        bodyAndHelmetMenu.Setup(playerItemsNotEquipped);
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
}

