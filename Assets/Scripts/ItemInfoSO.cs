using UnityEngine;

[CreateAssetMenu(fileName = "ItemInfo", menuName = "CreateItemInfo/Item")]
public class ItemInfoSO : ScriptableObject
{
    public enum ItemType
    {
        Consumables,
        Equipment,
        Item
    }

    public ItemType itemType;

    public enum ConsumablesType
    {
        HealthPotion
    }

    public ConsumablesType consumablesType;

    public enum EquipmentType
    {
        None,
        Weapon,
        Helmet,
        Bib,
        Pants,
        Slippers,
        Gloves
    }

    public EquipmentType equipmentType;

    public enum AttackType
    {
        None,
        Melle,
        Range
    }

    public AttackType attackType;

    public int id;

    public GameObject projectile;

    [Header("Info")]
    public Sprite itemSprite;
    public string itemName;
    public string itemDescription;

    [Header("Stats")]
    public Stats stats = new();

    [Header("Potion")]
    public int plus;

    [Header("DropChance")]
    public float dropChance;
    public float dropChanceInChest;
}
