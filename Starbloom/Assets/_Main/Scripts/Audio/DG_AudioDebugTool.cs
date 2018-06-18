using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_AudioDebugTool : MonoBehaviour {


    [System.Serializable]
    public class ZoneTrigger
    {
        public DG_AudioManager.AudioFXZoneTypes FXType;

        [Button()] public void ChangeZoneType() { if (!Application.isPlaying) return; QuickFind.AudioManager.SetAudioZone(FXType); }
        [Button()] public void StopMusic() { if (!Application.isPlaying) return; QuickFind.AudioManager.StopMusic(DG_AudioCrossFading.FadeSpeeds.Normal); }
    }

    public ZoneTrigger ZTrigger;
}
