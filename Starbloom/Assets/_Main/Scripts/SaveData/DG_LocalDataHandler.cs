using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class DG_LocalDataHandler : MonoBehaviour {

    string CacheDirectory;

    private void Awake()
    {
        QuickFind.SaveHandler = this;
        CacheDirectory = Environment.CurrentDirectory + "\\Cache/";
    }


    ///////////////////////////////////////////////////////////////////////
    public void SaveBoolTracker(bool[] Data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(CacheDirectory + "SaveData/Bool");
        bf.Serialize(file, Data);
        file.Close();
    }
    public bool[] LoadBoolTracker()
    {
        bool[] ReturnArray = null;
        if (File.Exists(CacheDirectory + "SaveData/Bool"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(CacheDirectory + "SaveData/Bool", FileMode.Open);
            ReturnArray = (bool[])bf.Deserialize(file);
            file.Close();
        }
        return ReturnArray;
    }
    ///////////////////////////////////////////////////////////////////////
    public void SaveIntTracker(int[] Data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(CacheDirectory + "SaveData/Int");
        bf.Serialize(file, Data);
        file.Close();
    }
    public int[] LoadIntTracker()
    {
        int[] ReturnArray = null;
        if (File.Exists(CacheDirectory + "SaveData/Int"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(CacheDirectory + "SaveData/Int", FileMode.Open);
            ReturnArray = (int[])bf.Deserialize(file);
            file.Close();
        }
        return ReturnArray;
    }
    ///////////////////////////////////////////////////////////////////////
    public void SaveFloatTracker(float[] Data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(CacheDirectory + "SaveData/Float");
        bf.Serialize(file, Data);
        file.Close();
    }
    public float[] LoadFloatTracker()
    {
        float[] ReturnArray = null;
        if (File.Exists(CacheDirectory + "SaveData/Float"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(CacheDirectory + "SaveData/Float", FileMode.Open);
            ReturnArray = (float[])bf.Deserialize(file);
            file.Close();
        }
        return ReturnArray;
    }
    ///////////////////////////////////////////////////////////////////////
    public void SaveStringTracker(string[] Data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(CacheDirectory + "SaveData/String");
        bf.Serialize(file, Data);
        file.Close();
    }
    public string[] LoadStringTracker()
    {
        string[] ReturnArray = null;
        if (File.Exists(CacheDirectory + "SaveData/String"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(CacheDirectory + "SaveData/String", FileMode.Open);
            ReturnArray = (string[])bf.Deserialize(file);
            file.Close();
        }
        return ReturnArray;
    }



    ///////////////////////////////////////////////////////////////////////
    public void SaveControls(string[] Data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(CacheDirectory + "SaveData/Controls");
        bf.Serialize(file, Data);
        file.Close();
    }
    public string[] LoadControls()
    {
        string[] ReturnArray = null;
        if (File.Exists(CacheDirectory + "SaveData/Controls"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(CacheDirectory  + "SaveData/Controls", FileMode.Open);
            ReturnArray = (string[])bf.Deserialize(file);
            file.Close();
        }
        return ReturnArray;
    }
}
