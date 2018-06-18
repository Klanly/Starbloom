using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Sirenix.OdinInspector;

public class DG_MusicObject : MonoBehaviour {

    [HideInInspector] public int DatabaseID;
    [HideInInspector] public bool LockItem;
    public string Name;

    public AudioClip MusicFile;
    public DG_AudioManager.AudioFXZoneTypes[] AllowedFXZones;
    public float Volume;

    [Button()] public void TriggerMusic() { if (!Application.isPlaying) return; QuickFind.AudioManager.PlayMusic(DatabaseID, DG_AudioCrossFading.FadeSpeeds.Normal); }
}
