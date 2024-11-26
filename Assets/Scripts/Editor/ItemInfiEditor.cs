using Codice.CM.Common;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemInfoSO))]
public class ItemInfiEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ItemInfoSO itemInfoSO = (ItemInfoSO)target;

        EditorGUILayout.LabelField("Info", EditorStyles.boldLabel);
        itemInfoSO.id = EditorGUILayout.IntField("ID", itemInfoSO.id);
        itemInfoSO.itemSprite = (Sprite)EditorGUILayout.ObjectField("Item Sprite", itemInfoSO.itemSprite, typeof(Sprite), false);
        itemInfoSO.itemName = EditorGUILayout.TextField("Item Name", itemInfoSO.itemName);
        itemInfoSO.itemDescription = EditorGUILayout.TextField("Item Description", itemInfoSO.itemDescription);
        
        EditorGUILayout.LabelField("DropChance", EditorStyles.boldLabel);
        itemInfoSO.dropChance = EditorGUILayout.FloatField("Drop Chance", itemInfoSO.dropChance);
        itemInfoSO.dropChanceInChest = EditorGUILayout.FloatField("Drop Chance In Chest", itemInfoSO.dropChanceInChest);

        EditorGUILayout.LabelField("ItemType", EditorStyles.boldLabel);
        itemInfoSO.itemType = (ItemInfoSO.ItemType)EditorGUILayout.EnumPopup("Item Type", itemInfoSO.itemType);

        switch (itemInfoSO.itemType)
        {
            case ItemInfoSO.ItemType.Consumables:
                EditorGUILayout.LabelField("Consumables Type", EditorStyles.boldLabel);
                itemInfoSO.consumablesType = (ItemInfoSO.ConsumablesType)EditorGUILayout.EnumPopup("ConsumablescType", itemInfoSO.consumablesType);

                EditorGUILayout.LabelField("Value", EditorStyles.boldLabel);
                itemInfoSO.plus = EditorGUILayout.IntField("Plus", itemInfoSO.plus);
                break;
            case ItemInfoSO.ItemType.Equipment:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("stats"), true);

                EditorGUILayout.LabelField("Equipment Type", EditorStyles.boldLabel);
                itemInfoSO.equipmentType = (ItemInfoSO.EquipmentType)EditorGUILayout.EnumPopup("Equipment Type", itemInfoSO.equipmentType);

                switch (itemInfoSO.equipmentType)
                {
                    case ItemInfoSO.EquipmentType.Weapon:
                        EditorGUILayout.LabelField("Attack Type", EditorStyles.boldLabel);
                        itemInfoSO.attackType = (ItemInfoSO.AttackType)EditorGUILayout.EnumPopup("Attack Type", itemInfoSO.attackType);

                        switch (itemInfoSO.attackType)
                        {
                            case ItemInfoSO.AttackType.Range:
                                itemInfoSO.projectile = (GameObject)EditorGUILayout.ObjectField("Projectile", itemInfoSO.projectile, typeof(GameObject), false);
                                break;
                        }
                        break;
                }
                break;
            case ItemInfoSO.ItemType.Item:

                break;
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
