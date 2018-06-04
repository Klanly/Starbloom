using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Sirenix.OdinInspector;


public class DG_LocalDataHandler : MonoBehaviour {


    public string SaveFileName;
    string CacheDirectory;
    string SaveDirectory;
    [HideInInspector] public GameObject DataChild;


    private void Awake()
    {
        QuickFind.SaveHandler = this;
        CacheDirectory = Environment.CurrentDirectory + "\\SaveFiles/";
        DataChild = new GameObject();
        DataChild.transform.SetParent(transform);
    }






    [Button(ButtonSizes.Medium)]
    public void FirstSaveButton()
    {
        SaveGame(true);
    }
    [Button(ButtonSizes.Small)]
    public void NotFirstSaveButton()
    {
        SaveGame(false);
    }



    public void SaveGame(bool FirstEverSave)
    {
        //Change Save Name if Farm Name is duplicate.
        if (FirstEverSave = CheckIfOverwritingPreviousDirectory(CacheDirectory, SaveFileName))
        {
            string KnownFileRequested = SaveFileName;
            for(int i = 2; i < int.MaxValue; i++)
            {
                SaveFileName = KnownFileRequested + i.ToString();
                if(!CheckIfOverwritingPreviousDirectory(CacheDirectory, SaveFileName))
                    break;
            }     
        }

        SaveDirectory = FindOrCreateSaveDirectory(CacheDirectory, SaveFileName);

        GatherPlayerDataInts(true);
        GatherPlayerDataStrings(true);

        GatherWorldInts(true);

        Debug.Log("Game Saved");
    }
    [Button(ButtonSizes.Medium)]
    public void LoadGame()
    {
        SaveDirectory = FindOrCreateSaveDirectory(CacheDirectory, SaveFileName);

        GetIntValues(null, true);
        GetStringValues(null, true);
        QuickFind.GUI_Inventory.UpdateInventoryVisuals();

        QuickFind.NetworkObjectManager.ClearObjects();
        GetWorldInts(null, true);
        QuickFind.NetworkObjectManager.GenerateSceneObjects(QuickFind.NetworkSync.CurrentScene);

        QuickFind.NetworkSync.GameWasLoaded();
        Debug.Log("Game Loaded");
    }








