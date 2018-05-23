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


    private void Awake()
    {
        QuickFind.SaveHandler = this;
        CacheDirectory = Environment.CurrentDirectory + "\\SaveFiles/";
    }









    [Button(ButtonSizes.Medium)]
    public void SaveGame()
    {
        SaveDirectory = FindOrCreateSaveDirectory(CacheDirectory, SaveFileName);
    }
    [Button(ButtonSizes.Medium)]
    public void LoadGame()
    {
        SaveDirectory = FindOrCreateSaveDirectory(CacheDirectory, SaveFileName);
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
        if (ToDisk) { SaveInts(IntData.ToArray(), FindOrCreateSaveDirectory(SaveDirectory, "SharedInts")); IntData.Clear(); }
        //
        for (int i = 0; i < PlayerData.PlayerCharacters.Count; i++)
        {
            DG_PlayerCharacters.PlayerCharacter PC = PlayerData.PlayerCharacters[i];
            DG_PlayerCharacters.CharacterEquipment CE = PC.Equipment;
            string PlayerNum = i.ToString();
            //
            IntData.Add(CE.HatId);
            IntData.Add(CE.Ring1);
            IntData.Add(CE.Ring2);
            IntData.Add(CE.Boots);
            if (ToDisk) { SaveInts(IntData.ToArray(), FindOrCreateSaveDirectory(SaveDirectory, "PlayerEquipment" + PlayerNum)); IntData.Clear(); }
            //
            IntData.Add(CE.RuckSackUnlockedSize);
            DG_PlayerCharacters.RucksackSlot[] RS = CE.RucksackSlots;
            for (int iN = 0; iN < CE.RuckSackUnlockedSize; iN++)
            {
                DG_PlayerCharacters.RucksackSlot RSlot = RS[iN];
                IntData.Add(RSlot.ContainedItem);
                IntData.Add(RSlot.CurrentStackActive);
                IntData.Add(RSlot.LowValue);
                IntData.Add(RSlot.NormalValue);
                IntData.Add(RSlot.HighValue);
                IntData.Add(RSlot.MaximumValue);
            }
            if (ToDisk) { SaveInts(IntData.ToArray(), FindOrCreateSaveDirectory(SaveDirectory, "PlayerRucksack" + PlayerNum)); IntData.Clear(); }
            //
        }

        return IntData;
    }
    public List<string> GatherPlayerDataStrings(bool ToDisk)
    {
        int Index = 0;
        List<string> StringData = new List<string>();
        DG_PlayerCharacters PlayerData = QuickFind.Farm;
        //
        StringData.Add(PlayerData.FarmName);
        StringData.Add(PlayerData.PlayerCharacters.Count.ToString());
        if (ToDisk) { SaveStrings(StringData.ToArray(), FindOrCreateSaveDirectory(SaveDirectory, "SharedStrings")); StringData.Clear(); }
        //
        for (int i = 0; i < PlayerData.PlayerCharacters.Count; i++)
        {
            DG_PlayerCharacters.PlayerCharacter PC = PlayerData.PlayerCharacters[i];
            string PlayerNum = i.ToString();
            //
            StringData.Add(PC.Name);
            if (ToDisk) { SaveStrings(StringData.ToArray(), FindOrCreateSaveDirectory(SaveDirectory, "PlayerStrings" + PlayerNum)); StringData.Clear(); }
        }
        return StringData;
    }





    public void GetIntValues(int[] IntValues, bool FromDisk)
    {
        int Index = 0;
        int PlayerCount = 0;
        DG_PlayerCharacters PlayerData = QuickFind.Farm;
        //
        if (FromDisk) IntValues = LoadInts(FindOrCreateSaveDirectory(SaveDirectory, "SharedInts"));
        if (!FromDisk) PlayerCount = IntValues[Index];                      else PlayerCount = VersionChangeSafeGetValue(IntValues, Index); Index++;
        if (!FromDisk) PlayerData.SharedMoney = IntValues[Index];           else PlayerData.SharedMoney = VersionChangeSafeGetValue(IntValues, Index); Index++;
        if (!FromDisk) PlayerData.Year = IntValues[Index];                  else PlayerData.Year = VersionChangeSafeGetValue(IntValues, Index); Index++;
        if (!FromDisk) PlayerData.Month = IntValues[Index];                 else PlayerData.Month = VersionChangeSafeGetValue(IntValues, Index); Index++;
        if (!FromDisk) PlayerData.Day = IntValues[Index];                   else PlayerData.Day = VersionChangeSafeGetValue(IntValues, Index); Index++;
        //
        for (int i = 0; i < PlayerCount; i++)
        {
            DG_PlayerCharacters.PlayerCharacter PC = PlayerData.PlayerCharacters[i];
            DG_PlayerCharacters.CharacterEquipment CE = PC.Equipment;
            string PlayerNum = i.ToString();
            //
            if (FromDisk) { IntValues = LoadInts(FindOrCreateSaveDirectory(SaveDirectory, "PlayerEquipment" + PlayerNum)); Index = 0; };
            if (!FromDisk) CE.HatId = IntValues[Index];                     else CE.HatId = VersionChangeSafeGetValue(IntValues, Index); Index++;
            if (!FromDisk) CE.Ring1 = IntValues[Index];                     else CE.Ring1 = VersionChangeSafeGetValue(IntValues, Index); Index++;
            if (!FromDisk) CE.Ring2 = IntValues[Index];                     else CE.Ring2 = VersionChangeSafeGetValue(IntValues, Index); Index++;
            if (!FromDisk) CE.Boots = IntValues[Index];                     else CE.Boots = VersionChangeSafeGetValue(IntValues, Index); Index++;              
            //
            if (FromDisk) { IntValues = LoadInts(FindOrCreateSaveDirectory(SaveDirectory, "PlayerRucksack" + PlayerNum)); Index = 0; };
            if (!FromDisk) CE.RuckSackUnlockedSize = IntValues[Index];      else CE.RuckSackUnlockedSize = VersionChangeSafeGetValue(IntValues, Index); Index++;
            //
            DG_PlayerCharacters.RucksackSlot[] RS = CE.RucksackSlots;
            for (int iN = 0; iN < CE.RuckSackUnlockedSize; iN++)
            {
                DG_PlayerCharacters.RucksackSlot RSlot = RS[iN];
                //
                if (!FromDisk) RSlot.ContainedItem = IntValues[Index];      else RSlot.ContainedItem = VersionChangeSafeGetValue(IntValues, Index); Index++;
                if (!FromDisk) RSlot.CurrentStackActive = IntValues[Index]; else RSlot.CurrentStackActive = VersionChangeSafeGetValue(IntValues, Index); Index++;
                if (!FromDisk) RSlot.LowValue = IntValues[Index];           else RSlot.LowValue = VersionChangeSafeGetValue(IntValues, Index); Index++;
                if (!FromDisk) RSlot.NormalValue = IntValues[Index];        else RSlot.NormalValue = VersionChangeSafeGetValue(IntValues, Index); Index++;
                if (!FromDisk) RSlot.HighValue = IntValues[Index];          else RSlot.HighValue = VersionChangeSafeGetValue(IntValues, Index); Index++;
                if (!FromDisk) RSlot.MaximumValue = IntValues[Index];       else RSlot.MaximumValue = VersionChangeSafeGetValue(IntValues, Index); Index++;
            }
        }
    }

    public void GetStringValues(string[] StringValues, bool FromDisk)
    {
        int Index = 0;
        string PlayerCount;
        DG_PlayerCharacters PlayerData = QuickFind.Farm;
        //
        if (FromDisk) StringValues = LoadStrings(FindOrCreateSaveDirectory(SaveDirectory, "SharedStrings"));
        if (!FromDisk) PlayerData.FarmName = StringValues[Index];           else PlayerData.FarmName = VersionChangeSafeGetString(StringValues, Index); Index++;
        if (!FromDisk) PlayerCount = StringValues[Index];                   else PlayerCount = VersionChangeSafeGetString(StringValues, Index); Index++;
        //
        int count = int.Parse(PlayerCount);
        for (int i = 0; i < count; i++)
        {
            DG_PlayerCharacters.PlayerCharacter PC = PlayerData.PlayerCharacters[i];
            string PlayerNum = i.ToString();
            //
            if (FromDisk) { StringValues = LoadStrings(FindOrCreateSaveDirectory(SaveDirectory, "PlayerStrings" + PlayerNum)); Index = 0; };
            if (!FromDisk) PC.Name = StringValues[Index];                   else PC.Name = VersionChangeSafeGetString(StringValues, Index); Index++;
        }
    }







    //Util///////////////////////////////
    public bool CheckIfOverwritingPreviousFile(string FileName)
    {
        string SaveFileBaseLocation = CacheDirectory + "/" + FileName;
        if (Directory.Exists(SaveFileBaseLocation))
            return true;
        else
            return false;
    }
    public string FindOrCreateSaveDirectory(string KnownDirectory, string SubFolder)
    {
        string SaveFileBaseLocation = KnownDirectory + "/" + SubFolder;
        if (!Directory.Exists(SaveFileBaseLocation))
            Directory.CreateDirectory(SaveFileBaseLocation);
        return SaveFileBaseLocation;
    }
    int VersionChangeSafeGetValue(int[] Array, int index)
    {
        if (index >= Array.Length) return 0;
        else return Array[index];
    }
    string VersionChangeSafeGetString(string[] Array, int index)
    {
        if (index >= Array.Length) return "";
        else return Array[index];
    }





    ///////////////////////////////////////////////////////////////////////
    public void SaveBools(bool[] Data, string FileName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(FileName);
        bf.Serialize(file, Data);
        file.Close();
    }
    public bool[] LoadBools(string FileName)
    {
        bool[] ReturnArray = null;
        if (File.Exists(FileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(FileName, FileMode.Open);
            ReturnArray = (bool[])bf.Deserialize(file);
            file.Close();
        }
        return ReturnArray;
    }
    ///////////////////////////////////////////////////////////////////////
    public void SaveInts(int[] Data, string FileName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(FileName);
        bf.Serialize(file, Data);
        file.Close();
    }
    public int[] LoadInts(string FileName)
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
    public void SaveFloats(float[] Data, string FileName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(FileName);
        bf.Serialize(file, Data);
        file.Close();
    }
    public float[] LoadFloats(string FileName)
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
    public void SaveStrings(string[] Data, string FileName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(FileName);
        bf.Serialize(file, Data);
        file.Close();
    }
    public string[] LoadStrings(string FileName)
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
}
