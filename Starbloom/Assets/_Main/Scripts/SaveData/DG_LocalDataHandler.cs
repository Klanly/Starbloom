using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class DG_LocalDataHandler : MonoBehaviour {


    public string SaveFileName;
    string CacheDirectory;

    private void Awake()
    {
        QuickFind.SaveHandler = this;
        CacheDirectory = Environment.CurrentDirectory + "\\SaveFiles/";
    }


    ///////////////////////////////////////////////////////////////////////
    public void SaveBools(bool[] Data, string Catagory)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(CacheDirectory + SaveFileName + Catagory);
        bf.Serialize(file, Data);
        file.Close();
    }
    public bool[] LoadBools(string Catagory)
    {
        bool[] ReturnArray = null;
        if (File.Exists(CacheDirectory + Catagory))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(CacheDirectory + SaveFileName + Catagory, FileMode.Open);
            ReturnArray = (bool[])bf.Deserialize(file);
            file.Close();
        }
        return ReturnArray;
    }
    ///////////////////////////////////////////////////////////////////////
    public void SaveInts(int[] Data, string Catagory)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(CacheDirectory + SaveFileName + Catagory);
        bf.Serialize(file, Data);
        file.Close();
    }
    public int[] LoadInts(string Catagory)
    {
        int[] ReturnArray = null;
        if (File.Exists(CacheDirectory + "SaveData/Int"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(CacheDirectory + SaveFileName + Catagory, FileMode.Open);
            ReturnArray = (int[])bf.Deserialize(file);
            file.Close();
        }
        return ReturnArray;
    }
    ///////////////////////////////////////////////////////////////////////
    public void SaveFloats(float[] Data, string Catagory)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(CacheDirectory + SaveFileName + Catagory);
        bf.Serialize(file, Data);
        file.Close();
    }
    public float[] LoadFloats(string Catagory)
    {
        float[] ReturnArray = null;
        if (File.Exists(CacheDirectory + Catagory))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(CacheDirectory + SaveFileName + Catagory, FileMode.Open);
            ReturnArray = (float[])bf.Deserialize(file);
            file.Close();
        }
        return ReturnArray;
    }
    ///////////////////////////////////////////////////////////////////////
    public void SaveStrings(string[] Data, string Catagory)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(CacheDirectory + SaveFileName + Catagory);
        bf.Serialize(file, Data);
        file.Close();
    }
    public string[] LoadStrings(string Catagory)
    {
        string[] ReturnArray = null;
        if (File.Exists(CacheDirectory + Catagory))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(CacheDirectory + SaveFileName + Catagory, FileMode.Open);
            ReturnArray = (string[])bf.Deserialize(file);
            file.Close();
        }
        return ReturnArray;
    }
}
