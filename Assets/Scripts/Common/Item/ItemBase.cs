using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アイテムのマスターデータ
/// </summary>
public class ItemBase : ScriptableObject
{
    [SerializeField] private string itemName;

    [TextArea]
    [SerializeField] private string description;

    [SerializeField] private int id;
    [SerializeField] private ItemType itemType;
    public ItemType ItemType { get => itemType; }


    public string ItemName { get => itemName; }
    public string Description { get => description; }
    public int Id { get => id; set => id = value; }
}