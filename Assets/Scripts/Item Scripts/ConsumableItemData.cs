using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Consumable Item")]
public class ConsumableItemData : ItemData
{
    public int HPRestore;
    public int ManaRestore;
    // Hmm ... temp buff items? Not at all necessary, but if I have time to kill in the future, sure
}
