using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[CreateAssetMenu(menuName = "Inventory System/Item")]
public class ItemData : ScriptableObject
{
    public Sprite MenuIcon;
    public Sprite OverworldIcon;
    public int MaxStackSize;
    public string DisplayName;

    public bool Consumable;
    public int HPRestore;
    public int MPRestore;

    [TextArea(1, 4)] public string InventoryDescription;
    [TextArea(1, 4)] public string UseDescription;
    [TextArea(1, 4)] public string BattleUseDescription;
    [TextArea(1, 4)] public string SingleObtainDescription;
    [TextArea(1, 4)] public string MultipleObtainDescription;
}
