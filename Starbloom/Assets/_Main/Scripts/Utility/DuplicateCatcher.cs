
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        CraftingCompendium
    }





    public DatabaseType DBType;
    [HideInInspector] public int instanceID = 0;

    void Awake()
    {
        if (Application.isPlaying)
            return;

        if (instanceID != GetInstanceID())
        {
            if (instanceID == 0) instanceID = GetInstanceID();
            else
            {
                instanceID = GetInstanceID();
                if (instanceID < 0)
                {
                    Debug.Log("Duplicate Made, Generating New ID");
                    switch(DBType)
                    {
                        case DatabaseType.Item: AssignNewDatabaseItemID(); break;
                        case DatabaseType.Character: AssignNewDatabaseCharacterID(); break;
                        case DatabaseType.DialogueTree: AssignNewDatabaseDialogueTreeID(); break;
                        case DatabaseType.Word: AssignNewDatabaseWordID(); break;
                        case DatabaseType.Quest: AssignNewDatabaseQuestID(); break;
                        case DatabaseType.FishingCompendium: AssignNewFishingID(); break;
                        case DatabaseType.ObjectCompendium: AssignNewObjectCompendiumID(); break;
                        case DatabaseType.ShopsCompendium: AssignNewShopCompendiumID(); break;
                        case DatabaseType.CraftingCompendium: AssignNewCraftingCompendiumID(); break;
                    }
                }
            }
        }
    }





    [Button(ButtonSizes.Small)]
    public void DoDataCheck()
    {
        switch (DBType)
        {
            case DatabaseType.Item: GiveAllItemsDatabaseID(); break;
            case DatabaseType.Character: GiveAllCharacterDatabaseID(); break;
            case DatabaseType.DialogueTree: GiveAllDialogueTreeDatabaseID(); break;
            case DatabaseType.Word: GiveAllWordDatabaseID(); break;
            case DatabaseType.Quest: GiveAllQuestDatabaseID(); break;
            case DatabaseType.FishingCompendium: GiveAllFishingID(); break;
            case DatabaseType.ObjectCompendium: GiveAllObjectCompendiumID(); break;
            case DatabaseType.ShopsCompendium: GiveAllShopCompendiumID(); break;
            case DatabaseType.CraftingCompendium: GiveAllCraftingCompendiumID(); break;
        }
    }




    #region Items
    void AssignNewDatabaseItemID()
    {
        transform.GetComponent<DG_ItemObject>().LockItem = false;
        GiveAllItemsDatabaseID();
    }
    public void GiveAllItemsDatabaseID()
    {
        DG_ItemsDatabase ItemDB = QuickFindInEditor.GetEditorItemDatabase();
        Transform ItemDatabaseRoot = ItemDB.transform;

        List<DG_ItemObject> AddObjects = new List<DG_ItemObject>();

        for (int iN = 0; iN < ItemDatabaseRoot.childCount; iN++)
        {
            Transform Child = ItemDatabaseRoot.GetChild(iN);
            for (int i = 0; i < Child.childCount; i++)
            {
                DG_ItemObject IO = Child.GetChild(i).GetComponent<DG_ItemObject>();
                if (!IO.LockItem)
                {
                    IO.DatabaseID = ItemDB.ListCount;
                    ItemDB.ListCount++;
                    IO.LockItem = true;
                    AddObjects.Add(IO);
                }

                if (IO.ModelPrefab == null)
                    Debug.Log("Model Prefab Missing WTF?");

                if(IO.DatabaseUsesNameInsteadOfPrefab)
                    IO.gameObject.name = IO.DatabaseID.ToString() + " - " + IO.Name;
                else
                    IO.gameObject.name = IO.DatabaseID.ToString() + " - " + IO.ModelPrefab.name;
            }
        }

        if (AddObjects.Count == 0) { Debug.Log("All Items Accounted For. :)"); return; }

        int Index = 0;
        DG_ItemObject[] NewArray = new DG_ItemObject[ItemDB.ListCount];
        for (int i = 0; i < ItemDB.ItemCatagoryList.Length; i++)
        {
            DG_ItemObject IO = ItemDB.ItemCatagoryList[i];
            if (IO == null) continue;
            NewArray[Index] = IO; Index++;
        }
        for (int i = 0; i < AddObjects.Count; i++)
            { NewArray[Index] = AddObjects[i]; Index++; }

        ItemDB.ItemCatagoryList = NewArray;
        Debug.Log("Safely Added New Item to Database.");
    }
    #endregion

    #region Character
    void AssignNewDatabaseCharacterID()
    {
        transform.GetComponent<DG_CharacterObject>().LockItem = false;
        GiveAllCharacterDatabaseID();
    }
    public void GiveAllCharacterDatabaseID()
    {
        DG_CharacterDatabase ItemDB = QuickFindInEditor.GetEditorCharacterDatabase();
        Transform ItemDatabaseRoot = ItemDB.transform;

        List<DG_CharacterObject> AddObjects = new List<DG_CharacterObject>();

        for (int iN = 0; iN < ItemDatabaseRoot.childCount; iN++)
        {
            Transform Child = ItemDatabaseRoot.GetChild(iN);
            for (int i = 0; i < Child.childCount; i++)
            {
                DG_CharacterObject IO = Child.GetChild(i).GetComponent<DG_CharacterObject>();
                if (!IO.LockItem)
                {
                    IO.DatabaseID = ItemDB.ListCount;
                    ItemDB.ListCount++;
                    IO.LockItem = true;
                    AddObjects.Add(IO);
                }

                IO.gameObject.name = IO.DatabaseID.ToString() + " - " + IO.ObjectName;
            }
        }

        if (AddObjects.Count == 0) { Debug.Log("All Items Accounted For. :)"); return; }

        int Index = 0;
        DG_CharacterObject[] NewArray = new DG_CharacterObject[ItemDB.ListCount];
        for (int i = 0; i < ItemDB.ItemCatagoryList.Length; i++)
        {
            DG_CharacterObject IO = ItemDB.ItemCatagoryList[i];
            if (IO == null) continue;
            NewArray[Index] = IO; Index++;
        }
        for (int i = 0; i < AddObjects.Count; i++)
        { NewArray[Index] = AddObjects[i]; Index++; }

        ItemDB.ItemCatagoryList = NewArray;
        Debug.Log("Safely Added New Item to Database.");
    }
    #endregion

    #region DialogueTree
    void AssignNewDatabaseDialogueTreeID()
    {
        transform.GetComponent<NodeLink>().LockItem = false;
        GiveAllDialogueTreeDatabaseID();
    }
    public void GiveAllDialogueTreeDatabaseID()
    {
        DG_DialogueManager ItemDB = QuickFindInEditor.GetEditorDialogueManager();
        Transform ItemDatabaseRoot = ItemDB.transform;

        List<NodeLink> AddObjects = new List<NodeLink>();

        for (int iN = 0; iN < ItemDatabaseRoot.childCount; iN++)
        {
            Transform Child = ItemDatabaseRoot.GetChild(iN);
            for (int i = 0; i < Child.childCount; i++)
            {
                NodeLink IO = Child.GetChild(i).GetComponent<NodeLink>();
                if (!IO.LockItem)
                {
                    IO.DatabaseID = ItemDB.ListCount;
                    ItemDB.ListCount++;            
                    IO.LockItem = true;
                    AddObjects.Add(IO);
                }

                IO.gameObject.name = IO.DatabaseID.ToString() + " - " + IO.TreeName;
            }
        }

        if (AddObjects.Count == 0) { Debug.Log("All Items Accounted For. :)"); return; }

        int Index = 0;
        NodeLink[] NewArray = new NodeLink[ItemDB.ListCount];
        for (int i = 0; i < ItemDB.ItemCatagoryList.Length; i++)
        {
            NodeLink IO = ItemDB.ItemCatagoryList[i];
            if (IO == null) continue;
            NewArray[Index] = IO; Index++;
        }
        for (int i = 0; i < AddObjects.Count; i++)
        { NewArray[Index] = AddObjects[i]; Index++; }

        ItemDB.ItemCatagoryList = NewArray;
        Debug.Log("Safely Added New Item to Database.");
    }
    #endregion

    #region Words
    void AssignNewDatabaseWordID()
    {
        transform.GetComponent<DG_WordObject>().LockItem = false;
        GiveAllWordDatabaseID();
    }
    public void GiveAllWordDatabaseID()
    {
        DG_WordDatabase ItemDB = QuickFindInEditor.GetEditorWordDatabase();
        Transform ItemDatabaseRoot = ItemDB.transform;

        List<DG_WordObject> AddObjects = new List<DG_WordObject>();

        for (int iN = 0; iN < ItemDatabaseRoot.childCount; iN++)
        {
            Transform Child = ItemDatabaseRoot.GetChild(iN);
            for (int i = 0; i < Child.childCount; i++)
            {
                DG_WordObject IO = Child.GetChild(i).GetComponent<DG_WordObject>();
                if (!IO.LockItem)
                {
                    IO.DatabaseID = ItemDB.ListCount;
                    ItemDB.ListCount++;
                    IO.LockItem = true;
                    AddObjects.Add(IO);
                }

                IO.gameObject.name = IO.DatabaseID.ToString() + " - " + GetFirst10Letters(IO.TextValues[0].stringEntry);
            }
        }

        if (AddObjects.Count == 0) { Debug.Log("All Items Accounted For. :)"); return; }

        int Index = 0;
        DG_WordObject[] NewArray = new DG_WordObject[ItemDB.ListCount];
        for (int i = 0; i < ItemDB.ItemCatagoryList.Length; i++)
        {
            DG_WordObject IO = ItemDB.ItemCatagoryList[i];
            if (IO == null) continue;
            NewArray[Index] = IO; Index++;
        }
        for (int i = 0; i < AddObjects.Count; i++)
        { NewArray[Index] = AddObjects[i]; Index++; }

        ItemDB.ItemCatagoryList = NewArray;
        Debug.Log("Safely Added New Item to Database.");
    }
    string GetFirst10Letters(string Base)
    {
        char[] c = Base.ToCharArray();
        System.Text.StringBuilder SB = new System.Text.StringBuilder();
        int count = 10;
        if (c.Length < 10) count = c.Length;
        for (int i = 0; i < count; i++)
            SB.Append(c[i]);
        return SB.ToString();
    }
    #endregion

    #region Quest
    void AssignNewDatabaseQuestID()
    {
        transform.GetComponent<DG_QuestObject>().LockItem = false;
        GiveAllQuestDatabaseID();
    }
    public void GiveAllQuestDatabaseID()
    {
        DG_QuestDatabase ItemDB = QuickFindInEditor.GetEditorQuestDatabase();
        Transform ItemDatabaseRoot = ItemDB.transform;

        List<DG_QuestObject> AddObjects = new List<DG_QuestObject>();

        for (int iN = 0; iN < ItemDatabaseRoot.childCount; iN++)
        {
            Transform Child = ItemDatabaseRoot.GetChild(iN);
            for (int i = 0; i < Child.childCount; i++)
            {
                DG_QuestObject IO = Child.GetChild(i).GetComponent<DG_QuestObject>();
                if (!IO.LockItem)
                {
                    IO.DatabaseID = ItemDB.ListCount;
                    ItemDB.ListCount++;
                    IO.LockItem = true;
                    AddObjects.Add(IO);
                }

                IO.gameObject.name = IO.DatabaseID.ToString() + " - " + IO.ObjectName;
            }
        }

        if (AddObjects.Count == 0) { Debug.Log("All Items Accounted For. :)"); return; }

        int Index = 0;
        DG_QuestObject[] NewArray = new DG_QuestObject[ItemDB.ListCount];
        for (int i = 0; i < ItemDB.ItemCatagoryList.Length; i++)
        {
            DG_QuestObject IO = ItemDB.ItemCatagoryList[i];
            if (IO == null) continue;
            NewArray[Index] = IO; Index++;
        }
        for (int i = 0; i < AddObjects.Count; i++)
        { NewArray[Index] = AddObjects[i]; Index++; }

        ItemDB.ItemCatagoryList = NewArray;
        Debug.Log("Safely Added New Item to Database.");
    }
    #endregion

    #region FishingCompendium
    void AssignNewFishingID()
    {
        transform.GetComponent<DG_FishingAtlasObject>().LockItem = false;
        GiveAllFishingID();
    }
    public void GiveAllFishingID()
    {
        DG_FishingCompendium ItemDB = QuickFindInEditor.GetEditorFishingCompendium();
        Transform ItemDatabaseRoot = ItemDB.transform;

        List<DG_FishingAtlasObject> AddObjects = new List<DG_FishingAtlasObject>();

        for (int iN = 0; iN < ItemDatabaseRoot.childCount; iN++)
        {
            Transform Child = ItemDatabaseRoot.GetChild(iN);
            for (int i = 0; i < Child.childCount; i++)
            {
                DG_FishingAtlasObject IO = Child.GetChild(i).GetComponent<DG_FishingAtlasObject>();
                if (!IO.LockItem)
                {
                    IO.DatabaseID = ItemDB.ListCount;
                    ItemDB.ListCount++;
                    IO.LockItem = true;
                    AddObjects.Add(IO);
                }

                IO.gameObject.name = IO.DatabaseID.ToString() + " - " + IO.Name;
            }
        }

        if (AddObjects.Count == 0) { Debug.Log("All Items Accounted For. :)"); return; }

        int Index = 0;
        DG_FishingAtlasObject[] NewArray = new DG_FishingAtlasObject[ItemDB.ListCount];
        for (int i = 0; i < ItemDB.ItemCatagoryList.Length; i++)
        {
            DG_FishingAtlasObject IO = ItemDB.ItemCatagoryList[i];
            if (IO == null) continue;
            NewArray[Index] = IO; Index++;
        }
        for (int i = 0; i < AddObjects.Count; i++)
        { NewArray[Index] = AddObjects[i]; Index++; }

        ItemDB.ItemCatagoryList = NewArray;
        Debug.Log("Safely Added New Item to Database.");
    }
    #endregion

    #region ObjectCompendium
    void AssignNewObjectCompendiumID()
    {
        transform.GetComponent<DG_BreakableObjectItem>().LockItem = false;
        GiveAllObjectCompendiumID();
    }
    public void GiveAllObjectCompendiumID()
    {
        DG_BreakableObjectsAtlas ItemDB = QuickFindInEditor.GetBreakableObjectsCompendium();
        Transform ItemDatabaseRoot = ItemDB.transform;

        List<DG_BreakableObjectItem> AddObjects = new List<DG_BreakableObjectItem>();

        for (int iN = 0; iN < ItemDatabaseRoot.childCount; iN++)
        {
            Transform Child = ItemDatabaseRoot.GetChild(iN);
            for (int i = 0; i < Child.childCount; i++)
            {
                DG_BreakableObjectItem IO = Child.GetChild(i).GetComponent<DG_BreakableObjectItem>();
                if (!IO.LockItem)
                {
                    IO.DatabaseID = ItemDB.ListCount;
                    ItemDB.ListCount++;
                    IO.LockItem = true;
                    AddObjects.Add(IO);
                }

                IO.gameObject.name = IO.DatabaseID.ToString() + " - " + IO.Name;
            }
        }

        if (AddObjects.Count == 0) { Debug.Log("All Items Accounted For. :)"); return; }

        int Index = 0;
        DG_BreakableObjectItem[] NewArray = new DG_BreakableObjectItem[ItemDB.ListCount];
        for (int i = 0; i < ItemDB.ItemCatagoryList.Length; i++)
        {
            DG_BreakableObjectItem IO = ItemDB.ItemCatagoryList[i];
            if (IO == null) continue;
            NewArray[Index] = IO; Index++;
        }
        for (int i = 0; i < AddObjects.Count; i++)
        { NewArray[Index] = AddObjects[i]; Index++; }

        ItemDB.ItemCatagoryList = NewArray;
        Debug.Log("Safely Added New Item to Database.");
    }
    #endregion

    #region Shops Compendium
    void AssignNewShopCompendiumID()
    {
        transform.GetComponent<DG_ShopAtlasObject>().LockItem = false;
        GiveAllShopCompendiumID();
    }
    public void GiveAllShopCompendiumID()
    {
        DG_ShopAtlas ItemDB = QuickFindInEditor.GetShopCompendium();
        Transform ItemDatabaseRoot = ItemDB.transform;

        List<DG_ShopAtlasObject> AddObjects = new List<DG_ShopAtlasObject>();

        for (int iN = 0; iN < ItemDatabaseRoot.childCount; iN++)
        {
            Transform Child = ItemDatabaseRoot.GetChild(iN);
            for (int i = 0; i < Child.childCount; i++)
            {
                DG_ShopAtlasObject IO = Child.GetChild(i).GetComponent<DG_ShopAtlasObject>();
                if (!IO.LockItem)
                {
                    IO.DatabaseID = ItemDB.ListCount;
                    ItemDB.ListCount++;
                    IO.LockItem = true;
                    AddObjects.Add(IO);
                }

                IO.gameObject.name = IO.DatabaseID.ToString() + " - " + IO.Name;
            }
        }

        if (AddObjects.Count == 0) { Debug.Log("All Items Accounted For. :)"); return; }

        int Index = 0;
        DG_ShopAtlasObject[] NewArray = new DG_ShopAtlasObject[ItemDB.ListCount];
        for (int i = 0; i < ItemDB.ItemCatagoryList.Length; i++)
        {
            DG_ShopAtlasObject IO = ItemDB.ItemCatagoryList[i];
            if (IO == null) continue;
            NewArray[Index] = IO; Index++;
        }
        for (int i = 0; i < AddObjects.Count; i++)
        { NewArray[Index] = AddObjects[i]; Index++; }

        ItemDB.ItemCatagoryList = NewArray;
        Debug.Log("Safely Added New Item to Database.");
    }
    #endregion

    #region Crafting Compendium
    void AssignNewCraftingCompendiumID()
    {
        transform.GetComponent<DG_CraftingDictionaryItem>().LockItem = false;
        GiveAllCraftingCompendiumID();
    }
    public void GiveAllCraftingCompendiumID()
    {
        DG_CraftingDictionary ItemDB = QuickFindInEditor.GetCraftingCompendium();
        Transform ItemDatabaseRoot = ItemDB.transform;

        List<DG_CraftingDictionaryItem> AddObjects = new List<DG_CraftingDictionaryItem>();

        for (int iN = 0; iN < ItemDatabaseRoot.childCount; iN++)
        {
            Transform Child = ItemDatabaseRoot.GetChild(iN);
            for (int i = 0; i < Child.childCount; i++)
            {
                DG_CraftingDictionaryItem IO = Child.GetChild(i).GetComponent<DG_CraftingDictionaryItem>();
                if (!IO.LockItem)
                {
                    IO.DatabaseID = ItemDB.ListCount;
                    ItemDB.ListCount++;
                    IO.LockItem = true;
                    AddObjects.Add(IO);
                }

                IO.gameObject.name = IO.DatabaseID.ToString() + " - " + IO.Name;
            }
        }

        if (AddObjects.Count == 0) { Debug.Log("All Items Accounted For. :)"); return; }

        int Index = 0;
        DG_CraftingDictionaryItem[] NewArray = new DG_CraftingDictionaryItem[ItemDB.ListCount];
        for (int i = 0; i < ItemDB.ItemCatagoryList.Length; i++)
        {
            DG_CraftingDictionaryItem IO = ItemDB.ItemCatagoryList[i];
            if (IO == null) continue;
            NewArray[Index] = IO; Index++;
        }
        for (int i = 0; i < AddObjects.Count; i++)
        { NewArray[Index] = AddObjects[i]; Index++; }

        ItemDB.ItemCatagoryList = NewArray;
        Debug.Log("Safely Added New Item to Database.");
    }
    #endregion



    //Take Care to not use this post Game Release or every Users save files will turn to monkey garbage.
    //[Button(ButtonSizes.Small)]
    public void FlushEverything()
    {
        switch (DBType)
        {
            case DatabaseType.Item:
                {
                    DG_ItemsDatabase ItemDB = QuickFindInEditor.GetEditorItemDatabase();
                    ItemDB.ListCount = 0;
                    ItemDB.ItemCatagoryList = new DG_ItemObject[0];
                    for (int i = 0; i < ItemDB.transform.childCount; i++)
                    {
                        for(int iN = 0; iN < ItemDB.transform.GetChild(i).childCount; iN++)
                            ItemDB.transform.GetChild(i).GetChild(iN).GetComponent<DG_ItemObject>().LockItem = false;
                    }
                    GiveAllItemsDatabaseID(); break;
                }
            case DatabaseType.Character:
                {
                    DG_CharacterDatabase ItemDB = QuickFindInEditor.GetEditorCharacterDatabase();
                    ItemDB.ListCount = 0;
                    ItemDB.ItemCatagoryList = new DG_CharacterObject[0];
                    for (int i = 0; i < ItemDB.transform.childCount; i++)
                    {
                        for (int iN = 0; iN < ItemDB.transform.GetChild(i).childCount; iN++)
                            ItemDB.transform.GetChild(i).GetChild(iN).GetComponent<DG_CharacterObject>().LockItem = false;
                    }
                    GiveAllCharacterDatabaseID(); break;
                }
            case DatabaseType.DialogueTree:
                {
                    DG_DialogueManager ItemDB = QuickFindInEditor.GetEditorDialogueManager();
                    ItemDB.ListCount = 0;
                    ItemDB.ItemCatagoryList = new NodeLink[0];
                    for (int i = 0; i < ItemDB.transform.childCount; i++)
                    {
                        for (int iN = 0; iN < ItemDB.transform.GetChild(i).childCount; iN++)
                            ItemDB.transform.GetChild(i).GetChild(iN).GetComponent<NodeLink>().LockItem = false;
                    }
                    GiveAllDialogueTreeDatabaseID(); break;
                }
            case DatabaseType.Word:
                {
                    DG_WordDatabase ItemDB = QuickFindInEditor.GetEditorWordDatabase();
                    ItemDB.ListCount = 0;
                    ItemDB.ItemCatagoryList = new DG_WordObject[0];
                    for (int i = 0; i < ItemDB.transform.childCount; i++)
                    {
                        for (int iN = 0; iN < ItemDB.transform.GetChild(i).childCount; iN++)
                            ItemDB.transform.GetChild(i).GetChild(iN).GetComponent<DG_WordObject>().LockItem = false;
                    }
                    GiveAllWordDatabaseID(); break;
                }
            case DatabaseType.Quest:
                {
                    DG_QuestDatabase ItemDB = QuickFindInEditor.GetEditorQuestDatabase();
                    ItemDB.ListCount = 0;
                    ItemDB.ItemCatagoryList = new DG_QuestObject[0];
                    for (int i = 0; i < ItemDB.transform.childCount; i++)
                    {
                        for (int iN = 0; iN < ItemDB.transform.GetChild(i).childCount; iN++)
                            ItemDB.transform.GetChild(i).GetChild(iN).GetComponent<DG_QuestObject>().LockItem = false;
                    }
                    GiveAllQuestDatabaseID(); break;
                }
            case DatabaseType.FishingCompendium:
                {
                    DG_FishingCompendium ItemDB = QuickFindInEditor.GetEditorFishingCompendium();
                    ItemDB.ListCount = 0;
                    ItemDB.ItemCatagoryList = new DG_FishingAtlasObject[0];
                    for (int i = 0; i < ItemDB.transform.childCount; i++)
                    {
                        for (int iN = 0; iN < ItemDB.transform.GetChild(i).childCount; iN++)
                            ItemDB.transform.GetChild(i).GetChild(iN).GetComponent<DG_FishingAtlasObject>().LockItem = false;
                    }
                    GiveAllFishingID(); break;
                }
            case DatabaseType.ObjectCompendium:
                {
                    DG_BreakableObjectsAtlas ItemDB = QuickFindInEditor.GetBreakableObjectsCompendium();
                    ItemDB.ListCount = 0;
                    ItemDB.ItemCatagoryList = new DG_BreakableObjectItem[0];
                    for (int i = 0; i < ItemDB.transform.childCount; i++)
                    {
                        for (int iN = 0; iN < ItemDB.transform.GetChild(i).childCount; iN++)
                            ItemDB.transform.GetChild(i).GetChild(iN).GetComponent<DG_BreakableObjectItem>().LockItem = false;
                    }
                    GiveAllObjectCompendiumID(); break;
                }
            case DatabaseType.ShopsCompendium:
                {
                    DG_ShopAtlas ItemDB = QuickFindInEditor.GetShopCompendium();
                    ItemDB.ListCount = 0;
                    ItemDB.ItemCatagoryList = new DG_ShopAtlasObject[0];
                    for (int i = 0; i < ItemDB.transform.childCount; i++)
                    {
                        for (int iN = 0; iN < ItemDB.transform.GetChild(i).childCount; iN++)
                            ItemDB.transform.GetChild(i).GetChild(iN).GetComponent<DG_ShopAtlasObject>().LockItem = false;
                    }
                    GiveAllShopCompendiumID(); break;
                }
            case DatabaseType.CraftingCompendium:
                {
                    DG_CraftingDictionary ItemDB = QuickFindInEditor.GetCraftingCompendium();
                    ItemDB.ListCount = 0;
                    ItemDB.ItemCatagoryList = new DG_CraftingDictionaryItem[0];
                    for (int i = 0; i < ItemDB.transform.childCount; i++)
                    {
                        for (int iN = 0; iN < ItemDB.transform.GetChild(i).childCount; iN++)
                            ItemDB.transform.GetChild(i).GetChild(iN).GetComponent<DG_CraftingDictionaryItem>().LockItem = false;
                    }
                    GiveAllCraftingCompendiumID(); break;
                }
        }
    }
}

#endif
