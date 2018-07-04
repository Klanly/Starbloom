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

    [Header("Debug")]
    public bool PrintDebug;


    private void Awake()
    {
        QuickFind.SaveHandler = this;
        CacheDirectory = Environment.CurrentDirectory + "\\SaveFiles/";
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
        QuickFind.NetworkObjectManager.GenerateSceneObjects();

        QuickFind.NetworkSync.GameWasLoaded();
        Debug.Log("Game Loaded");
    }








    public List<int> GatherPlayerDataInts(bool ToDisk)
    {
        List<int> IntData = new List<int>();
        DG_PlayerCharacters PlayerData = QuickFind.Farm;
        //
        IntData.Add(PlayerData.PlayerCharacters.Count);
        IntData.Add(PlayerData.SharedMoney);
        IntData.Add(PlayerData.Year);
        IntData.Add(PlayerData.Month);
        IntData.Add(PlayerData.Day);
        //
        for (int i = 0; i < PlayerData.PlayerCharacters.Count; i++)
        {
            DG_PlayerCharacters.PlayerCharacter PC = PlayerData.PlayerCharacters[i];
            DG_PlayerCharacters.CharacterEquipment CE = PC.Equipment;

            IntData.Add((int)PC.CharacterGender);

            IntData.Add(PC.NonCombatSkillEXP.Farming);
            IntData.Add(PC.NonCombatSkillEXP.Mining);
            IntData.Add(PC.NonCombatSkillEXP.Foraging);
            IntData.Add(PC.NonCombatSkillEXP.Fishing);


            //
            // Equipment
            IntData.Add(CE.EquippedClothing.Count);
            for (int iN = 0; iN < CE.EquippedClothing.Count; iN++)
                IntData.Add(CE.EquippedClothing[iN]);

            //
            // Rucksack
            IntData.Add(CE.RuckSackUnlockedSize);
            for (int iN = 0; iN < CE.RuckSackUnlockedSize; iN++)
            {
                DG_PlayerCharacters.RucksackSlot RSlot = CE.RucksackSlots[iN];
                //
                IntData.Add(RSlot.ContainedItem);       
                IntData.Add(RSlot.CurrentStackActive);  
                IntData.Add(RSlot.LowValue);          
                IntData.Add(RSlot.NormalValue);       
                IntData.Add(RSlot.HighValue);      
                IntData.Add(RSlot.MaximumValue);
            }

            //Acheivements
            DG_PlayerCharacters.CharacterAchievements Acheivements = PC.Acheivements;

            IntData.Add(Acheivements.LargestFishCaught.Length);
            for (int iN = 0; iN < Acheivements.LargestFishCaught.Length; iN++)
            {
                int FishAward = Acheivements.LargestFishCaught[iN];
                //
                IntData.Add(FishAward);
            }
        }

        if (ToDisk)
            SaveInts(IntData.ToArray(), SaveDirectory + "/CharInts");

        if (PrintDebug) PrintFileSentSize(IntData.Count, false, ToDisk);

        return IntData;
    }
    public void GetIntValues(int[] IntValues, bool FromDisk)
    {
        if (FromDisk)
            IntValues = LoadInts(SaveDirectory + "/CharInts");

        if (PrintDebug) PrintFileSentSize(IntValues.Length, false, FromDisk);

        int Index = 0;
        int PlayerCount = 0;
        DG_PlayerCharacters PlayerData = QuickFind.Farm;
        //
        PlayerCount = IntValues[Index]; Index++;
        PlayerData.SharedMoney = IntValues[Index]; Index++;
        PlayerData.Year = IntValues[Index]; Index++;
        PlayerData.Month = IntValues[Index]; Index++;
        PlayerData.Day = IntValues[Index]; Index++;
        //
        for (int i = 0; i < PlayerCount; i++)
        {
            DG_PlayerCharacters.PlayerCharacter PC = PlayerData.PlayerCharacters[i];
            DG_PlayerCharacters.CharacterEquipment CE = PC.Equipment;

            PC.CharacterGender = (DG_PlayerCharacters.GenderValue)IntValues[Index]; Index++;

            PC.NonCombatSkillEXP.Farming = IntValues[Index]; Index++;
            PC.NonCombatSkillEXP.Mining = IntValues[Index]; Index++;
            PC.NonCombatSkillEXP.Foraging = IntValues[Index]; Index++;
            PC.NonCombatSkillEXP.Fishing = IntValues[Index]; Index++;




            //
            // Equipment
            CE.EquippedClothing = new List<int>();
            int Count = IntValues[Index]; Index++;
            for (int iN = 0; iN < Count; iN++)
                { CE.EquippedClothing.Add(IntValues[Index]); Index++; }

            //
            //RuckSack
            CE.RuckSackUnlockedSize = IntValues[Index]; Index++;

            DG_PlayerCharacters.RucksackSlot[] RS = CE.RucksackSlots;
            for (int iN = 0; iN < CE.RuckSackUnlockedSize; iN++)
            {
                DG_PlayerCharacters.RucksackSlot RSlot = RS[iN];
                //
                RSlot.ContainedItem = IntValues[Index]; Index++;
                RSlot.CurrentStackActive = IntValues[Index]; Index++;
                RSlot.LowValue = IntValues[Index]; Index++;
                RSlot.NormalValue = IntValues[Index]; Index++;
                RSlot.HighValue = IntValues[Index]; Index++;
                RSlot.MaximumValue = IntValues[Index]; Index++;
            }

            //Acheivements
            DG_PlayerCharacters.CharacterAchievements Acheivements = PC.Acheivements;

            int count;
            count = IntValues[Index]; Index++;
            for (int iN = 0; iN < count; iN++)
            {
                Acheivements.LargestFishCaught[iN] = IntValues[Index]; Index++;
            }
        }
    }


    public List<string> GatherPlayerDataStrings(bool ToDisk)
    {
        List<string> StringData = new List<string>();
        DG_PlayerCharacters PlayerData = QuickFind.Farm;
        //
        StringData.Add(PlayerData.FarmName);
        string CharCount = PlayerData.PlayerCharacters.Count.ToString();
        StringData.Add(CharCount);
        //
        for (int i = 0; i < PlayerData.PlayerCharacters.Count; i++)
        {
            DG_PlayerCharacters.PlayerCharacter PC = PlayerData.PlayerCharacters[i];
            //
            StringData.Add(PC.Name);
        }

        if(ToDisk)
            SaveStrings(StringData.ToArray(), SaveDirectory + "/CharStrings");

        return StringData;
    }
    public void GetStringValues(string[] StringValues, bool FromDisk)
    {
        if (FromDisk)
            StringValues = LoadStrings(SaveDirectory + "/CharStrings");


        int Index = 0;
        string PlayerCount;
        DG_PlayerCharacters PlayerData = QuickFind.Farm;
        //
        PlayerData.FarmName = StringValues[Index]; Index++;
        PlayerCount = StringValues[Index]; Index++;
        //
        int count = int.Parse(PlayerCount);
        for (int i = 0; i < count; i++)
        {
            DG_PlayerCharacters.PlayerCharacter PC = PlayerData.PlayerCharacters[i];
            //
            PC.Name = StringValues[Index]; Index++;
        }
    }







    //WorldObjects
    public List<int> GatherWorldInts(bool ToDisk)
    {
        List<int> IntData = new List<int>();
        //
        IntData.Add(QuickFind.SceneList.Scenes.Count);
        //
        for (int i = 0; i < QuickFind.SceneList.Scenes.Count; i++)
        {
            NetworkScene NS = QuickFind.SceneList.Scenes[i].SceneLink;
            //
            IntData.Add(NS.SceneID);
            if(!ToDisk) IntData.Add(NS.SceneOwnerID);
            IntData.Add(NS.NetworkObjectList.Count);
            //
            for (int iN = 0; iN < NS.NetworkObjectList.Count; iN++)
            {
                NetworkObject NO = NS.NetworkObjectList[iN];
                //
                IntData.Add(NO.NetworkObjectID);
                IntData.Add(NO.ItemRefID);
                IntData.Add(NO.PositionX);
                IntData.Add(NO.PositionY);
                IntData.Add(NO.PositionZ);
                IntData.Add(NO.YFacing);
                IntData.Add((int)NO.ObjectType);

                int HasHealth = NO.HasHealth ? 1 : 0; IntData.Add(HasHealth);
                if (HasHealth == 1) IntData.Add(NO.HealthValue);

                if (NO.ObjectType == NetworkObjectManager.NetworkObjectTypes.Item)
                {
                    IntData.Add(NO.ItemQualityLevel);

                    //Waterable
                    int isTrue = NO.isWaterable ? 1 : 0; IntData.Add(isTrue);
                    if (isTrue == 1)
                    {
                        IntData.Add(NO.SurrogateObjectIndex);
                        int InnerisTrue = NO.HasBeenWatered ? 1 : 0; IntData.Add(InnerisTrue);
                    }
                    //Health
                    if (HasHealth == 1)
                    {
                        IntData.Add(NO.GrowthValue);
                        IntData.Add(NO.ActiveVisual);
                    }
                    //Storage
                    isTrue = NO.isStorageContainer ? 1 : 0; IntData.Add(isTrue);
                    if (isTrue == 1)
                    {
                        DG_PlayerCharacters.RucksackSlot[] RS = NO.StorageSlots;
                        isTrue = NO.isTreasureList ? 1 : 0;
                        IntData.Add(isTrue);
                        IntData.Add(NO.StorageSlots.Length);

                        for (int iR = 0; iR < NO.StorageSlots.Length; iR++)
                        {
                            DG_PlayerCharacters.RucksackSlot RSlot = NO.StorageSlots[iR];
                            //
                            IntData.Add(RSlot.ContainedItem);
                            IntData.Add(RSlot.CurrentStackActive);
                            IntData.Add(RSlot.LowValue);
                            IntData.Add(RSlot.NormalValue);
                            IntData.Add(RSlot.HighValue);
                            IntData.Add(RSlot.MaximumValue);
                        }
                    }
                }
            }
        }
        if (ToDisk)
            SaveInts(IntData.ToArray(), SaveDirectory + "/WorldInts");

        if (PrintDebug) PrintFileSentSize(IntData.Count, true, ToDisk);


        return IntData;
    }
    public void GetWorldInts(int[] IntValues, bool FromDisk)
    {
        if (FromDisk)
            IntValues = LoadInts(SaveDirectory + "/WorldInts");

        if (PrintDebug) PrintFileSentSize(IntValues.Length, false, FromDisk);

        int Index = 0;
        int SceneCount = 0;

        SceneCount = IntValues[Index]; Index++;
        //
        for (int i = 0; i < SceneCount; i++)
        {
            int SceneID = IntValues[Index]; Index++;
            NetworkScene NS = QuickFind.NetworkObjectManager.GetSceneByID(SceneID);
            if(!FromDisk) NS.SceneOwnerID = IntValues[Index]; Index++;

            NS.JunkObject = new GameObject();
            Transform Child = NS.transform;
            int count;
            //
            count = IntValues[Index]; Index++;
            //
            for (int iN = 0; iN < count; iN++)
            {
                NetworkObject NO = NS.JunkObject.AddComponent<NetworkObject>();
                NS.TempNetworkObjectList.Add(NO);

                string ChildChild = iN.ToString();                
                NO.NetworkObjectID = IntValues[Index]; Index++;
                NO.ItemRefID = IntValues[Index]; Index++;
                NO.PositionX = IntValues[Index]; Index++;
                NO.PositionY = IntValues[Index]; Index++;
                NO.PositionZ = IntValues[Index]; Index++;
                NO.YFacing = IntValues[Index]; Index++;
                NO.ObjectType = (NetworkObjectManager.NetworkObjectTypes)IntValues[Index]; Index++;

                NO.transform.position = QuickFind.ConvertIntsToPosition(NO.PositionX, NO.PositionY, NO.PositionZ);
                NO.transform.eulerAngles = new Vector3(0, QuickFind.ConvertIntToFloat(NO.YFacing), 0);
                //


                int HasHealth = IntValues[Index]; Index++;
                bool ThisHasHealth = false; if (HasHealth == 1) ThisHasHealth = true; NO.HasHealth = ThisHasHealth;
                if (ThisHasHealth) { NO.HealthValue = IntValues[Index]; Index++; }



                if (NO.ObjectType == NetworkObjectManager.NetworkObjectTypes.Item)
                {
                    NO.ItemQualityLevel = IntValues[Index]; Index++;
                    int BoolValue;

                    //Waterable
                    BoolValue = IntValues[Index]; Index++;
                    bool isTrue = false; if (BoolValue == 1) isTrue = true; NO.isWaterable = isTrue;
                    if (isTrue)
                    {
                        NO.SurrogateObjectIndex = IntValues[Index]; Index++;
                        BoolValue = IntValues[Index]; Index++;
                        bool InnerisTrue = false; if (BoolValue == 1) InnerisTrue = true; NO.HasBeenWatered = InnerisTrue;
                    }

                    //Health
                    if (ThisHasHealth)
                    {            
                        NO.GrowthValue = IntValues[Index]; Index++;
                        NO.ActiveVisual = IntValues[Index]; Index++;
                    }

                    //Storage
                    BoolValue = IntValues[Index]; Index++;
                    isTrue = false; if (BoolValue == 1) isTrue = true; NO.isStorageContainer = isTrue;
                    if (isTrue)
                    {
                        int Count = 0;
                        BoolValue = IntValues[Index]; Index++;
                        isTrue = false; if (BoolValue == 1) isTrue = true; NO.isTreasureList = isTrue;
                        Count = IntValues[Index]; Index++;
                        NO.StorageSlots = new DG_PlayerCharacters.RucksackSlot[Count];

                        DG_PlayerCharacters.RucksackSlot[] RS = NO.StorageSlots;
                        for (int iR = 0; iR < Count; iR++)
                        {
                            RS[iR] = new DG_PlayerCharacters.RucksackSlot();
                            DG_PlayerCharacters.RucksackSlot RSlot = RS[iR];
                            //
                            RSlot.ContainedItem = IntValues[Index]; Index++;
                            RSlot.CurrentStackActive = IntValues[Index]; Index++;
                            RSlot.LowValue = IntValues[Index]; Index++;
                            RSlot.NormalValue = IntValues[Index]; Index++;
                            RSlot.HighValue = IntValues[Index]; Index++;
                            RSlot.MaximumValue = IntValues[Index]; Index++;
                        }
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

    void PrintFileSentSize(int Count, bool Sent, bool FromDisk)
    {
        int ByteSize = Count * 4;
        string Print = string.Empty;
        if (!Sent) Print += "Bytes Received";
        else Print += "Bytes Sent";
        Print += " - ";
        if (!FromDisk) Print += "From Master";
        else Print += "From Disk";
        Print += " - ";
        Print += ByteSize.ToString();
        Print += " - KB - ";
        Print += (ByteSize / 1000).ToString();

        Debug.Log(Print);
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
