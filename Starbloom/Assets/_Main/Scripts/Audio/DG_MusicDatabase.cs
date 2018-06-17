using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class DG_MusicDatabase : MonoBehaviour {

    [HideInInspector]
    public DG_MusicObject[] ItemCatagoryList;
    [HideInInspector]
    public int ListCount;


    public DG_AudioManager.AudioFX[] MusicEffectZoneBusList;


    private void Awake()
    {
        QuickFind.MusicDatabase = this;
    }


    public DG_MusicObject GetItemFromID(int ID)
    {
        if (ID < 0) { Debug.Log("Get By ID Failed"); return null; }
        if (ID >= ItemCatagoryList.Length) { Debug.Log("Get By ID Failed"); return null; }

        return ItemCatagoryList[ID];
    }

    public AudioMixerGroup GetMixerGroupByZone(DG_MusicObject MusicObject)
    {
        DG_AudioManager.AudioFXZoneTypes CurrentAudioFXZone = QuickFind.AudioManager.CurrentAudioFXZone;
        DG_AudioManager.AudioFXZoneTypes ReturnType = DG_AudioManager.AudioFXZoneTypes.Normal;
        for (int i = 0; i < MusicObject.AllowedFXZones.Length; i++)
        {
            if (MusicObject.AllowedFXZones[i] == CurrentAudioFXZone)
            {
                ReturnType = MusicObject.AllowedFXZones[i];
                break;
            }
        }
        for(int i = 0; i < MusicEffectZoneBusList.Length; i++)
        {
            if (MusicEffectZoneBusList[i].FXType == ReturnType)
                return MusicEffectZoneBusList[i].MixerGroup;
        }

        Debug.Log("Default bus was not set correctly in Music Bus List");
        return null;
    }
}
