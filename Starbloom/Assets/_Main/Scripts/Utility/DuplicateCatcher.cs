#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using Sirenix.OdinInspector;
using UnityEditor;



[ExecuteInEditMode]
public class DuplicateCatcher : MonoBehaviour
{
    public enum DatabaseType
    {
        Item,
        Character,
        DialogueTree,
        Word,
        Quest,
        FishingCompendium,
        ObjectCompendium,
        ShopsCompendium,
        CraftingCompendium,
        PrefabPool,
        FXCompendium,
        Enemy,
        MusicObject,
        SFXObject,
        ClothingObject,
        AnimationObject
    }

    public DatabaseType DBType;
    [HideInInspector] public int instanceID = 0;


    void Awake()
    {
        if (Application.isPlaying) return;

        if (instanceID != GetInstanceID())
        {
            if (instanceID == 0) instanceID = GetInstanceID();
            else
            {
                instanceID = GetInstanceID();
                if (instanceID < 0)
                {
                    Debug.Log("Duplicate Made, Generating New ID");
                    AssignNewID(DBType);
                }
            }
        }
    }

    [Button(ButtonSizes.Small)]
    public void DoDataCheck()
    {
        GiveAllID(DBType);
    }









    public class GenericDatabase
    {
        public Type ClassType;
        public object ObjectReference;
        public Transform TransformReference;
        public Type ItemClassType;
    }
    public class GenericItem
    {
        public Type ClassType;
        public object ObjectReference;
    }



