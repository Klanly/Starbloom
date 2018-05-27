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
        GatherWorldFloats(true);

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
        GetWorldFloats(null, true);
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
            //
            if (ToDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "PlayerEquipment" + PlayerNum.ToString() + "/");
            if (!ToDisk) IntData.Add(CE.HatId); else SaveInt(CE.HatId, Directory + "HatId");
            if (!ToDisk) IntData.Add(CE.Ring1); else SaveInt(CE.Ring1, Directory + "Ring1");
            if (!ToDisk) IntData.Add(CE.Ring2); else SaveInt(CE.Ring2, Directory + "Ring2");
            if (!ToDisk) IntData.Add(CE.Boots); else SaveInt(CE.Boots, Directory + "Boots");
            //
            if (ToDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "Rucksack" + PlayerNum.ToString() + "/");
            if (!ToDisk) IntData.Add(CE.RuckSackUnlockedSize); else SaveInt(CE.RuckSackUnlockedSize, Directory + "RuckSackUnlockedSize");
            DG_PlayerCharacters.RucksackSlot[] RS = CE.RucksackSlots;
            string KnownDirectory = Directory;
            for (int iN = 0; iN < CE.RuckSackUnlockedSize; iN++)
            {
                DG_PlayerCharacters.RucksackSlot RSlot = RS[iN];
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
            //
            if (FromDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "PlayerEquipment" + PlayerNum.ToString() + "/");
            if (!FromDisk) CE.HatId = IntValues[Index]; else CE.HatId = LoadInt(Directory + "HatId"); Index++;
            if (!FromDisk) CE.Ring1 = IntValues[Index]; else CE.Ring1 = LoadInt(Directory + "Ring1"); Index++;
            if (!FromDisk) CE.Ring2 = IntValues[Index]; else CE.Ring2 = LoadInt(Directory + "Ring2"); Index++;
            if (!FromDisk) CE.Boots = IntValues[Index]; else CE.Boots = LoadInt(Directory + "Boots"); Index++;
            //
            if (FromDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "Rucksack" + PlayerNum.ToString() + "/");
            if (!FromDisk) CE.RuckSackUnlockedSize = IntValues[Index]; else LoadInt(Directory + "RuckSackUnlockedSize"); Index++;
            //
            DG_PlayerCharacters.RucksackSlot[] RS = CE.RucksackSlots;
            string KnownDirectory = Directory;
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
                if (!ToDisk) IntData.Add(NO.ItemGrowthLevel); else SaveInt(NO.ItemGrowthLevel, Directory + "ItemGrowthLevel");



                int isTrue = NO.isStorageContainer ? 1 : 0;
                if (!ToDisk) IntData.Add(isTrue); else SaveInt(isTrue, Directory + "IsStorageContainer");
                if (isTrue == 1)
                {
                    DG_PlayerCharacters.RucksackSlot[] RS = NO.StorageSlots;
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
                if (!FromDisk) NO.ItemGrowthLevel = IntValues[Index]; else NO.ItemGrowthLevel = LoadInt(Directory + "ItemGrowthLevel"); Index++;
                //

                int StorageValue;
                if (!FromDisk) StorageValue = IntValues[Index]; else StorageValue = LoadInt(Directory + "IsStorageContainer"); Index++;
                bool isTrue = false;
                if (StorageValue == 1) isTrue = true;
                NO.isStorageContainer = isTrue;
                if (isTrue)
                {
                    int Count = 0;
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

    public List<float> GatherWorldFloats(bool ToDisk)
    {
        string Directory = "";
        List<float> OutgoingFloats = new List<float>();
        Transform NOM = QuickFind.NetworkObjectManager.transform;
        if (ToDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "WorldFloatData") + "/";
        if (!ToDisk) OutgoingFloats.Add(NOM.childCount); else SaveFloat(NOM.childCount, Directory + "Count");
        //
        for (int i = 0; i < NOM.childCount; i++)
        {
            Transform Child = NOM.GetChild(i);
            NetworkScene NS = Child.GetComponent<NetworkScene>();
            if (ToDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "WorldFloatDataScene") + "/";
            if (!ToDisk) OutgoingFloats.Add(NS.SceneID); else SaveFloat(NS.SceneID, Directory + "SceneID");
            if (!ToDisk) OutgoingFloats.Add(Child.childCount); else SaveFloat(Child.childCount, Directory + "ChildCount");
            string KnownDirectory = Directory;
            for (int iN = 0; iN < Child.childCount; iN++)
            {
                NetworkObject NO = Child.GetChild(iN).GetComponent<NetworkObject>();
                string ChildChild = iN.ToString();
                //
                if (ToDisk) Directory = FindOrCreateSaveDirectory(KnownDirectory, "CC" + ChildChild + "/");
                if (!ToDisk) OutgoingFloats.Add(NO.Position.x); else SaveFloat(NO.Position.x, Directory + "x");
                if (!ToDisk) OutgoingFloats.Add(NO.Position.y); else SaveFloat(NO.Position.y, Directory + "y");
                if (!ToDisk) OutgoingFloats.Add(NO.Position.z); else SaveFloat(NO.Position.z, Directory + "z");
                if (!ToDisk) OutgoingFloats.Add(NO.YFacing); else SaveFloat(NO.YFacing, Directory + "YFacing");
            }
        }
        return OutgoingFloats;
    }
    public void GetWorldFloats(float[] FloatValues, bool FromDisk)
    {
        string Directory = "";
        Transform NOM = QuickFind.NetworkObjectManager.transform;
        int Index = 0;
        int SceneCount = 0;
        if (FromDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "WorldFloatData") + "/";
        if (!FromDisk) SceneCount = (int)FloatValues[Index]; else SceneCount = (int)LoadFloat(Directory + "Count"); Index++;
        for (int i = 0; i < NOM.childCount; i++)
        {
            int SceneID;
            if (FromDisk) Directory = FindOrCreateSaveDirectory(SaveDirectory, "WorldFloatDataScene") + "/";
            if (!FromDisk) SceneID = (int)FloatValues[Index]; else SceneID = (int)LoadFloat(Directory + "SceneID"); Index++;
            //
            NetworkScene NS = QuickFind.NetworkObjectManager.GetSceneByID(SceneID);
            Transform Child = NS.transform;
            int count;
            //
            if (!FromDisk) count = (int)FloatValues[Index]; else count = (int)LoadFloat(Directory + "ChildCount"); Index++;
            //
            string KnownDirectory = Directory;
            for (int iN = 0; iN < count; iN++)
            {
                NetworkObject NO = NS.NetworkObjectList[iN];
                string ChildChild = iN.ToString();
                //
                if (FromDisk) Directory = FindOrCreateSaveDirectory(KnownDirectory, "CC" + ChildChild + "/");

                float x = 0; if (!FromDisk) x = FloatValues[Index]; else x = LoadFloat(Directory + "x"); Index++;
                float y = 0; if (!FromDisk) y = FloatValues[Index]; else y = LoadFloat(Directory + "y"); Index++;
                float z = 0; if (!FromDisk) z = FloatValues[Index]; else z = LoadFloat(Directory + "z"); Index++;
                NO.Position = new Vector3(x, y, z);
                if (!FromDisk) NO.YFacing = FloatValues[Index]; else NO.YFacing = LoadFloat(Directory + "YFacing"); Index++;

                NO.transform.position = NO.Position;
                NO.transform.eulerAngles = new Vector3(0, NO.YFacing, 0);
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
