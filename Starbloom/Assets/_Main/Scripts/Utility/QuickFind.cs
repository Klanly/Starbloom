using UnityEngine;
using System.Collections;
using System.Collections.Generic;





#if UNITY_EDITOR
using UnityEditor.SceneManagement;
public static class QuickFindInEditor
{
    public static NodeViewOptions NodeViewOptions = null;
    public static NodeViewOptions GetEditorNodeViewOptions()
    {
        if (GameObject.Find("NodeViewerOptions") != null)
            return GameObject.Find("NodeViewerOptions").GetComponent<NodeViewOptions>();
        else
        {
            EditorSceneManager.OpenScene("Assets/_Main/Scenes/Additive/EditorOnly/EditorOnlyScene.unity", OpenSceneMode.Additive);
            return GameObject.Find("NodeViewerOptions").GetComponent<NodeViewOptions>();
        }
    }
    //Database
    public static DG_DialogueManager GetEditorDialogueManager() { return GameObject.Find("Dialogue Tree Database").GetComponent<DG_DialogueManager>(); }
    public static DG_WordDatabase GetEditorWordDatabase() { return GameObject.Find("Word Database").GetComponent<DG_WordDatabase>(); }
    public static DG_QuestDatabase GetEditorQuestDatabase() { return GameObject.Find("Quest Database").GetComponent<DG_QuestDatabase>(); }
    public static DG_ItemsDatabase GetEditorItemDatabase() { return GameObject.Find("Item Database").GetComponent<DG_ItemsDatabase>(); }
    public static DG_CharacterDatabase GetEditorCharacterDatabase() { return GameObject.Find("Character Database").GetComponent<DG_CharacterDatabase>(); }
    public static DG_ColorCodes GetEditorColorCodes() { return GameObject.Find("Color Database").GetComponent<DG_ColorCodes>(); }
    public static SceneIDList GetEditorSceneList() { return GameObject.Find("Scene ID List").GetComponent<SceneIDList>(); }
    public static DG_FishingCompendium GetEditorFishingCompendium() { return GameObject.Find("Fish Atlas").GetComponent<DG_FishingCompendium>(); }
    public static DG_BreakableObjectsAtlas GetBreakableObjectsCompendium() { return GameObject.Find("Cluster Rewards Atlas").GetComponent<DG_BreakableObjectsAtlas>(); }
    public static DG_ShopAtlas GetShopCompendium() { return GameObject.Find("Shop Type Atlas").GetComponent<DG_ShopAtlas>(); }
    public static DG_CraftingDictionary GetCraftingCompendium() { return GameObject.Find("Crafting Dictionary").GetComponent<DG_CraftingDictionary>(); }
    public static DG_PrefabPoolDictionary GetPrefabPool() { return GameObject.Find("Prefab Pool Dictionary").GetComponent<DG_PrefabPoolDictionary>(); }
    public static DG_FXPoolDictionary GetFXPool() { return GameObject.Find("FX Pool Dictionary").GetComponent<DG_FXPoolDictionary>(); }

    //Save Data
    public static UserSettings GetEditorUserSettings() { return GameObject.Find("Player Settings").GetComponent<UserSettings>(); }

}
#endif





public static class QuickFind
{

    //GAME
    public static DG_PlayerCharacters Farm = null;
    public static DG_DebugSettings GameSettings = null;
    public static UserSettings UserSettings = null;




    //InputController
    public static Transform PlayerTrans = null;
    public static DG_PlayerInput InputController = null;
    public static DG_CharacterControllers CharacterManager = null;
    public static DG_InteractHandler InteractHandler = null;
    public static HotbarItemHandler ItemActivateableHandler = null;

    //Cameras
    public static CameraLogic PlayerCam;
    public static EZCameraShake.CameraShaker CameraShake = null;

    //Detection
    public static DG_ContextCheckHandler ContextDetectionHandler = null;
    public static GridPointDetector GridDetection = null;




    //GUI
    public static DG_GUIMainMenu MainMenuUI = null;
    public static DG_GUI_FadeScreen FadeScreen = null;
    public static NA_DialogueGUIController DialogueGUIController = null;
    public static DG_TextPrintout TextPrintout = null;
    public static DG_GUINameChange NameChangeUI = null;
    public static DG_GUIControllerGhange ControllerChange = null;
    public static DG_GUIContextHandler GUIContextHandler = null;
    public static DG_GUICharacterCreation CharacterCreation = null;
    public static DG_TooltipGUI TooltipHandler = null;
    public static DG_StorageGUI StorageUI = null;
    public static DG_FishingGUI FishingGUI = null;
    public static DG_SystemMessageGUI SystemMessageGUI = null;
    public static DG_ShopGUI ShopGUI = null;

    //GUI - Main Overview
    public static GuiMainGameplay GUI_MainOverview = null;
    public static DG_OverviewTabsGUI GUI_OverviewTabs = null;
    public static DG_InventoryGUI GUI_Inventory = null;
    public static DG_SkillsGUI GUI_Skills = null;
    public static DG_CraftingGUI GUI_Crafting = null;





