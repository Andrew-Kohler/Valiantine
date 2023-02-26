using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory System/Item")]
public class ItemData : ScriptableObject
{
    public Sprite Icon;
    public int MaxStackSize;
    public string DisplayName;

    [TextArea(1, 4)] public string InventoryDescription;
    [TextArea(1, 4)] public string UseDescription;
    [TextArea(1, 4)] public string BattleUseDescription;
}
