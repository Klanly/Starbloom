using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_CraftingDictionaryItem : MonoBehaviour {

    [HideInInspector] public int DatabaseID;
    [HideInInspector] public bool LockItem;

    public string Name;
    public int ItemCreatedRef;

    public Ingredient[] IngredientList;

    [System.Serializable]
    public class Ingredient
    {
        [Header("---------------------------------------------------")]
        public string Name;
        public int ItemDatabaseRef;
        public int Value;


#if UNITY_EDITOR
        [Button(ButtonSizes.Small)] void SyncIngredientNameToItemName() { Name = QuickFindInEditor.GetEditorItemDatabase().GetItemFromID(ItemDatabaseRef).name; }
#endif
    }
#if UNITY_EDITOR
    [Button(ButtonSizes.Small)] void SyncCraftNameToCreatedItem() { Name = QuickFindInEditor.GetEditorItemDatabase().GetItemFromID(ItemCreatedRef).ModelPrefab.name; }
#endif
}
