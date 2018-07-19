using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_EnvironmentUnderwaterTrigger : MonoBehaviour {

    public Transform AquasObject;



    GameObject RainVFX;
    GameObject ThunderstormVFX;

    [System.NonSerialized] public float WaterLevel = -1;
    [System.NonSerialized] public bool isUnderwater;


    private void Awake()
    {
        QuickFind.UnderwaterTrigger = this;
    }

    private void Update()
    {
        WaterLevel = AquasObject.position.y;

        for (int i = 0; i < QuickFind.InputController.Players.Length; i++)
        {
            DG_PlayerInput.Player P = QuickFind.InputController.Players[i];
            if (P.CharLink == null) continue;


            if (RainVFX == null) { GetObjects(); if (RainVFX == null) return; }

            bool AboveWater = (P.CharLink.PlayerCam.CamTrans.position.y > WaterLevel);

            if (AboveWater == isUnderwater)
            {
                RainVFX.SetActive(AboveWater);
                ThunderstormVFX.SetActive(AboveWater);
                isUnderwater = !AboveWater;
            }
        }
    }

    void GetObjects()
    {
        Transform VFX = QuickFind.WeatherController.EffectsHolder.transform.Find("VFX");
        if (VFX == null) return;
        if (VFX.Find("RAIN") == null) return;
        RainVFX = VFX.Find("RAIN").gameObject;
        ThunderstormVFX = VFX.Find("RAINSTORM").gameObject;
    }
}
