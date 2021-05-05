using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    int attackMod;
    int defMod;
    public string Slot { get; private set; }

    public Item()
    {
        attackMod = 3;
        defMod = 4;
        Slot = "Helmet";
    }



}
