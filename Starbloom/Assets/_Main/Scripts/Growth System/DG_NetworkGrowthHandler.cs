using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_NetworkGrowthHandler : MonoBehaviour
{


    private void Awake()
    {
        QuickFind.NetworkGrowthHandler = this;
    }


    public void CheckDayChangedHasBeenWatered()
    {
        WeatherHandler.WeatherTyps CurrentWeather = QuickFind.WeatherHandler.CurrentWeather;

        for (int i = 0; i < QuickFind.NetworkObjectManager.transform.childCount; i++)
        {
            NetworkScene Scene = QuickFind.NetworkObjectManager.transform.GetChild(i).GetComponent<NetworkScene>();
            for (int iN = 0; iN < Scene.NetworkObjectList.Count; iN++)
            {
                NetworkObject NO = Scene.NetworkObjectList[iN];
                DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(NO.ItemRefID);

                if (IO.isGrowableItem)
                {
                    if (IO.GrowthStages != null && IO.GrowthStages.Length != 0)
                    {
                        AddGrowthValue(NO, IO);
                        CheckGrowthVisual(NO, IO);
                        SetActiveVisual(NO);
                    }
                    if (!IO.IsWaterable) continue;
                    if (CurrentWeather == WeatherHandler.WeatherTyps.Raining || CurrentWeather == WeatherHandler.WeatherTyps.Thunderstorm) NO.HasBeenWatered = true;
                    else NO.HasBeenWatered = false;
                    QuickFind.WateringSystem.AdjustWateredObjectVisual(NO, NO.HasBeenWatered);
                }
            }
        }
    }



    public void SetBroken(NetworkObject NO, DG_ItemObject IO, int Value)
    {
        NO.GrowthValue = Value;
    }

    void AddGrowthValue(NetworkObject NO, DG_ItemObject IO)
    {
        if (NO.GrowthValue == -1) return;  //Broken Item

        bool AddValue = true;
        bool Watered = NO.HasBeenWatered;
        if (IO.IsWaterable && !Watered) AddValue = false;

        if (AddValue) NO.GrowthValue++;
    }
    void CheckGrowthVisual(NetworkObject NO, DG_ItemObject IO)
    {
        for (int i = 0; i < IO.GrowthStages.Length; i++)
        {
            DG_ItemObject.GrowthStage GS = IO.GrowthStages[i];
            if (NO.GrowthValue == GS.GrowthLevelRequired)
                NO.ActiveVisual = i;
        }
    }
    public void SetActiveVisual(NetworkObject NO, bool DestroyOld = true)
    {
        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(NO.ItemRefID);

        bool ShowBrokenItem = false;
        int SpawnID = NO.ActiveVisual;
        if (NO.GrowthValue == -1) ShowBrokenItem = true;

        if(ShowBrokenItem) SpawnID = IO.BreakIndex;

        DG_ItemObject.GrowthStage GS = IO.GrowthStages[SpawnID];

        GameObject Spawn = Instantiate(GS.StagePrefabReference);
        Transform T = Spawn.transform;
        T.SetParent(NO.transform);
        T.localPosition = Vector3.zero;
        T.localEulerAngles = Vector3.zero;
        float Scale = IO.DefaultScale;
        T.localScale = new Vector3(Scale, Scale, Scale);

        if (ShowBrokenItem) Spawn.GetComponent<DG_BreakObjectLoadSet>().LoadBrokenType();

        if (DestroyOld)
        {
            GameObject DestroyObject = NO.transform.GetChild(0).gameObject;
            Destroy(DestroyObject);
        }
    }


    public void Harvest(NetworkObject NO, DG_ItemObject IO, int Scene)
    {
        int[] OutData = new int[2];
        OutData[0] = Scene;
        OutData[1] = NO.NetworkObjectID;

        QuickFind.NetworkSync.HarvestObject(OutData);
    }
    public void ReceivedPlantHarvested(int[] Data)
    {
        NetworkScene Scene = QuickFind.NetworkObjectManager.transform.GetChild(Data[0]).GetComponent<NetworkScene>();
        NetworkObject NO = Scene.GetObjectByID(Data[1]);
        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(NO.ItemRefID);

        NO.ActiveVisual = IO.ResetIndex;
        if (NO.GrowthValue != -1)
        {
            NO.GrowthValue = IO.GrowthStages[NO.ActiveVisual].GrowthLevelRequired;
            CheckGrowthVisual(NO, IO);
        }
        SetActiveVisual(NO);
    }


    public int GetCurrentVisualByGrowthValue(NetworkObject NO)
    {
        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(NO.ItemRefID);
        int KnownIndex = 0;
        for (int i = 0; i < IO.GrowthStages.Length; i++)
        {
            DG_ItemObject.GrowthStage GS = IO.GrowthStages[i];
            if (NO.GrowthValue >= GS.GrowthLevelRequired)
                KnownIndex = i;
            else
                break;
        }
        return KnownIndex;
    }
}