    public List<int> GatherPlayerDataInts(bool ToDisk)
    {
        string Directory = "";
        List<int> IntData = new List<int>();
        DG_PlayerCharacters PlayerData = QuickFind.Farm;
        //
        if (ToDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "PlayerData") + "/";
        if (!ToDisk) IntData.Add(PlayerData.PlayerCharacters.Count); else SaveInt(PlayerData.PlayerCharacters.Count, Directory + "Count");
        if (!ToDisk) IntData.Add(PlayerData.SharedMoney); else SaveInt(PlayerData.SharedMoney, Directory + "SharedMoney");
        if (!ToDisk) IntData.Add(PlayerData.Year); else SaveInt(PlayerData.Year, Directory + "Year");
        if (!ToDisk) IntData.Add(PlayerData.Month); else SaveInt(PlayerData.Month, Directory + "Month");
        if (!ToDisk) IntData.Add(PlayerData.Day); else SaveInt(PlayerData.Day, Directory + "Day");
        //
        for (int i = 0; i < PlayerData.PlayerCharacters.Count; i++)
        {
            DG_PlayerCharacters.PlayerCharacter PC = PlayerData.PlayerCharacters[i];
            DG_PlayerCharacters.CharacterEquipment CE = PC.Equipment;
            string PlayerNum = i.ToString();
            string KnownDirectory;

            if (ToDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "PlayerNonCombatStats" + PlayerNum + "/");
            if (!ToDisk) IntData.Add(PC.NonCombatSkillEXP.Farming); else SaveInt(PC.NonCombatSkillEXP.Farming, Directory + "Farming");
            if (!ToDisk) IntData.Add(PC.NonCombatSkillEXP.Mining); else SaveInt(PC.NonCombatSkillEXP.Mining, Directory + "Mining");
            if (!ToDisk) IntData.Add(PC.NonCombatSkillEXP.Foraging); else SaveInt(PC.NonCombatSkillEXP.Foraging, Directory + "Foraging");
            if (!ToDisk) IntData.Add(PC.NonCombatSkillEXP.Fishing); else SaveInt(PC.NonCombatSkillEXP.Fishing, Directory + "Fishing");

            //
            if (ToDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "PlayerEquipment" + PlayerNum + "/");
            if (!ToDisk) IntData.Add(CE.HatId); else SaveInt(CE.HatId, Directory + "HatId");
            if (!ToDisk) IntData.Add(CE.Ring1); else SaveInt(CE.Ring1, Directory + "Ring1");
            if (!ToDisk) IntData.Add(CE.Ring2); else SaveInt(CE.Ring2, Directory + "Ring2");
            if (!ToDisk) IntData.Add(CE.Boots); else SaveInt(CE.Boots, Directory + "Boots");
            //
            // Rucksack
            if (ToDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "Rucksack" + PlayerNum + "/");
            if (!ToDisk) IntData.Add(CE.RuckSackUnlockedSize); else SaveInt(CE.RuckSackUnlockedSize, Directory + "RuckSackUnlockedSize");
            KnownDirectory = Directory;
            for (int iN = 0; iN < CE.RuckSackUnlockedSize; iN++)
            {
                DG_PlayerCharacters.RucksackSlot RSlot = CE.RucksackSlots[iN];
                string RuckSackSlot = iN.ToString();
                //
                if (ToDisk) Directory = FindOrCreateSaveDirectory(KnownDirectory, "RS" + RuckSackSlot + "/");
                if (!ToDisk) IntData.Add(RSlot.ContainedItem);        else SaveInt(RSlot.ContainedItem, Directory + "ContainedItem");
                if (!ToDisk) IntData.Add(RSlot.CurrentStackActive);   else SaveInt(RSlot.CurrentStackActive, Directory + "CurrentStackActive");
                if (!ToDisk) IntData.Add(RSlot.LowValue);             else SaveInt(RSlot.LowValue, Directory + "LowValue");
                if (!ToDisk) IntData.Add(RSlot.NormalValue);          else SaveInt(RSlot.NormalValue, Directory + "NormalValue");
                if (!ToDisk) IntData.Add(RSlot.HighValue);            else SaveInt(RSlot.HighValue, Directory + "HighValue");
                if (!ToDisk) IntData.Add(RSlot.MaximumValue); else SaveInt(RSlot.MaximumValue, Directory + "MaximumValue");
            }

            //Acheivements
            DG_PlayerCharacters.CharacterAchievements Acheivements = PC.Acheivements;

            if (ToDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "FishLarge" + PlayerNum + "/");
            if (!ToDisk) IntData.Add(Acheivements.LargestFishCaught.Length); else SaveInt(Acheivements.LargestFishCaught.Length, Directory + "FishAwardsSize");
            KnownDirectory = Directory;
            for (int iN = 0; iN < Acheivements.LargestFishCaught.Length; iN++)
            {
                int FishAward = Acheivements.LargestFishCaught[iN];
                string FishAcheivementSlot = iN.ToString();
                //
                if (ToDisk) Directory = FindOrCreateSaveDirectory(KnownDirectory, "Fish" + FishAcheivementSlot + "/");
                if (!ToDisk) IntData.Add(FishAward); else SaveInt(FishAward, Directory + "Largest");
            }
        }
        return IntData;
    }
    public void GetIntValues(int[] IntValues, bool FromDisk)
    {
        string Directory = "";
        int Index = 0;
        int PlayerCount = 0;
        DG_PlayerCharacters PlayerData = QuickFind.Farm;
        //
        if (FromDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "PlayerData") + "/";
        if (!FromDisk) PlayerCount = IntValues[Index]; else PlayerCount = LoadInt(Directory + "Count"); Index++;
        if (!FromDisk) PlayerData.SharedMoney = IntValues[Index]; else PlayerData.SharedMoney = LoadInt(Directory + "SharedMoney"); Index++;
        if (!FromDisk) PlayerData.Year = IntValues[Index]; else PlayerData.Year = LoadInt(Directory + "Year"); Index++;
        if (!FromDisk) PlayerData.Month = IntValues[Index]; else PlayerData.Month = LoadInt(Directory + "Month"); Index++;
        if (!FromDisk) PlayerData.Day = IntValues[Index]; else PlayerData.Day = LoadInt(Directory + "Day"); Index++;
        //
        for (int i = 0; i < PlayerCount; i++)
        {
            DG_PlayerCharacters.PlayerCharacter PC = PlayerData.PlayerCharacters[i];
            DG_PlayerCharacters.CharacterEquipment CE = PC.Equipment;
            string PlayerNum = i.ToString();
            string KnownDirectory;

            if (FromDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "PlayerNonCombatStats" + PlayerNum + "/");
            if (!FromDisk) PC.NonCombatSkillEXP.Farming = IntValues[Index]; else PC.NonCombatSkillEXP.Farming = LoadInt(Directory + "Farming"); Index++;
            if (!FromDisk) PC.NonCombatSkillEXP.Mining = IntValues[Index]; else PC.NonCombatSkillEXP.Mining = LoadInt(Directory + "Mining"); Index++;
            if (!FromDisk) PC.NonCombatSkillEXP.Foraging = IntValues[Index]; else PC.NonCombatSkillEXP.Foraging = LoadInt(Directory + "Foraging"); Index++;
            if (!FromDisk) PC.NonCombatSkillEXP.Fishing = IntValues[Index]; else PC.NonCombatSkillEXP.Fishing = LoadInt(Directory + "Fishing"); Index++;

            //
            if (FromDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "PlayerEquipment" + PlayerNum + "/");
            if (!FromDisk) CE.HatId = IntValues[Index]; else CE.HatId = LoadInt(Directory + "HatId"); Index++;
            if (!FromDisk) CE.Ring1 = IntValues[Index]; else CE.Ring1 = LoadInt(Directory + "Ring1"); Index++;
            if (!FromDisk) CE.Ring2 = IntValues[Index]; else CE.Ring2 = LoadInt(Directory + "Ring2"); Index++;
            if (!FromDisk) CE.Boots = IntValues[Index]; else CE.Boots = LoadInt(Directory + "Boots"); Index++;
            //
            if (FromDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "Rucksack" + PlayerNum + "/");
            if (!FromDisk) CE.RuckSackUnlockedSize = IntValues[Index]; else CE.RuckSackUnlockedSize = LoadInt(Directory + "RuckSackUnlockedSize"); Index++;
            //

            //RuckSack
            DG_PlayerCharacters.RucksackSlot[] RS = CE.RucksackSlots;
            KnownDirectory = Directory;
            for (int iN = 0; iN < CE.RuckSackUnlockedSize; iN++)
            {
                DG_PlayerCharacters.RucksackSlot RSlot = RS[iN];
                string RuckSackSlot = iN.ToString();
                //
                if (FromDisk) Directory = FindOrCreateSaveDirectory(KnownDirectory, "RS" + RuckSackSlot + "/");
                if (!FromDisk) RSlot.ContainedItem = IntValues[Index]; else RSlot.ContainedItem = LoadInt(Directory + "ContainedItem"); Index++;
                if (!FromDisk) RSlot.CurrentStackActive = IntValues[Index]; else RSlot.CurrentStackActive = LoadInt(Directory + "CurrentStackActive"); Index++;
                if (!FromDisk) RSlot.LowValue = IntValues[Index]; else RSlot.LowValue = LoadInt(Directory + "LowValue"); Index++;
                if (!FromDisk) RSlot.NormalValue = IntValues[Index]; else RSlot.NormalValue = LoadInt(Directory + "NormalValue"); Index++;
                if (!FromDisk) RSlot.HighValue = IntValues[Index]; else RSlot.HighValue = LoadInt(Directory + "HighValue"); Index++;
                if (!FromDisk) RSlot.MaximumValue = IntValues[Index]; else RSlot.MaximumValue = LoadInt(Directory + "MaximumValue"); Index++;
            }

            //Acheivements
            DG_PlayerCharacters.CharacterAchievements Acheivements = PC.Acheivements;

            if (FromDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "FishLarge" + PlayerNum + "/");
            int count;
            if (!FromDisk) count = IntValues[Index]; else count = LoadInt(Directory + "FishAwardsSize"); Index++;
            KnownDirectory = Directory;
            for (int iN = 0; iN < count; iN++)
            {
                string FishAcheivementSlot = iN.ToString();
                if (FromDisk) Directory = FindOrCreateSaveDirectory(KnownDirectory, "Fish" + FishAcheivementSlot + "/");
                if (!FromDisk) Acheivements.LargestFishCaught[iN] = IntValues[Index]; else Acheivements.LargestFishCaught[iN] = LoadInt(Directory + "Largest"); Index++;
            }


        }
    }


