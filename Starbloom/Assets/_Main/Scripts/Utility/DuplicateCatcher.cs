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
        FXCompendium
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















    //Delete this when you feel safe it isn't needed anymore.














    //void Awake()
    //{
    //    if (Application.isPlaying)
    //        return;
    //
    //    if (instanceID != GetInstanceID())
    //    {
    //        if (instanceID == 0) instanceID = GetInstanceID();
    //        else
    //        {
    //            instanceID = GetInstanceID();
    //            if (instanceID < 0)
    //            {
    //                Debug.Log("Duplicate Made, Generating New ID");
    //
    //                AssignNewID(DBType);
    //                return;
    //
    //                switch (DBType)
    //                {
    //                    case DatabaseType.Item: AssignNewDatabaseItemID(); break;
    //                    case DatabaseType.Character: AssignNewDatabaseCharacterID(); break;
    //                    case DatabaseType.DialogueTree: AssignNewDatabaseDialogueTreeID(); break;
    //                    case DatabaseType.Word: AssignNewDatabaseWordID(); break;
    //                    case DatabaseType.Quest: AssignNewDatabaseQuestID(); break;
    //                    case DatabaseType.FishingCompendium: AssignNewFishingID(); break;
    //                    case DatabaseType.ObjectCompendium: AssignNewObjectCompendiumID(); break;
    //                    case DatabaseType.ShopsCompendium: AssignNewShopCompendiumID(); break;
    //                    case DatabaseType.CraftingCompendium: AssignNewCraftingCompendiumID(); break;
    //                    case DatabaseType.PrefabPool: AssignNewPrefabPoolID(); break;
    //                    case DatabaseType.FXPool: AssignNewID(DBType); break;
    //                }
    //            }
    //        }
    //    }
    //}




    //[Button(ButtonSizes.Small)]
    //public void DoDataCheck()
    //{
    //    GiveAllID(DBType);
    //    return;
    //
    //    switch (DBType)
    //    {
    //        case DatabaseType.Item: GiveAllItemsDatabaseID(); break;
    //        case DatabaseType.Character: GiveAllCharacterDatabaseID(); break;
    //        case DatabaseType.DialogueTree: GiveAllDialogueTreeDatabaseID(); break;
    //        case DatabaseType.Word: GiveAllWordDatabaseID(); break;
    //        case DatabaseType.Quest: GiveAllQuestDatabaseID(); break;
    //        case DatabaseType.FishingCompendium: GiveAllFishingID(); break;
    //        case DatabaseType.ObjectCompendium: GiveAllObjectCompendiumID(); break;
    //        case DatabaseType.ShopsCompendium: GiveAllShopCompendiumID(); break;
    //        case DatabaseType.CraftingCompendium: GiveAllCraftingCompendiumID(); break;
    //        case DatabaseType.PrefabPool: GiveAllPrefabPoolID(); break;
    //        case DatabaseType.FXPool: GiveAllID(DBType); break;
    //    }
    //}




    //#region Items
    //void AssignNewDatabaseItemID()
    //{
    //    transform.GetComponent<DG_ItemObject>().LockItem = false;
    //    GiveAllItemsDatabaseID();
    //}
    //public void GiveAllItemsDatabaseID()
    //{
    //    DG_ItemsDatabase ItemDB = QuickFindInEditor.GetEditorItemDatabase();
    //    Transform ItemDatabaseRoot = ItemDB.transform;
    //
    //    List<DG_ItemObject> AddObjects = new List<DG_ItemObject>();
    //
    //    for (int iN = 0; iN < ItemDatabaseRoot.childCount; iN++)
    //    {
    //        Transform Child = ItemDatabaseRoot.GetChild(iN);
    //        for (int i = 0; i < Child.childCount; i++)
    //        {
    //            DG_ItemObject IO = Child.GetChild(i).GetComponent<DG_ItemObject>();
    //            if (!IO.LockItem)
    //            {
    //                IO.DatabaseID = ItemDB.ListCount;
    //                ItemDB.ListCount++;
    //                IO.LockItem = true;
    //                AddObjects.Add(IO);
    //            }
    //
    //            if (IO.ModelPrefab == null)
    //                Debug.Log("Model Prefab Missing WTF?");
    //
    //            if (IO.DatabaseUsesNameInsteadOfPrefab)
    //                IO.gameObject.name = IO.DatabaseID.ToString() + " - " + IO.Name;
    //            else
    //                IO.gameObject.name = IO.DatabaseID.ToString() + " - " + IO.ModelPrefab.name;
    //        }
    //    }
    //
    //    if (AddObjects.Count == 0) { Debug.Log("All Items Accounted For. :)"); return; }
    //
    //    int Index = 0;
    //    DG_ItemObject[] NewArray = new DG_ItemObject[ItemDB.ListCount];
    //    for (int i = 0; i < ItemDB.ItemCatagoryList.Length; i++)
    //    {
    //        DG_ItemObject IO = ItemDB.ItemCatagoryList[i];
    //        if (IO == null) continue;
    //        NewArray[Index] = IO; Index++;
    //    }
    //    for (int i = 0; i < AddObjects.Count; i++)
    //    { NewArray[Index] = AddObjects[i]; Index++; }
    //
    //    ItemDB.ItemCatagoryList = NewArray;
    //    Debug.Log("Safely Added New Item to Database.");
    //}
    //#endregion
    //
    //#region Character
    //void AssignNewDatabaseCharacterID()
    //{
    //    transform.GetComponent<DG_CharacterObject>().LockItem = false;
    //    GiveAllCharacterDatabaseID();
    //}
    //public void GiveAllCharacterDatabaseID()
    //{
    //    DG_CharacterDatabase ItemDB = QuickFindInEditor.GetEditorCharacterDatabase();
    //    Transform ItemDatabaseRoot = ItemDB.transform;
    //
    //    List<DG_CharacterObject> AddObjects = new List<DG_CharacterObject>();
    //
    //    for (int iN = 0; iN < ItemDatabaseRoot.childCount; iN++)
    //    {
    //        Transform Child = ItemDatabaseRoot.GetChild(iN);
    //        for (int i = 0; i < Child.childCount; i++)
    //        {
    //            DG_CharacterObject IO = Child.GetChild(i).GetComponent<DG_CharacterObject>();
    //            if (!IO.LockItem)
    //            {
    //                IO.DatabaseID = ItemDB.ListCount;
    //                ItemDB.ListCount++;
    //                IO.LockItem = true;
    //                AddObjects.Add(IO);
    //            }
    //
    //            IO.gameObject.name = IO.DatabaseID.ToString() + " - " + IO.ObjectName;
    //        }
    //    }
    //
    //    if (AddObjects.Count == 0) { Debug.Log("All Items Accounted For. :)"); return; }
    //
    //    int Index = 0;
    //    DG_CharacterObject[] NewArray = new DG_CharacterObject[ItemDB.ListCount];
    //    for (int i = 0; i < ItemDB.ItemCatagoryList.Length; i++)
    //    {
    //        DG_CharacterObject IO = ItemDB.ItemCatagoryList[i];
    //        if (IO == null) continue;
    //        NewArray[Index] = IO; Index++;
    //    }
    //    for (int i = 0; i < AddObjects.Count; i++)
    //    { NewArray[Index] = AddObjects[i]; Index++; }
    //
    //    ItemDB.ItemCatagoryList = NewArray;
    //    Debug.Log("Safely Added New Item to Database.");
    //}
    //#endregion
    //
    //#region DialogueTree
    //void AssignNewDatabaseDialogueTreeID()
    //{
    //    transform.GetComponent<NodeLink>().LockItem = false;
    //    GiveAllDialogueTreeDatabaseID();
    //}
    //public void GiveAllDialogueTreeDatabaseID()
    //{
    //    DG_DialogueManager ItemDB = QuickFindInEditor.GetEditorDialogueManager();
    //    Transform ItemDatabaseRoot = ItemDB.transform;
    //
    //    List<NodeLink> AddObjects = new List<NodeLink>();
    //
    //    for (int iN = 0; iN < ItemDatabaseRoot.childCount; iN++)
    //    {
    //        Transform Child = ItemDatabaseRoot.GetChild(iN);
    //        for (int i = 0; i < Child.childCount; i++)
    //        {
    //            NodeLink IO = Child.GetChild(i).GetComponent<NodeLink>();
    //            if (!IO.LockItem)
    //            {
    //                IO.DatabaseID = ItemDB.ListCount;
    //                ItemDB.ListCount++;
    //                IO.LockItem = true;
    //                AddObjects.Add(IO);
    //            }
    //
    //            IO.gameObject.name = IO.DatabaseID.ToString() + " - " + IO.TreeName;
    //        }
    //    }
    //
    //    if (AddObjects.Count == 0) { Debug.Log("All Items Accounted For. :)"); return; }
    //
    //    int Index = 0;
    //    NodeLink[] NewArray = new NodeLink[ItemDB.ListCount];
    //    for (int i = 0; i < ItemDB.ItemCatagoryList.Length; i++)
    //    {
    //        NodeLink IO = ItemDB.ItemCatagoryList[i];
    //        if (IO == null) continue;
    //        NewArray[Index] = IO; Index++;
    //    }
    //    for (int i = 0; i < AddObjects.Count; i++)
    //    { NewArray[Index] = AddObjects[i]; Index++; }
    //
    //    ItemDB.ItemCatagoryList = NewArray;
    //    Debug.Log("Safely Added New Item to Database.");
    //}
    //#endregion
    //
    //#region Words
    //void AssignNewDatabaseWordID()
    //{
    //    transform.GetComponent<DG_WordObject>().LockItem = false;
    //    GiveAllWordDatabaseID();
    //}
    //public void GiveAllWordDatabaseID()
    //{
    //    DG_WordDatabase ItemDB = QuickFindInEditor.GetEditorWordDatabase();
    //    Transform ItemDatabaseRoot = ItemDB.transform;
    //
    //    List<DG_WordObject> AddObjects = new List<DG_WordObject>();
    //
    //    for (int iN = 0; iN < ItemDatabaseRoot.childCount; iN++)
    //    {
    //        Transform Child = ItemDatabaseRoot.GetChild(iN);
    //        for (int i = 0; i < Child.childCount; i++)
    //        {
    //            DG_WordObject IO = Child.GetChild(i).GetComponent<DG_WordObject>();
    //            if (!IO.LockItem)
    //            {
    //                IO.DatabaseID = ItemDB.ListCount;
    //                ItemDB.ListCount++;
    //                IO.LockItem = true;
    //                AddObjects.Add(IO);
    //            }
    //
    //            IO.gameObject.name = IO.DatabaseID.ToString() + " - " + GetFirst10Letters(IO.TextValues[0].stringEntry);
    //        }
    //    }
    //
    //    if (AddObjects.Count == 0) { Debug.Log("All Items Accounted For. :)"); return; }
    //
    //    int Index = 0;
    //    DG_WordObject[] NewArray = new DG_WordObject[ItemDB.ListCount];
    //    for (int i = 0; i < ItemDB.ItemCatagoryList.Length; i++)
    //    {
    //        DG_WordObject IO = ItemDB.ItemCatagoryList[i];
    //        if (IO == null) continue;
    //        NewArray[Index] = IO; Index++;
    //    }
    //    for (int i = 0; i < AddObjects.Count; i++)
    //    { NewArray[Index] = AddObjects[i]; Index++; }
    //
    //    ItemDB.ItemCatagoryList = NewArray;
    //    Debug.Log("Safely Added New Item to Database.");
    //}
    //string GetFirst10Letters(string Base)
    //{
    //    char[] c = Base.ToCharArray();
    //    System.Text.StringBuilder SB = new System.Text.StringBuilder();
    //    int count = 10;
    //    if (c.Length < 10) count = c.Length;
    //    for (int i = 0; i < count; i++)
    //        SB.Append(c[i]);
    //    return SB.ToString();
    //}
    //#endregion
    //
    //#region Quest
    //void AssignNewDatabaseQuestID()
    //{
    //    transform.GetComponent<DG_QuestObject>().LockItem = false;
    //    GiveAllQuestDatabaseID();
    //}
    //public void GiveAllQuestDatabaseID()
    //{
    //    DG_QuestDatabase ItemDB = QuickFindInEditor.GetEditorQuestDatabase();
    //    Transform ItemDatabaseRoot = ItemDB.transform;
    //
    //    List<DG_QuestObject> AddObjects = new List<DG_QuestObject>();
    //
    //    for (int iN = 0; iN < ItemDatabaseRoot.childCount; iN++)
    //    {
    //        Transform Child = ItemDatabaseRoot.GetChild(iN);
    //        for (int i = 0; i < Child.childCount; i++)
    //        {
    //            DG_QuestObject IO = Child.GetChild(i).GetComponent<DG_QuestObject>();
    //            if (!IO.LockItem)
    //            {
    //                IO.DatabaseID = ItemDB.ListCount;
    //                ItemDB.ListCount++;
    //                IO.LockItem = true;
    //                AddObjects.Add(IO);
    //            }
    //
    //            IO.gameObject.name = IO.DatabaseID.ToString() + " - " + IO.ObjectName;
    //        }
    //    }
    //
    //    if (AddObjects.Count == 0) { Debug.Log("All Items Accounted For. :)"); return; }
    //
    //    int Index = 0;
    //    DG_QuestObject[] NewArray = new DG_QuestObject[ItemDB.ListCount];
    //    for (int i = 0; i < ItemDB.ItemCatagoryList.Length; i++)
    //    {
    //        DG_QuestObject IO = ItemDB.ItemCatagoryList[i];
    //        if (IO == null) continue;
    //        NewArray[Index] = IO; Index++;
    //    }
    //    for (int i = 0; i < AddObjects.Count; i++)
    //    { NewArray[Index] = AddObjects[i]; Index++; }
    //
    //    ItemDB.ItemCatagoryList = NewArray;
    //    Debug.Log("Safely Added New Item to Database.");
    //}
    //#endregion
    //
    //#region FishingCompendium
    //void AssignNewFishingID()
    //{
    //    transform.GetComponent<DG_FishingAtlasObject>().LockItem = false;
    //    GiveAllFishingID();
    //}
    //public void GiveAllFishingID()
    //{
    //    DG_FishingCompendium ItemDB = QuickFindInEditor.GetEditorFishingCompendium();
    //    Transform ItemDatabaseRoot = ItemDB.transform;
    //
    //    List<DG_FishingAtlasObject> AddObjects = new List<DG_FishingAtlasObject>();
    //
    //    for (int iN = 0; iN < ItemDatabaseRoot.childCount; iN++)
    //    {
    //        Transform Child = ItemDatabaseRoot.GetChild(iN);
    //        for (int i = 0; i < Child.childCount; i++)
    //        {
    //            DG_FishingAtlasObject IO = Child.GetChild(i).GetComponent<DG_FishingAtlasObject>();
    //            if (!IO.LockItem)
    //            {
    //                IO.DatabaseID = ItemDB.ListCount;
    //                ItemDB.ListCount++;
    //                IO.LockItem = true;
    //                AddObjects.Add(IO);
    //            }
    //
    //            IO.gameObject.name = IO.DatabaseID.ToString() + " - " + IO.Name;
    //        }
    //    }
    //
    //    if (AddObjects.Count == 0) { Debug.Log("All Items Accounted For. :)"); return; }
    //
    //    int Index = 0;
    //    DG_FishingAtlasObject[] NewArray = new DG_FishingAtlasObject[ItemDB.ListCount];
    //    for (int i = 0; i < ItemDB.ItemCatagoryList.Length; i++)
    //    {
    //        DG_FishingAtlasObject IO = ItemDB.ItemCatagoryList[i];
    //        if (IO == null) continue;
    //        NewArray[Index] = IO; Index++;
    //    }
    //    for (int i = 0; i < AddObjects.Count; i++)
    //    { NewArray[Index] = AddObjects[i]; Index++; }
    //
    //    ItemDB.ItemCatagoryList = NewArray;
    //    Debug.Log("Safely Added New Item to Database.");
    //}
    //#endregion
    //
    //#region ObjectCompendium
    //void AssignNewObjectCompendiumID()
    //{
    //    transform.GetComponent<DG_BreakableObjectItem>().LockItem = false;
    //    GiveAllObjectCompendiumID();
    //}
    //public void GiveAllObjectCompendiumID()
    //{
    //    DG_BreakableObjectsAtlas ItemDB = QuickFindInEditor.GetBreakableObjectsCompendium();
    //    Transform ItemDatabaseRoot = ItemDB.transform;
    //
    //    List<DG_BreakableObjectItem> AddObjects = new List<DG_BreakableObjectItem>();
    //
    //    for (int iN = 0; iN < ItemDatabaseRoot.childCount; iN++)
    //    {
    //        Transform Child = ItemDatabaseRoot.GetChild(iN);
    //        for (int i = 0; i < Child.childCount; i++)
    //        {
    //            DG_BreakableObjectItem IO = Child.GetChild(i).GetComponent<DG_BreakableObjectItem>();
    //            if (!IO.LockItem)
    //            {
    //                IO.DatabaseID = ItemDB.ListCount;
    //                ItemDB.ListCount++;
    //                IO.LockItem = true;
    //                AddObjects.Add(IO);
    //            }
    //
    //            IO.gameObject.name = IO.DatabaseID.ToString() + " - " + IO.Name;
    //        }
    //    }
    //
    //    if (AddObjects.Count == 0) { Debug.Log("All Items Accounted For. :)"); return; }
    //
    //    int Index = 0;
    //    DG_BreakableObjectItem[] NewArray = new DG_BreakableObjectItem[ItemDB.ListCount];
    //    for (int i = 0; i < ItemDB.ItemCatagoryList.Length; i++)
    //    {
    //        DG_BreakableObjectItem IO = ItemDB.ItemCatagoryList[i];
    //        if (IO == null) continue;
    //        NewArray[Index] = IO; Index++;
    //    }
    //    for (int i = 0; i < AddObjects.Count; i++)
    //    { NewArray[Index] = AddObjects[i]; Index++; }
    //
    //    ItemDB.ItemCatagoryList = NewArray;
    //    Debug.Log("Safely Added New Item to Database.");
    //}
    //#endregion
    //
    //#region Shops Compendium
    //void AssignNewShopCompendiumID()
    //{
    //    transform.GetComponent<DG_ShopAtlasObject>().LockItem = false;
    //    GiveAllShopCompendiumID();
    //}
    //public void GiveAllShopCompendiumID()
    //{
    //    DG_ShopAtlas ItemDB = QuickFindInEditor.GetShopCompendium();
    //    Transform ItemDatabaseRoot = ItemDB.transform;
    //
    //    List<DG_ShopAtlasObject> AddObjects = new List<DG_ShopAtlasObject>();
    //
    //    for (int iN = 0; iN < ItemDatabaseRoot.childCount; iN++)
    //    {
    //        Transform Child = ItemDatabaseRoot.GetChild(iN);
    //        for (int i = 0; i < Child.childCount; i++)
    //        {
    //            DG_ShopAtlasObject IO = Child.GetChild(i).GetComponent<DG_ShopAtlasObject>();
    //            if (!IO.LockItem)
    //            {
    //                IO.DatabaseID = ItemDB.ListCount;
    //                ItemDB.ListCount++;
    //                IO.LockItem = true;
    //                AddObjects.Add(IO);
    //            }
    //
    //            IO.gameObject.name = IO.DatabaseID.ToString() + " - " + IO.Name;
    //        }
    //    }
    //
    //    if (AddObjects.Count == 0) { Debug.Log("All Items Accounted For. :)"); return; }
    //
    //    int Index = 0;
    //    DG_ShopAtlasObject[] NewArray = new DG_ShopAtlasObject[ItemDB.ListCount];
    //    for (int i = 0; i < ItemDB.ItemCatagoryList.Length; i++)
    //    {
    //        DG_ShopAtlasObject IO = ItemDB.ItemCatagoryList[i];
    //        if (IO == null) continue;
    //        NewArray[Index] = IO; Index++;
    //    }
    //    for (int i = 0; i < AddObjects.Count; i++)
    //    { NewArray[Index] = AddObjects[i]; Index++; }
    //
    //    ItemDB.ItemCatagoryList = NewArray;
    //    Debug.Log("Safely Added New Item to Database.");
    //}
    //#endregion
    //
    //#region Crafting Compendium
    //void AssignNewCraftingCompendiumID()
    //{
    //    transform.GetComponent<DG_CraftingDictionaryItem>().LockItem = false;
    //    GiveAllCraftingCompendiumID();
    //}
    //public void GiveAllCraftingCompendiumID()
    //{
    //    DG_CraftingDictionary ItemDB = QuickFindInEditor.GetCraftingCompendium();
    //    Transform ItemDatabaseRoot = ItemDB.transform;
    //
    //    List<DG_CraftingDictionaryItem> AddObjects = new List<DG_CraftingDictionaryItem>();
    //
    //    for (int iN = 0; iN < ItemDatabaseRoot.childCount; iN++)
    //    {
    //        Transform Child = ItemDatabaseRoot.GetChild(iN);
    //        for (int i = 0; i < Child.childCount; i++)
    //        {
    //            DG_CraftingDictionaryItem IO = Child.GetChild(i).GetComponent<DG_CraftingDictionaryItem>();
    //            if (!IO.LockItem)
    //            {
    //                IO.DatabaseID = ItemDB.ListCount;
    //                ItemDB.ListCount++;
    //                IO.LockItem = true;
    //                AddObjects.Add(IO);
    //            }
    //
    //            IO.gameObject.name = IO.DatabaseID.ToString() + " - " + IO.Name;
    //        }
    //    }
    //
    //    if (AddObjects.Count == 0) { Debug.Log("All Items Accounted For. :)"); return; }
    //
    //    int Index = 0;
    //    DG_CraftingDictionaryItem[] NewArray = new DG_CraftingDictionaryItem[ItemDB.ListCount];
    //    for (int i = 0; i < ItemDB.ItemCatagoryList.Length; i++)
    //    {
    //        DG_CraftingDictionaryItem IO = ItemDB.ItemCatagoryList[i];
    //        if (IO == null) continue;
    //        NewArray[Index] = IO; Index++;
    //    }
    //    for (int i = 0; i < AddObjects.Count; i++)
    //    { NewArray[Index] = AddObjects[i]; Index++; }
    //
    //    ItemDB.ItemCatagoryList = NewArray;
    //    Debug.Log("Safely Added New Item to Database.");
    //}
    //#endregion
    //
    //#region Prefab Pool Dictionary
    //void AssignNewPrefabPoolID()
    //{
    //    transform.GetComponent<DG_PrefabPoolItem>().LockItem = false;
    //    GiveAllPrefabPoolID();
    //}
    //public void GiveAllPrefabPoolID()
    //{
    //    DG_PrefabPoolDictionary ItemDB = QuickFindInEditor.GetPrefabPool();
    //    Transform ItemDatabaseRoot = ItemDB.transform;
    //
    //    List<DG_PrefabPoolItem> AddObjects = new List<DG_PrefabPoolItem>();
    //
    //    for (int iN = 0; iN < ItemDatabaseRoot.childCount; iN++)
    //    {
    //        Transform Child = ItemDatabaseRoot.GetChild(iN);
    //        for (int i = 0; i < Child.childCount; i++)
    //        {
    //            DG_PrefabPoolItem IO = Child.GetChild(i).GetComponent<DG_PrefabPoolItem>();
    //            if (!IO.LockItem)
    //            {
    //                IO.DatabaseID = ItemDB.ListCount;
    //                ItemDB.ListCount++;
    //                IO.LockItem = true;
    //                AddObjects.Add(IO);
    //            }
    //
    //            IO.gameObject.name = IO.DatabaseID.ToString() + " - " + IO.Name;
    //        }
    //    }
    //
    //    if (AddObjects.Count == 0) { Debug.Log("All Items Accounted For. :)"); return; }
    //
    //    int Index = 0;
    //    DG_PrefabPoolItem[] NewArray = new DG_PrefabPoolItem[ItemDB.ListCount];
    //    for (int i = 0; i < ItemDB.ItemCatagoryList.Length; i++)
    //    {
    //        DG_PrefabPoolItem IO = ItemDB.ItemCatagoryList[i];
    //        if (IO == null) continue;
    //        NewArray[Index] = IO; Index++;
    //    }
    //    for (int i = 0; i < AddObjects.Count; i++)
    //    { NewArray[Index] = AddObjects[i]; Index++; }
    //
    //    ItemDB.ItemCatagoryList = NewArray;
    //    Debug.Log("Safely Added New Item to Database.");
    //}
    //#endregion
    //
    //#region FX Pool Dictionary
    //void AssignNewFXPoolID()
    //{
    //    transform.GetComponent<DG_FXPoolItem>().LockItem = false;
    //    GiveAllFXPoolID();
    //}
    //public void GiveAllFXPoolID()
    //{
    //    DG_FXPoolDictionary ItemDB = QuickFindInEditor.GetFXPool();
    //    Transform ItemDatabaseRoot = ItemDB.transform;
    //
    //    List<DG_FXPoolItem> AddObjects = new List<DG_FXPoolItem>();
    //
    //    for (int iN = 0; iN < ItemDatabaseRoot.childCount; iN++)
    //    {
    //        Transform Child = ItemDatabaseRoot.GetChild(iN);
    //        for (int i = 0; i < Child.childCount; i++)
    //        {
    //            DG_FXPoolItem IO = Child.GetChild(i).GetComponent<DG_FXPoolItem>();
    //            if (!IO.LockItem)
    //            {
    //                IO.DatabaseID = ItemDB.ListCount;
    //                ItemDB.ListCount++;
    //                IO.LockItem = true;
    //                AddObjects.Add(IO);
    //            }
    //
    //            IO.gameObject.name = IO.DatabaseID.ToString() + " - " + IO.Name;
    //        }
    //    }
    //
    //    if (AddObjects.Count == 0) { Debug.Log("All Items Accounted For. :)"); return; }
    //
    //    int Index = 0;
    //    DG_FXPoolItem[] NewArray = new DG_FXPoolItem[ItemDB.ListCount];
    //    for (int i = 0; i < ItemDB.ItemCatagoryList.Length; i++)
    //    {
    //        DG_FXPoolItem IO = ItemDB.ItemCatagoryList[i];
    //        if (IO == null) continue;
    //        NewArray[Index] = IO; Index++;
    //    }
    //    for (int i = 0; i < AddObjects.Count; i++)
    //    { NewArray[Index] = AddObjects[i]; Index++; }
    //
    //    ItemDB.ItemCatagoryList = NewArray;
    //    Debug.Log("Safely Added New Item to Database.");
    //}
    //#endregion





    //Take Care to not use this post Game Release or every Users save files will turn to monkey garbage.
    //[Button(ButtonSizes.Small)]
    //public void FlushEverything()
    //{
    //    switch (DBType)
    //    {
    //        case DatabaseType.Item:
    //            {
    //                DG_ItemsDatabase ItemDB = QuickFindInEditor.GetEditorItemDatabase();
    //                ItemDB.ListCount = 0;
    //                ItemDB.ItemCatagoryList = new DG_ItemObject[0];
    //                for (int i = 0; i < ItemDB.transform.childCount; i++)
    //                {
    //                    for(int iN = 0; iN < ItemDB.transform.GetChild(i).childCount; iN++)
    //                        ItemDB.transform.GetChild(i).GetChild(iN).GetComponent<DG_ItemObject>().LockItem = false;
    //                }
    //                GiveAllItemsDatabaseID(); break;
    //            }
    //        case DatabaseType.Character:
    //            {
    //                DG_CharacterDatabase ItemDB = QuickFindInEditor.GetEditorCharacterDatabase();
    //                ItemDB.ListCount = 0;
    //                ItemDB.ItemCatagoryList = new DG_CharacterObject[0];
    //                for (int i = 0; i < ItemDB.transform.childCount; i++)
    //                {
    //                    for (int iN = 0; iN < ItemDB.transform.GetChild(i).childCount; iN++)
    //                        ItemDB.transform.GetChild(i).GetChild(iN).GetComponent<DG_CharacterObject>().LockItem = false;
    //                }
    //                GiveAllCharacterDatabaseID(); break;
    //            }
    //        case DatabaseType.DialogueTree:
    //            {
    //                DG_DialogueManager ItemDB = QuickFindInEditor.GetEditorDialogueManager();
    //                ItemDB.ListCount = 0;
    //                ItemDB.ItemCatagoryList = new NodeLink[0];
    //                for (int i = 0; i < ItemDB.transform.childCount; i++)
    //                {
    //                    for (int iN = 0; iN < ItemDB.transform.GetChild(i).childCount; iN++)
    //                        ItemDB.transform.GetChild(i).GetChild(iN).GetComponent<NodeLink>().LockItem = false;
    //                }
    //                GiveAllDialogueTreeDatabaseID(); break;
    //            }
    //        case DatabaseType.Word:
    //            {
    //                DG_WordDatabase ItemDB = QuickFindInEditor.GetEditorWordDatabase();
    //                ItemDB.ListCount = 0;
    //                ItemDB.ItemCatagoryList = new DG_WordObject[0];
    //                for (int i = 0; i < ItemDB.transform.childCount; i++)
    //                {
    //                    for (int iN = 0; iN < ItemDB.transform.GetChild(i).childCount; iN++)
    //                        ItemDB.transform.GetChild(i).GetChild(iN).GetComponent<DG_WordObject>().LockItem = false;
    //                }
    //                GiveAllWordDatabaseID(); break;
    //            }
    //        case DatabaseType.Quest:
    //            {
    //                DG_QuestDatabase ItemDB = QuickFindInEditor.GetEditorQuestDatabase();
    //                ItemDB.ListCount = 0;
    //                ItemDB.ItemCatagoryList = new DG_QuestObject[0];
    //                for (int i = 0; i < ItemDB.transform.childCount; i++)
    //                {
    //                    for (int iN = 0; iN < ItemDB.transform.GetChild(i).childCount; iN++)
    //                        ItemDB.transform.GetChild(i).GetChild(iN).GetComponent<DG_QuestObject>().LockItem = false;
    //                }
    //                GiveAllQuestDatabaseID(); break;
    //            }
    //        case DatabaseType.FishingCompendium:
    //            {
    //                DG_FishingCompendium ItemDB = QuickFindInEditor.GetEditorFishingCompendium();
    //                ItemDB.ListCount = 0;
    //                ItemDB.ItemCatagoryList = new DG_FishingAtlasObject[0];
    //                for (int i = 0; i < ItemDB.transform.childCount; i++)
    //                {
    //                    for (int iN = 0; iN < ItemDB.transform.GetChild(i).childCount; iN++)
    //                        ItemDB.transform.GetChild(i).GetChild(iN).GetComponent<DG_FishingAtlasObject>().LockItem = false;
    //                }
    //                GiveAllFishingID(); break;
    //            }
    //        case DatabaseType.ObjectCompendium:
    //            {
    //                DG_BreakableObjectsAtlas ItemDB = QuickFindInEditor.GetBreakableObjectsCompendium();
    //                ItemDB.ListCount = 0;
    //                ItemDB.ItemCatagoryList = new DG_BreakableObjectItem[0];
    //                for (int i = 0; i < ItemDB.transform.childCount; i++)
    //                {
    //                    for (int iN = 0; iN < ItemDB.transform.GetChild(i).childCount; iN++)
    //                        ItemDB.transform.GetChild(i).GetChild(iN).GetComponent<DG_BreakableObjectItem>().LockItem = false;
    //                }
    //                GiveAllObjectCompendiumID(); break;
    //            }
    //        case DatabaseType.ShopsCompendium:
    //            {
    //                DG_ShopAtlas ItemDB = QuickFindInEditor.GetShopCompendium();
    //                ItemDB.ListCount = 0;
    //                ItemDB.ItemCatagoryList = new DG_ShopAtlasObject[0];
    //                for (int i = 0; i < ItemDB.transform.childCount; i++)
    //                {
    //                    for (int iN = 0; iN < ItemDB.transform.GetChild(i).childCount; iN++)
    //                        ItemDB.transform.GetChild(i).GetChild(iN).GetComponent<DG_ShopAtlasObject>().LockItem = false;
    //                }
    //                GiveAllShopCompendiumID(); break;
    //            }
    //        case DatabaseType.CraftingCompendium:
    //            {
    //                DG_CraftingDictionary ItemDB = QuickFindInEditor.GetCraftingCompendium();
    //                ItemDB.ListCount = 0;
    //                ItemDB.ItemCatagoryList = new DG_CraftingDictionaryItem[0];
    //                for (int i = 0; i < ItemDB.transform.childCount; i++)
    //                {
    //                    for (int iN = 0; iN < ItemDB.transform.GetChild(i).childCount; iN++)
    //                        ItemDB.transform.GetChild(i).GetChild(iN).GetComponent<DG_CraftingDictionaryItem>().LockItem = false;
    //                }
    //                GiveAllCraftingCompendiumID(); break;
    //            }
    //        case DatabaseType.PrefabPool:
    //            {
    //                DG_PrefabPoolDictionary ItemDB = QuickFindInEditor.GetPrefabPool();
    //                ItemDB.ListCount = 0;
    //                ItemDB.ItemCatagoryList = new DG_PrefabPoolItem[0];
    //                for (int i = 0; i < ItemDB.transform.childCount; i++)
    //                {
    //                    for (int iN = 0; iN < ItemDB.transform.GetChild(i).childCount; iN++)
    //                        ItemDB.transform.GetChild(i).GetChild(iN).GetComponent<DG_PrefabPoolItem>().LockItem = false;
    //                }
    //                GiveAllPrefabPoolID(); break;
    //            }
    //        case DatabaseType.FXPool:
    //            {
    //                DG_FXPoolDictionary ItemDB = QuickFindInEditor.GetFXPool();
    //                ItemDB.ListCount = 0;
    //                ItemDB.ItemCatagoryList = new DG_FXPoolItem[0];
    //                for (int i = 0; i < ItemDB.transform.childCount; i++)
    //                {
    //                    for (int iN = 0; iN < ItemDB.transform.GetChild(i).childCount; iN++)
    //                        ItemDB.transform.GetChild(i).GetChild(iN).GetComponent<DG_FXPoolItem>().LockItem = false;
    //                }
    //                GiveAllFXPoolID(); break;
    //            }
    //    }
    //}
}

#endif