    public GenericDatabase GetReflectionData(DatabaseType DBType)
    {
        GenericDatabase GD = new GenericDatabase();
        switch (DBType)
        {
            case DatabaseType.Item: { GD.ClassType = typeof(DG_ItemsDatabase); GD.TransformReference = QuickFindInEditor.GetEditorItemDatabase().transform; GD.ObjectReference = GD.TransformReference.GetComponent<DG_ItemsDatabase>(); GD.ItemClassType = typeof(DG_ItemObject); } break;
            case DatabaseType.Character: { GD.ClassType = typeof(DG_CharacterDatabase); GD.TransformReference = QuickFindInEditor.GetEditorCharacterDatabase().transform; GD.ObjectReference = GD.TransformReference.GetComponent<DG_CharacterDatabase>(); GD.ItemClassType = typeof(DG_CharacterObject); } break;
            case DatabaseType.DialogueTree: { GD.ClassType = typeof(DG_DialogueManager); GD.TransformReference = QuickFindInEditor.GetEditorDialogueManager().transform; GD.ObjectReference = GD.TransformReference.GetComponent<DG_DialogueManager>(); GD.ItemClassType = typeof(NodeLink); } break;
            case DatabaseType.Word: { GD.ClassType = typeof(DG_WordDatabase); GD.TransformReference = QuickFindInEditor.GetEditorWordDatabase().transform; GD.ObjectReference = GD.TransformReference.GetComponent<DG_WordDatabase>(); GD.ItemClassType = typeof(DG_WordObject); } break;
            case DatabaseType.Quest: { GD.ClassType = typeof(DG_QuestDatabase); GD.TransformReference = QuickFindInEditor.GetEditorQuestDatabase().transform; GD.ObjectReference = GD.TransformReference.GetComponent<DG_QuestDatabase>(); GD.ItemClassType = typeof(DG_QuestObject); } break;
            case DatabaseType.FishingCompendium: { GD.ClassType = typeof(DG_FishingCompendium); GD.TransformReference = QuickFindInEditor.GetEditorFishingCompendium().transform; GD.ObjectReference = GD.TransformReference.GetComponent<DG_FishingCompendium>(); GD.ItemClassType = typeof(DG_FishingAtlasObject); } break;
            case DatabaseType.ObjectCompendium: { GD.ClassType = typeof(DG_BreakableObjectsAtlas); GD.TransformReference = QuickFindInEditor.GetBreakableObjectsCompendium().transform; GD.ObjectReference = GD.TransformReference.GetComponent<DG_BreakableObjectsAtlas>(); GD.ItemClassType = typeof(DG_BreakableObjectItem); } break;
            case DatabaseType.ShopsCompendium: { GD.ClassType = typeof(DG_ShopAtlas); GD.TransformReference = QuickFindInEditor.GetShopCompendium().transform; GD.ObjectReference = GD.TransformReference.GetComponent<DG_ShopAtlas>(); GD.ItemClassType = typeof(DG_ShopAtlasObject); } break;
            case DatabaseType.CraftingCompendium: { GD.ClassType = typeof(DG_CraftingDictionary); GD.TransformReference = QuickFindInEditor.GetCraftingCompendium().transform; GD.ObjectReference = GD.TransformReference.GetComponent<DG_CraftingDictionary>(); GD.ItemClassType = typeof(DG_CraftingDictionaryItem); } break;
            case DatabaseType.PrefabPool: { GD.ClassType = typeof(DG_PrefabPoolDictionary); GD.TransformReference = QuickFindInEditor.GetPrefabPool().transform; GD.ObjectReference = GD.TransformReference.GetComponent<DG_PrefabPoolDictionary>(); GD.ItemClassType = typeof(DG_PrefabPoolItem); } break;
            case DatabaseType.FXCompendium: { GD.ClassType = typeof(DG_FXHandler); GD.TransformReference = QuickFindInEditor.GetFXHandler().transform; GD.ObjectReference = GD.TransformReference.GetComponent<DG_FXHandler>(); GD.ItemClassType = typeof(DG_FXObject); } break;
            case DatabaseType.Enemy: { GD.ClassType = typeof(DG_EnemyDatabase); GD.TransformReference = QuickFindInEditor.GetEnemyDatabase().transform; GD.ObjectReference = GD.TransformReference.GetComponent<DG_EnemyDatabase>(); GD.ItemClassType = typeof(DG_EnemyObject); } break;
            case DatabaseType.MusicObject: { GD.ClassType = typeof(DG_MusicDatabase); GD.TransformReference = QuickFindInEditor.GetMusicDatabase().transform; GD.ObjectReference = GD.TransformReference.GetComponent<DG_MusicDatabase>(); GD.ItemClassType = typeof(DG_MusicObject); } break;
            case DatabaseType.SFXObject: { GD.ClassType = typeof(DG_SFXDatabase); GD.TransformReference = QuickFindInEditor.GetSFXDatabase().transform; GD.ObjectReference = GD.TransformReference.GetComponent<DG_SFXDatabase>(); GD.ItemClassType = typeof(DG_SFXObject); } break;
            case DatabaseType.ClothingObject: { GD.ClassType = typeof(DG_ClothingDatabase); GD.TransformReference = QuickFindInEditor.GetClothingDatabase().transform; GD.ObjectReference = GD.TransformReference.GetComponent<DG_ClothingDatabase>(); GD.ItemClassType = typeof(DG_ClothingObject); } break;
            case DatabaseType.AnimationObject: { GD.ClassType = typeof(DG_AnimationDatabase); GD.TransformReference = QuickFindInEditor.GetAnimationDatabase().transform; GD.ObjectReference = GD.TransformReference.GetComponent<DG_AnimationDatabase>(); GD.ItemClassType = typeof(DG_AnimationObject); } break;
        }
        return GD;
    }
    public GenericItem GetReflectionItem(DatabaseType DBType, Transform T)
    {
        GenericItem TO = new GenericItem();
        switch (DBType)
        {
            case DatabaseType.Item: { TO.ClassType = typeof(DG_ItemObject); TO.ObjectReference = T.GetComponent<DG_ItemObject>(); } break;
            case DatabaseType.Character: { TO.ClassType = typeof(DG_CharacterObject); TO.ObjectReference = T.GetComponent<DG_CharacterObject>(); } break;
            case DatabaseType.DialogueTree: { TO.ClassType = typeof(NodeLink); TO.ObjectReference = T.GetComponent<NodeLink>(); } break;
            case DatabaseType.Word: { TO.ClassType = typeof(DG_WordObject); TO.ObjectReference = T.GetComponent<DG_WordObject>(); } break;
            case DatabaseType.Quest: { TO.ClassType = typeof(DG_QuestObject); TO.ObjectReference = T.GetComponent<DG_QuestObject>(); } break;
            case DatabaseType.FishingCompendium: { TO.ClassType = typeof(DG_FishingAtlasObject); TO.ObjectReference = T.GetComponent<DG_FishingAtlasObject>(); } break;
            case DatabaseType.ObjectCompendium: { TO.ClassType = typeof(DG_BreakableObjectItem); TO.ObjectReference = T.GetComponent<DG_BreakableObjectItem>(); } break;
            case DatabaseType.ShopsCompendium: { TO.ClassType = typeof(DG_ShopAtlasObject); TO.ObjectReference = T.GetComponent<DG_ShopAtlasObject>(); } break;
            case DatabaseType.CraftingCompendium: { TO.ClassType = typeof(DG_CraftingDictionaryItem); TO.ObjectReference = T.GetComponent<DG_CraftingDictionaryItem>(); } break;
            case DatabaseType.PrefabPool: { TO.ClassType = typeof(DG_PrefabPoolItem); TO.ObjectReference = T.GetComponent<DG_PrefabPoolItem>(); } break;
            case DatabaseType.FXCompendium: { TO.ClassType = typeof(DG_FXObject); TO.ObjectReference = T.GetComponent<DG_FXObject>(); } break;
            case DatabaseType.Enemy: { TO.ClassType = typeof(DG_EnemyObject); TO.ObjectReference = T.GetComponent<DG_EnemyObject>(); } break;
            case DatabaseType.MusicObject: { TO.ClassType = typeof(DG_MusicObject); TO.ObjectReference = T.GetComponent<DG_MusicObject>(); } break;
            case DatabaseType.SFXObject: { TO.ClassType = typeof(DG_SFXObject); TO.ObjectReference = T.GetComponent<DG_SFXObject>(); } break;
            case DatabaseType.ClothingObject: { TO.ClassType = typeof(DG_ClothingObject); TO.ObjectReference = T.GetComponent<DG_ClothingObject>(); } break;
            case DatabaseType.AnimationObject: { TO.ClassType = typeof(DG_AnimationObject); TO.ObjectReference = T.GetComponent<DG_AnimationObject>(); } break;
        }
        return TO;
    }