    //Trackers
    public static DG_AcheivementTracker AcheivementTracker = null;
    public static DG_SkillTracker SkillTracker = null;

    //Item Compendiums
    public static DG_FishingCompendium FishingCompendium = null;
    public static DG_BreakableObjectsAtlas BreakableObjectsCompendium = null;
    public static DG_ShopAtlas ShopAtlas;
    public static DG_CraftingDictionary CraftingDictionary = null;





    //Managers
    public static DG_Inventory InventoryManager = null;
    public static DG_TreasureSelection TreasureManager = null;
    public static DG_WateringSystem WateringSystem = null;
    public static DG_BreakableObjectsHandler BreakableObjectsHandler = null;
    public static DG_ObjectPlacement ObjectPlacementManager = null;
    public static DG_NetworkGrowthHandler NetworkGrowthHandler = null;
    public static DG_MoneyHandler MoneyHandler = null;
    public static DG_ShippingBin ShippingBin = null;

    //Tools
    public static DG_HoeHandler HoeHandler = null;
    public static DG_WateringCan WateringCanHandler = null;
    public static DG_PickAxeHandler PickaxeHandler = null;

    //Fishing
    public static Fishing_MasterHandler FishingHandler = null;
    public static DG_FishingRoller FishingRoller = null;




    //Network
    public static ConnectAndJoinRandom NetworkMaster = null;
    public static DG_NetworkSync NetworkSync = null;
    public static NetworkObjectManager NetworkObjectManager = null;

    //Database
    public static DG_DialogueManager DialogueManager = null;
    public static DG_WordDatabase WordDatabase = null;
    public static DG_QuestDatabase QuestDatabase = null;
    public static DG_ItemsDatabase ItemDatabase = null;
    public static DG_CharacterDatabase CharacterDatabase = null;
    public static DG_ColorCodes ColorDatabase = null;
    public static SceneIDList SceneList = null;
    public static DG_TextLanguageFonts LanguageFonts = null;

    //Serialization
    public static DG_LocalDataHandler SaveHandler;



    //Environment
    public static WeatherHandler WeatherHandler = null;
    public static Tenkoku.Core.TenkokuModule WeatherModule = null;
    public static Suimono.Core.SuimonoModule WaterModule = null;
    public static Suimono.Core.SuimonoObject WaterObject = null;
    public static TimeHandler TimeHandler = null;
    public static FakeRainDropCollision RainDropHandler = null;



    //Pooling
    public static DG_PrefabPoolDictionary PrefabPool = null;
    public static DG_FXPoolDictionary FXPool = null;
















    //Utilities

    public static Transform FindTransform(Transform parent, string name) { Transform[] children = parent.GetComponentsInChildren<Transform>(); foreach (Transform child in children) { if (child.name == name) return child; } return null; }
    public static bool WithinDistance(Transform Object, Transform Target, float MasterMinDistance) { if (Vector3.Distance(Object.position, Target.position) < MasterMinDistance) return true; else return false; }
    public static bool WithinDistanceVec(Vector3 Object, Vector3 Target, float MasterMinDistance) { if (Vector3.Distance(Object, Target) < MasterMinDistance) return true; else return false; }
    public static int ConvertFloatToInt(float Incoming) { return (int)(Incoming * 100); }
    public static float ConvertIntToFloat(int Incoming) { return (float)Incoming / 100; }
    public static Vector3 ConvertIntsToPosition(int IncomingX, int IncomingY, int IncomingZ) { return new Vector3(ConvertIntToFloat(IncomingX), ConvertIntToFloat(IncomingY), ConvertIntToFloat(IncomingZ)); }


    public static Vector3 AngleLerp(Vector3 StartAngle, Vector3 FinishAngle, float t)
    {
        float xLerp = Mathf.LerpAngle(StartAngle.x, FinishAngle.x, t);
        float yLerp = Mathf.LerpAngle(StartAngle.y, FinishAngle.y, t);
        float zLerp = Mathf.LerpAngle(StartAngle.z, FinishAngle.z, t);
        Vector3 Lerped = new Vector3(xLerp, yLerp, zLerp);
        return Lerped;
    }

    public static int GetNextValueInArray(int current, int ArrayLength, bool Add, bool CanLoop)
    {
        int Return = current;
        if(Add) { Return++; if (Return == ArrayLength) { if (CanLoop) Return = 0; else Return--; } }
        else { Return--; if (Return < 0) { if (CanLoop) Return = ArrayLength - 1; else Return++; } }
        return Return;
    }
    public static void EnableCanvas(CanvasGroup C, bool isTrue)
    { float value = 0; if (isTrue) value = 1; C.alpha = value; C.interactable = isTrue; C.blocksRaycasts = isTrue; }

    public static int GetIfWithinBounds(int IncomingValue, int Min, int ArrayLength)
    {
        if (IncomingValue < Min) return Min;
        if (IncomingValue >= ArrayLength) return ArrayLength - 1;
        return IncomingValue;
    }
}