    public List<string> GatherPlayerDataStrings(bool ToDisk)
    {
        string Directory = "";
        List<string> StringData = new List<string>();
        DG_PlayerCharacters PlayerData = QuickFind.Farm;
        //
        if (ToDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "PlayerData") + "/";
        if (!ToDisk) StringData.Add(PlayerData.FarmName); else SaveString(PlayerData.FarmName, Directory + "FarmName");
        string CharCount = PlayerData.PlayerCharacters.Count.ToString();
        if (!ToDisk) StringData.Add(CharCount); else SaveString(CharCount, Directory + "CharCount");
        //
        for (int i = 0; i < PlayerData.PlayerCharacters.Count; i++)
        {
            DG_PlayerCharacters.PlayerCharacter PC = PlayerData.PlayerCharacters[i];
            string PlayerNum = i.ToString();
            //
            if (ToDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "PlayerCharStrings" + PlayerNum.ToString() + "/");
            if (!ToDisk) StringData.Add(PC.Name); else SaveString(PC.Name, Directory + "Name");
        }
        return StringData;
    }
    public void GetStringValues(string[] StringValues, bool FromDisk)
    {
        string Directory = "";
        int Index = 0;
        string PlayerCount;
        DG_PlayerCharacters PlayerData = QuickFind.Farm;
        //
        if (FromDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "PlayerData") + "/";
        if (!FromDisk) PlayerData.FarmName = StringValues[Index]; else PlayerData.FarmName = LoadString(Directory + "FarmName"); Index++;
        if (!FromDisk) PlayerCount = StringValues[Index]; else PlayerCount = LoadString(Directory + "CharCount"); Index++;
        //
        int count = int.Parse(PlayerCount);
        for (int i = 0; i < count; i++)
        {
            DG_PlayerCharacters.PlayerCharacter PC = PlayerData.PlayerCharacters[i];
            string PlayerNum = i.ToString();
            //
            if (FromDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "PlayerCharStrings" + PlayerNum.ToString() + "/");
            if (!FromDisk) PC.Name = StringValues[Index]; else PC.Name = LoadString(Directory + "Name"); Index++;
        }
    }