    #region Generic
    void AssignNewID(DatabaseType DBType)
    {
        //Item Data, and Variables that will be needed with this class type.
        GenericItem ItemObject = GetReflectionItem(DBType, this.transform);
        FieldInfo LockItem = ItemObject.ClassType.GetField("LockItem");

        LockItem.SetValue(ItemObject.ObjectReference, false);
        GiveAllID(DBType);
    }
    public void GiveAllID(DatabaseType DBType)
    {
        //Database Data, and Variables that will be needed with this class type.
        GenericDatabase ItemDatabase = GetReflectionData(DBType);
        FieldInfo ItemListCount = ItemDatabase.ClassType.GetField("ListCount");
        FieldInfo ItemList = ItemDatabase.ClassType.GetField("ItemCatagoryList");

        Transform ItemDatabaseRoot = ItemDatabase.TransformReference;
        List<object> AddObjects = new List<object>();
        AddSearchForLowestChild(ItemDatabaseRoot, ItemDatabase, AddObjects);

        if (AddObjects.Count == 0) { Debug.Log("All Items Accounted For. :)"); return; }

        int Index = 0;
        object[] NewArray = new object[(int)ItemListCount.GetValue(ItemDatabase.ObjectReference)];
        object[] CatagoryList = (object[])ItemList.GetValue(ItemDatabase.ObjectReference);
        for (int i = 0; i < CatagoryList.Length; i++)
        {
            object IO = CatagoryList[i];
            if (IO == null) continue;
            NewArray[Index] = IO; Index++;
        }
        for (int i = 0; i < AddObjects.Count; i++)
        { NewArray[Index] = AddObjects[i]; Index++; }

        Array destinationArray = Array.CreateInstance(ItemDatabase.ItemClassType, NewArray.Length);
        Array.Copy(NewArray, destinationArray, NewArray.Length);


        ItemList.SetValue(ItemDatabase.ObjectReference, destinationArray);
        Debug.Log("Safely Added New Item to Database.");
    }
    #endregion

