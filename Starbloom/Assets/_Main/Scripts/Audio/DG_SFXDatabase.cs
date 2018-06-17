using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class DG_SFXDatabase : MonoBehaviour {

    [HideInInspector]
    public DG_SFXObject[] ItemCatagoryList;
    [HideInInspector]
    public int ListCount;


    public DG_AudioManager.AudioFX[] SFXEffectZoneBusList;



    private void Awake()
    {
        QuickFind.SFXDatabase = this;
    }


    public DG_SFXObject GetItemFromID(int ID)
    {
        if (ID < 0) { Debug.Log("Get By ID Failed"); return null; }
        if (ID >= ItemCatagoryList.Length) { Debug.Log("Get By ID Failed"); return null; }

        return ItemCatagoryList[ID];
    }

    public AudioMixerGroup GetMixerGroupByZone(DG_SFXObject SFXObject)
    {
        DG_AudioManager.AudioFXZoneTypes CurrentAudioFXZone = QuickFind.AudioManager.CurrentAudioFXZone;
        DG_AudioManager.AudioFXZoneTypes ReturnType = DG_AudioManager.AudioFXZoneTypes.Normal;
        for (int i = 0; i < SFXObject.AllowedFXZones.Length; i++)
        {
            if (SFXObject.AllowedFXZones[i] == CurrentAudioFXZone)
            {
                ReturnType = SFXObject.AllowedFXZones[i];
                break;
            }
        }
        for (int i = 0; i < SFXEffectZoneBusList.Length; i++)
        {
            if (SFXEffectZoneBusList[i].FXType == ReturnType)
                return SFXEffectZoneBusList[i].MixerGroup;
        }

        Debug.Log("Default bus was not set correctly in Music Bus List");
        return null;
    }
}
