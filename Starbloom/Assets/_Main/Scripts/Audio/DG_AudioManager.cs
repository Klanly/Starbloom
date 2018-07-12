using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Audio;

public class DG_AudioManager : MonoBehaviour {



    public enum AudioFXZoneTypes
    {
        Normal,
        Underwater,
        Cave
    }
    [System.Serializable]
    public class AudioFX
    {
        public AudioFXZoneTypes FXType;
        public AudioMixerGroup MixerGroup;
    }

    public AudioMixerGroup MasterMixerGroup;

    public AudioFXZoneTypes CurrentAudioFXZone;
    [Header("References")]
    public DG_AudioCrossFading MusicHandler;
    public DG_SFXHandler SFXHandler;




    private void Awake() { QuickFind.AudioManager = this; }


    private void Update()
    {
        if (QuickFind.UnderwaterTrigger.isUnderwater) SetAudioZone(AudioFXZoneTypes.Underwater);
        else SetAudioZone(AudioFXZoneTypes.Normal);
    }

    public void SetAudioZone(AudioFXZoneTypes NewZone)
    {
        if (CurrentAudioFXZone == NewZone) return;
        CurrentAudioFXZone = NewZone;

        if(MusicHandler.MusicIsCurrentlyPlaying())
            PlayMusic(MusicHandler.CurrentlyPlayingID(), DG_AudioCrossFading.FadeSpeeds.Normal, true);
    }


    //MUSIC
    public void PlayMusic(int ID, DG_AudioCrossFading.FadeSpeeds FadeSpeed, bool SyncTime = false)
    {
        MusicHandler.TriggerMusic(ID, FadeSpeed, SyncTime);
    }
    public void StopMusic(DG_AudioCrossFading.FadeSpeeds FadeOutSpeed)
    {
        MusicHandler.StopMusic(FadeOutSpeed);
    }



    //SFX One shot
    public void PlaySoundEffect(int ID, Vector3 Position)
    {
        SFXHandler.TriggerRegularSoundEffect(ID, Position);
    }



    //SFX Looping
    public void StartLoopingSoundEffect()
    {

    }
    public void StopLoopingSoundEffect()
    {

    }
}