    //WorldObjects
    public List<int> GatherWorldInts(bool ToDisk)
    {
        string Directory = "";
        List<int> IntData = new List<int>();
        Transform NOM = QuickFind.NetworkObjectManager.transform;
        //
        if (ToDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "WorldData") + "/";
        if (!ToDisk) IntData.Add(NOM.childCount); else SaveInt(NOM.childCount, Directory + "Count");
        //
        for (int i = 0; i < NOM.childCount; i++)
        {
            Transform Child = NOM.GetChild(i);
            NetworkScene NS = Child.GetComponent<NetworkScene>();
            string PlayerNum = i.ToString();
            //
            if (ToDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "WorldDataScene") + "/";
            if (!ToDisk) IntData.Add(NS.SceneID); else SaveInt(NS.SceneID, Directory + "SceneID");
            if (!ToDisk) IntData.Add(Child.childCount); else SaveInt(Child.childCount, Directory + "ChildCount");
            //
            string KnownDirectory = Directory;
            for (int iN = 0; iN < Child.childCount; iN++)
            {
                NetworkObject NO = Child.GetChild(iN).GetComponent<NetworkObject>();
                string ChildChild = iN.ToString();
                //
                if (ToDisk) Directory = FindOrCreateSaveDirectory(KnownDirectory, "CC" + ChildChild.ToString() + "/");
                if (!ToDisk) IntData.Add(NO.ItemRefID); else SaveInt(NO.ItemRefID, Directory + "ItemRefID");
                if (!ToDisk) IntData.Add(NO.ItemQualityLevel); else SaveInt(NO.ItemQualityLevel, Directory + "ItemGrowthLevel");
                if (!ToDisk) IntData.Add(NO.PositionX); else SaveInt(NO.PositionX, Directory + "x");
                if (!ToDisk) IntData.Add(NO.PositionY); else SaveInt(NO.PositionY, Directory + "y");
                if (!ToDisk) IntData.Add(NO.PositionZ); else SaveInt(NO.PositionZ, Directory + "z");
                if (!ToDisk) IntData.Add(NO.YFacing); else SaveInt(NO.YFacing, Directory + "YFacing");


                int isTrue = NO.isWaterable ? 1 : 0; if (!ToDisk) IntData.Add(isTrue); else SaveInt(isTrue, Directory + "IsWaterable");
                if (isTrue == 1)
                {
                    isTrue = NO.HasBeenWatered ? 1 : 0; if (!ToDisk) IntData.Add(isTrue); else SaveInt(isTrue, Directory + "HasBeenWatered");
                }

                isTrue = NO.isStorageContainer ? 1 : 0; if (!ToDisk) IntData.Add(isTrue); else SaveInt(isTrue, Directory + "IsStorageContainer");
                if (isTrue == 1)
                {
                    DG_PlayerCharacters.RucksackSlot[] RS = NO.StorageSlots;
                    isTrue = NO.isTreasureList ? 1 : 0;
                    if (!ToDisk) IntData.Add(isTrue); else SaveInt(isTrue, Directory + "IsTreasureList");
                    if (!ToDisk) IntData.Add(NO.StorageSlots.Length); else SaveInt(NO.StorageSlots.Length, Directory + "StorageContainerCount");

                    string SubKnownDirectory = Directory;
                    for (int iR = 0; iR < NO.StorageSlots.Length; iR++)
                    {
                        DG_PlayerCharacters.RucksackSlot RSlot = NO.StorageSlots[iR];
                        string RuckSackSlot = iR.ToString();
                        //
                        if (ToDisk) Directory = FindOrCreateSaveDirectory(SubKnownDirectory, "RS" + RuckSackSlot + "/");
                        if (!ToDisk) IntData.Add(RSlot.ContainedItem); else SaveInt(RSlot.ContainedItem, Directory + "ContainedItem");
                        if (!ToDisk) IntData.Add(RSlot.CurrentStackActive); else SaveInt(RSlot.CurrentStackActive, Directory + "CurrentStackActive");
                        if (!ToDisk) IntData.Add(RSlot.LowValue); else SaveInt(RSlot.LowValue, Directory + "LowValue");
                        if (!ToDisk) IntData.Add(RSlot.NormalValue); else SaveInt(RSlot.NormalValue, Directory + "NormalValue");
                        if (!ToDisk) IntData.Add(RSlot.HighValue); else SaveInt(RSlot.HighValue, Directory + "HighValue");
                        if (!ToDisk) IntData.Add(RSlot.MaximumValue); else SaveInt(RSlot.MaximumValue, Directory + "MaximumValue");
                    }
                }
            }
        }
        return IntData;
    }
    public void GetWorldInts(int[] IntValues, bool FromDisk)
    {
        string Directory = "";
        int Index = 0;
        int SceneCount = 0;
        Transform NOM = QuickFind.NetworkObjectManager.transform;

        if (FromDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "WorldData") + "/";
        if (!FromDisk) SceneCount = IntValues[Index]; else SceneCount = LoadInt(Directory + "Count"); Index++;
        //
        for (int i = 0; i < SceneCount; i++)
        {
            int SceneID;
            if (FromDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "WorldDataScene") + "/";
            if (!FromDisk) SceneID = IntValues[Index]; else SceneID = LoadInt(Directory + "SceneID"); Index++;
            //
            NetworkScene NS = QuickFind.NetworkObjectManager.GetSceneByID(SceneID);
            Transform Child = NS.transform;
            int count;
            //
            if (!FromDisk) count = IntValues[Index]; else count = LoadInt(Directory + "ChildCount"); Index++;
            //
            string KnownDirectory = Directory;
            for (int iN = 0; iN < count; iN++)
            {
                NetworkObject NO = DataChild.AddComponent<NetworkObject>();
                NS.NetworkObjectList.Add(NO);

                string ChildChild = iN.ToString();
                //
                if (FromDisk) Directory = FindOrCreateSaveDirectory(KnownDirectory, "CC" + ChildChild + "/");
                if (!FromDisk) NO.ItemRefID = IntValues[Index];     else NO.ItemRefID = LoadInt(Directory + "ItemRefID"); Index++;
                if (!FromDisk) NO.ItemQualityLevel = IntValues[Index]; else NO.ItemQualityLevel = LoadInt(Directory + "ItemGrowthLevel"); Index++;
                if (!FromDisk) NO.PositionX = IntValues[Index]; else NO.PositionX = LoadInt(Directory + "x"); Index++;
                if (!FromDisk) NO.PositionY = IntValues[Index]; else NO.PositionY = LoadInt(Directory + "y"); Index++;
                if (!FromDisk) NO.PositionZ = IntValues[Index]; else NO.PositionZ = LoadInt(Directory + "z"); Index++;
                if (!FromDisk) NO.YFacing = IntValues[Index]; else NO.YFacing = LoadInt(Directory + "YFacing"); Index++;

                NO.transform.position = QuickFind.ConvertIntsToPosition(NO.PositionX, NO.PositionY, NO.PositionZ);
                NO.transform.eulerAngles = new Vector3(0, QuickFind.ConvertIntToFloat(NO.YFacing), 0);
                //

                int StorageValue;
                if (!FromDisk) StorageValue = IntValues[Index]; else StorageValue = LoadInt(Directory + "IsWaterable"); Index++;
                bool isTrue = false; if (StorageValue == 1) isTrue = true; NO.isWaterable = isTrue;
                if (isTrue)
                {
                    if (!FromDisk) StorageValue = IntValues[Index]; else StorageValue = LoadInt(Directory + "HasBeenWatered"); Index++;
                    isTrue = false; if (StorageValue == 1) isTrue = true; NO.HasBeenWatered = isTrue;
                }

                if (!FromDisk) StorageValue = IntValues[Index]; else StorageValue = LoadInt(Directory + "IsStorageContainer"); Index++;
                isTrue = false; if (StorageValue == 1) isTrue = true; NO.isStorageContainer = isTrue;
                if (isTrue)
                {
                    int Count = 0;
                    if (!FromDisk) StorageValue = IntValues[Index]; else StorageValue = LoadInt(Directory + "IsTreasureList"); Index++;
                    isTrue = false; if (StorageValue == 1) isTrue = true; NO.isTreasureList = isTrue;
                    if (!FromDisk) Count = IntValues[Index]; else Count = LoadInt(Directory + "StorageContainerCount"); Index++;
                    NO.StorageSlots = new DG_PlayerCharacters.RucksackSlot[Count];

                    string SubKnownDirectory = Directory;
                    DG_PlayerCharacters.RucksackSlot[] RS = NO.StorageSlots;
                    for (int iR = 0; iR < Count; iR++)
                    {
                        RS[iR] = new DG_PlayerCharacters.RucksackSlot();
                        DG_PlayerCharacters.RucksackSlot RSlot = RS[iR];
                        string RuckSackSlot = iR.ToString();
                        //
                        if (FromDisk) Directory = FindOrCreateSaveDirectory(SubKnownDirectory, "RS" + RuckSackSlot + "/");
                        if (!FromDisk) RSlot.ContainedItem = IntValues[Index]; else RSlot.ContainedItem = LoadInt(Directory + "ContainedItem"); Index++;
                        if (!FromDisk) RSlot.CurrentStackActive = IntValues[Index]; else RSlot.CurrentStackActive = LoadInt(Directory + "CurrentStackActive"); Index++;
                        if (!FromDisk) RSlot.LowValue = IntValues[Index]; else RSlot.LowValue = LoadInt(Directory + "LowValue"); Index++;
                        if (!FromDisk) RSlot.NormalValue = IntValues[Index]; else RSlot.NormalValue = LoadInt(Directory + "NormalValue"); Index++;
                        if (!FromDisk) RSlot.HighValue = IntValues[Index]; else RSlot.HighValue = LoadInt(Directory + "HighValue"); Index++;
                        if (!FromDisk) RSlot.MaximumValue = IntValues[Index]; else RSlot.MaximumValue = LoadInt(Directory + "MaximumValue"); Index++;
                    }
                }
            }
        }
    }








