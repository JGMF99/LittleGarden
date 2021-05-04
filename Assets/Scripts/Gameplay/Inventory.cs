using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Creating multiple Instances of Inventory");
            return;
        }
        instance = this;
    }

    public List<Item> helmets = new List<Item>();
    public List<Item> chestPlates = new List<Item>();
    public List<Item> trinkets = new List<Item>();


    public void AddItem(Item item)
    {
        switch (item.Slot)
        {
            case "Helmet":
                helmets.Add(item);
                break;
            case "Trinket":
                trinkets.Add(item);
                break;
            case "Chest":
                chestPlates.Add(item);
                break;
            default:
                Debug.Log("Error adding item to inventory");
                break;

        }
    }

    public void RemoveItem(Item item)
    {
        switch (item.Slot)
        {
            case "Helmet":
                helmets.Remove(item);
                break;

            case "Trinket":
                trinkets.Remove(item);
                break;
            
            case "Chest":
                chestPlates.Remove(item);
                break;
            
            default:
                Debug.Log("Error removing item from inventory");
                break;

        }
    }
}

