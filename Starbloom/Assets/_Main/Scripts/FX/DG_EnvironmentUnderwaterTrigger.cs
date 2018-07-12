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

        if (RainVFX == null) { GetObjects(); if (RainVFX == null) return; }

        bool AboveWater = (QuickFind.PlayerCam.MainCam.transform.position.y > WaterLevel);

        RainVFX.SetActive(AboveWater);
        ThunderstormVFX.SetActive(AboveWater);
        isUnderwater = !AboveWater;
    }

    void GetObjects()
    {
        RainVFX = GameObject.Find("RAIN");
        ThunderstormVFX = GameObject.Find("RAINSTORM");
    }
}