    #region Util
    //Util///////////////////////////////
    //Directory
    public static bool CheckIfOverwritingPreviousDirectory(string KnownDirectory, string SubFolder)
    {
        string SaveFileBaseLocation = KnownDirectory + "/" + SubFolder;
        if (Directory.Exists(SaveFileBaseLocation)) return true;
        else return false;
    }
    public static string FindOrCreateSaveDirectory(string KnownDirectory, string SubFolder)
    {
        string SaveFileBaseLocation = KnownDirectory + "/" + SubFolder;
        if (!Directory.Exists(SaveFileBaseLocation))
            Directory.CreateDirectory(SaveFileBaseLocation);
        return SaveFileBaseLocation;
    }
    //File
    public static bool CheckIfOverwritingPreviousFile(string KnownDirectory, string FileName)
    {
        string SaveFileBaseLocation = KnownDirectory + FileName;
        if (File.Exists(SaveFileBaseLocation)) return true;
        else return false;
    }
    #endregion




    #region Save Utilities
    ///////////////////////////////////////////////////////////////////////
    public static void SaveBool(bool Data, string FileName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(FileName);
        bf.Serialize(file, Data);
        file.Close();
    }
    public static bool LoadBool(string FileName)
    {
        bool ReturnArray = false;
        if (File.Exists(FileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(FileName, FileMode.Open);
            ReturnArray = (bool)bf.Deserialize(file);
            file.Close();
        }
        return ReturnArray;
    }
    public static void SaveInt(int Data, string FileName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(FileName);
        bf.Serialize(file, Data);
        file.Close();
    }
    public static int LoadInt(string FileName)
    {
        int ReturnArray = 0;
        if (File.Exists(FileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(FileName, FileMode.Open);
            ReturnArray = (int)bf.Deserialize(file);
            file.Close();
        }
        return ReturnArray;
    }
    public static void SaveInts(int[] Data, string FileName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(FileName);
        bf.Serialize(file, Data);
        file.Close();
    }
    public static int[] LoadInts(string FileName)
    {
        int[] ReturnArray = null;
        if (File.Exists(FileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(FileName, FileMode.Open);
            ReturnArray = (int[])bf.Deserialize(file);
            file.Close();
        }
        return ReturnArray;
    }
    ///////////////////////////////////////////////////////////////////////
    public static void SaveFloat(float Data, string FileName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(FileName);
        bf.Serialize(file, Data);
        file.Close();
    }
    public static float LoadFloat(string FileName)
    {
        float ReturnArray = 0;
        if (File.Exists(FileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(FileName, FileMode.Open);
            ReturnArray = (float)bf.Deserialize(file);
            file.Close();
        }
        return ReturnArray;
    }
    public static void SaveFloats(float[] Data, string FileName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(FileName);
        bf.Serialize(file, Data);
        file.Close();
    }
    public static float[] LoadFloats(string FileName)
    {
        float[] ReturnArray = null;
        if (File.Exists(FileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(FileName, FileMode.Open);
            ReturnArray = (float[])bf.Deserialize(file);
            file.Close();
        }
        return ReturnArray;
    }
    ///////////////////////////////////////////////////////////////////////
    public static void SaveString(string Data, string FileName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(FileName);
        bf.Serialize(file, Data);
        file.Close();
    }
    public static string LoadString(string FileName)
    {
        string ReturnArray = string.Empty;
        if (File.Exists(FileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(FileName, FileMode.Open);
            ReturnArray = (string)bf.Deserialize(file);
            file.Close();
        }
        return ReturnArray;
    }
    public static void SaveStrings(string[] Data, string FileName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(FileName);
        bf.Serialize(file, Data);
        file.Close();
    }
    public static string[] LoadStrings(string FileName)
    {
        string[] ReturnArray = null;
        if (File.Exists(FileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(FileName, FileMode.Open);
            ReturnArray = (string[])bf.Deserialize(file);
            file.Close();
        }
        return ReturnArray;
    }
    #endregion
}
