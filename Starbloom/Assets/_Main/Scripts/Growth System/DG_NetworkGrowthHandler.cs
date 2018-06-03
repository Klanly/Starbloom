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
            Transform Scene = QuickFind.NetworkObjectManager.transform.GetChild(i);
            for (int iN = 0; iN < Scene.childCount; iN++)
            {
                NetworkObject NO = Scene.GetChild(iN).GetComponent<NetworkObject>();
                if (!NO.isWaterable) continue;

                DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(NO.ItemRefID);
                if (IO.isItem)
                {
                    if (!NO.HasBeenWatered) NO.transform.GetChild(0).GetComponent<Growable>().GrowthOffset--; ;
                }

                if (CurrentWeather == WeatherHandler.WeatherTyps.Raining || CurrentWeather == WeatherHandler.WeatherTyps.Thunderstorm)
                    NO.HasBeenWatered = true;
                else
                    NO.HasBeenWatered = false;

                QuickFind.WateringSystem.AdjustWateredObjectVisual(NO, NO.HasBeenWatered);
            }
        }
    }
}