    void AddSearchForLowestChild(Transform T, GenericDatabase ItemDatabase, List<object> AddObjects)
    {
        FieldInfo ItemListCount = ItemDatabase.ClassType.GetField("ListCount");
        FieldInfo ItemList = ItemDatabase.ClassType.GetField("ItemCatagoryList");

        if (T.childCount == 0)
        {
            //Item Data, and Variables that will be needed with this class type.
            GenericItem ItemObject = GetReflectionItem(DBType, T);
            FieldInfo LockItem = ItemObject.ClassType.GetField("LockItem");
            FieldInfo DatabaseID = ItemObject.ClassType.GetField("DatabaseID");
            FieldInfo Name = ItemObject.ClassType.GetField("Name");

            bool IsLocked = (bool)LockItem.GetValue(ItemObject.ObjectReference);

            if (!IsLocked)
            {
                int DatabaseListCount = (int)ItemListCount.GetValue(ItemDatabase.ObjectReference);
                DatabaseID.SetValue(ItemObject.ObjectReference, DatabaseListCount);
                DatabaseListCount++;

                ItemListCount.SetValue(ItemDatabase.ObjectReference, DatabaseListCount);
                LockItem.SetValue(ItemObject.ObjectReference, true);

                AddObjects.Add(ItemObject.ObjectReference);
            }

            int DatabaseIDValue = (int)DatabaseID.GetValue(ItemObject.ObjectReference);
            string NameValue = (string)Name.GetValue(ItemObject.ObjectReference);

            T.gameObject.name = DatabaseIDValue.ToString() + " - " + NameValue;
        }
        else
        {
            for (int iN = 0; iN < T.childCount; iN++)
                AddSearchForLowestChild(T.GetChild(iN), ItemDatabase, AddObjects);
        }
    }









    //Take Care to not use this post Game Release or every Users save files will turn to monkey garbage.
    //[Button(ButtonSizes.Small)]
    public void FlushEverything()
    {
        GenericDatabase ItemDatabase = GetReflectionData(DBType);
        FieldInfo DataListCount = ItemDatabase.ClassType.GetField("ListCount");
        FieldInfo DataCatagoryList = ItemDatabase.ClassType.GetField("ItemCatagoryList");

        DataListCount.SetValue(ItemDatabase.ObjectReference, 0);

        Array destinationArray = Array.CreateInstance(ItemDatabase.ItemClassType, 0);
        DataCatagoryList.SetValue(ItemDatabase.ObjectReference, destinationArray);

        Transform ItemDatabaseRoot = ItemDatabase.TransformReference;
        FlushSearchForLowestChild(ItemDatabaseRoot);

        GiveAllID(DBType);
    }
    void FlushSearchForLowestChild(Transform T)
    {
        if(T.childCount == 0)
        {
            GenericItem ItemObject = GetReflectionItem(DBType, T);
            FieldInfo LockItemVariable = ItemObject.ClassType.GetField("LockItem");

            LockItemVariable.SetValue(ItemObject.ObjectReference, false);
        }
        else
        {
            for (int iN = 0; iN < T.childCount; iN++)
                FlushSearchForLowestChild(T.GetChild(iN));
        }
    }


}

#endif
