using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_AudioCrossFading : MonoBehaviour
{

    public enum FadeSpeeds
    {
        Instant,
        Quick,
        Normal,
        Slow,
        ReallySlow
    }


    [System.Serializable]
    public class Fades
    {
        public FadeSpeeds FadeSpeedType;
        public float FadeTime;
    }


    [System.Serializable]
    public class MusicObject
    {
        public AudioSource AudioSource;
        [HideInInspector] public int MusicID;
        [HideInInspector] public bool IsPlaying = false;
        [HideInInspector] public bool IsFading = false;
        [HideInInspector] public bool IsFadingOut = false;
        [HideInInspector] public float FadeTime;
        [HideInInspector] public float FadeTimer;
        [HideInInspector] public float FinalVolume;
    }

    public MusicObject Source1;
    public MusicObject Source2;
    bool Activeis1 = false;

    [Header("Fade Data")]
    public Fades[] FadeData;




    private void Awake()
    {
        this.enabled = false;
    }

    private void Update()
    {
        float DeltaTime = Time.deltaTime;
        bool FadeAComplete = FadeComplete(Source1, DeltaTime);
        bool FadeBComplete = FadeComplete(Source2, DeltaTime);
        if (FadeAComplete && FadeBComplete) this.enabled = false;
    }





    bool FadeComplete(MusicObject Source, float DeltaTime)
    {
        bool FadeComplete = true;

        if (Source.IsFading)
        {
            float Volume;
            Source.FadeTimer -= DeltaTime;
            if (Source.FadeTimer < 0) { Source.IsFading = false; if (Source.IsFadingOut) { Volume = 0; Source.IsPlaying = false; } else Volume = Source.FinalVolume; }
            else
            {
                FadeComplete = false;
                float PercentageDiff = Source.FadeTimer / Source.FadeTime;
                if (!Source.IsFadingOut) PercentageDiff = 1 - PercentageDiff;
                Volume = Source.FinalVolume * PercentageDiff;
            }
            Source.AudioSource.volume = Volume;
        }

        return FadeComplete;
    }





    public void TriggerMusic(int ID, FadeSpeeds FadeSpeed, bool SyncTimeToCurrentlyPlaying = false)
    {
        DG_MusicObject MO = QuickFind.MusicDatabase.GetItemFromID(ID);
        float FadeTime = GetFadeSpeed(FadeSpeed);

        if(Activeis1) { Activeis1 = false; SetupCrossFade(Source2, Source1, MO, FadeTime, SyncTimeToCurrentlyPlaying, ID); }
                else { Activeis1 = true; SetupCrossFade(Source1, Source2, MO, FadeTime, SyncTimeToCurrentlyPlaying, ID); }
    }
    void SetupCrossFade(MusicObject CurrentActive, MusicObject PreviousActive, DG_MusicObject MO, float FadeTime, bool SyncTimeToCurrentlyPlaying, int ID)
    {
        if (PreviousActive.IsPlaying)
        {
            PreviousActive.IsFading = true;
            PreviousActive.IsFadingOut = true;
            PreviousActive.FadeTime = FadeTime;
            PreviousActive.FadeTimer = FadeTime;
        }

        CurrentActive.MusicID = ID;

        CurrentActive.IsPlaying = true;
        CurrentActive.IsFading = true;
        CurrentActive.IsFadingOut = false;
        CurrentActive.FadeTime = FadeTime;
        CurrentActive.FadeTimer = FadeTime;

        CurrentActive.FinalVolume = MO.Volume;
        CurrentActive.AudioSource.clip = MO.MusicFile;
        CurrentActive.AudioSource.outputAudioMixerGroup = QuickFind.MusicDatabase.GetMixerGroupByZone(MO);
        CurrentActive.AudioSource.volume = 0;

        if (SyncTimeToCurrentlyPlaying) CurrentActive.AudioSource.time = PreviousActive.AudioSource.time;

        CurrentActive.AudioSource.Play();

        this.enabled = true;
    }







    public void StopMusic(FadeSpeeds FadeSpeed)
    {
        if (Activeis1) SetupFadeOut(Source1, FadeSpeed); else SetupFadeOut(Source2, FadeSpeed);
    }
    void SetupFadeOut(MusicObject CurrentActive, FadeSpeeds FadeSpeed)
    {
        if (!CurrentActive.IsPlaying) return;
        float FadeTime = GetFadeSpeed(FadeSpeed);
        CurrentActive.IsFading = true;
        CurrentActive.IsFadingOut = true;
        CurrentActive.FadeTimer = FadeTime;

        this.enabled = true;
    }



    public bool MusicIsCurrentlyPlaying()
    {
        if (Activeis1) return Source1.IsPlaying;
        else return Source2.IsPlaying;
    }
    public int CurrentlyPlayingID()
    {
        if (Activeis1) return Source1.MusicID;
        else return Source2.MusicID;
    }


    float GetFadeSpeed(FadeSpeeds FadeSpeed)
    {
        for(int i = 0; i < FadeData.Length; i++)
        {
            Fades F = FadeData[i];
            if (F.FadeSpeedType == FadeSpeed)
                return F.FadeTime;
        }
        Debug.Log("No Fade Speed Found");
        return 0;
    }
}
