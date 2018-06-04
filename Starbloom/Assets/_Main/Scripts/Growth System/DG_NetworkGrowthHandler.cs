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

                if (IO.isGrowableItem || IO.isEnvironment)
                {
                    if (IO.isGrowableItem)
                        NO.GrowthValue++;

                    //Trees, and Rocks, and anything that doesn't require Watering.
                    if (IO.EnvironmentValues[0] == null) continue;
                    if (!IO.EnvironmentValues[0].IsWaterable) continue;


                    if (IO.isGrowableItem && !NO.HasBeenWatered)
                    {
                        NO.GrowthValue--;
                        Growable Grow = NO.transform.GetChild(0).GetComponent<Growable>();
                        if (Grow != null) Grow.GrowthOffset--;
                        else
                            Debug.Log("FuckedUpshit");
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
}
