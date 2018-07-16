using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_EnvironmentColorSync : MonoBehaviour {

    [ReadOnly] public float V;
    [Header("Water Values")]
    public AQUAS_Reflection ReflectionScript;
    public Transform Sun;
    public MeshRenderer WaterRend;
    public float SpecularBase;
    public float SpecularMultiplier;
    public float SpecularFogOffset;
    [ReadOnly] public float CurrentSpecularValue;
    public float GlossBase;
    public float GlossMultiplier;
    public float GlossFogOffset;
    [ReadOnly] public float CurrentGlossValue;
    public Color DeepWaterBase;
    public float DeepWaterVBase;
    public float DeepWaterMultiplier;
    [ReadOnly] public float DeepWaterCurrentV;
    public float FogLighten;
    [ReadOnly] public float FogDeepWaterOffset;
    [ReadOnly] public float FinalDeepWaterCurrentV;
    [Header("UnderWater Values")]
    public float AboveWaterDensity;
    public float BelowWaterDensity;
    [Header("Snow Values")]
    public float BaseExposureLevel;
    public float ExposureMultiplier;
    public float BaseRoughness;
    public float RoughnessMulitplier;


    [System.NonSerialized] public bool AllowSnowUpdate = false;

    bool isFog;
    bool isRain;
    bool isSnow;
    float SunHeight;
    EnviroWeatherPreset weatherType;

    private void Awake()
    {
        QuickFind.EnvironmentColorSync = this;
    }

    private void Update()
    {
        if (QuickFind.WeatherController == null || QuickFind.WeatherController.Weather == null || QuickFind.WeatherController.Weather.currentActiveWeatherPreset == null) return;
        weatherType = QuickFind.WeatherController.Weather.currentActiveWeatherPreset;

        isFog = (weatherType.fogDistance < 100);
        isRain = (weatherType.wetnessLevel > 0);
        SunHeight = Sun.position.y;

        //Disable Reflections if necessary
        if (isFog || isRain || QuickFind.UserSettings.GlobalDisableWaterReflection) ReflectionScript.enabled = false;
        else ReflectionScript.enabled = true;


        Color MainLight = QuickFind.WeatherController.MainLight.color;
        Color AmbientLight = RenderSettings.ambientSkyColor;
        float H;
        float S;
        Color.RGBToHSV(AmbientLight, out H, out S, out V);

        if (WaterRend.gameObject.activeInHierarchy) AdjustWater(V, AmbientLight);

        AdjustUnderWater();
        AdjustSnow(V);
    }
    void AdjustWater(float V, Color AmbientLight)
    {
        float SpecOffset = 0;
        if (isFog) SpecOffset = SpecularFogOffset;
        //
        CurrentSpecularValue = SpecularBase + (((1 - V) * SpecularMultiplier) + SpecOffset);
        WaterRend.material.SetFloat("_Specular", CurrentSpecularValue);

        float GlossOffset = 0;
        if (isFog) GlossOffset = GlossFogOffset;
        //
        CurrentGlossValue = GlossBase + (((1 - V) * GlossMultiplier) + GlossOffset);
        WaterRend.material.SetFloat("_Gloss", CurrentGlossValue);



        // Get Main Light brightness level.
        float H1;
        float S1;
        float V1;
        Color.RGBToHSV(DeepWaterBase, out H1, out S1, out V1);
        DeepWaterCurrentV = (DeepWaterVBase + ((1 - V) * DeepWaterMultiplier));

        Color NewCol = Color.HSVToRGB(H1, S1, DeepWaterCurrentV);

        float FogLevel = FogLighten * SunHeight;
        if (isFog)
        {
            NewCol.r += FogLighten;
            NewCol.g += FogLighten;
            NewCol.b += FogLighten;
        }


        WaterRend.material.SetColor("_DeepWaterColor", NewCol);

    }
    void AdjustUnderWater()
    {
        if(QuickFind.UnderwaterTrigger.isUnderwater)
        {
            WaterRend.material.SetFloat("_Density", BelowWaterDensity);
        }
        else
        {
            WaterRend.material.SetFloat("_Density", AboveWaterDensity);
        }
    }

    void AdjustSnow(float V)
    {
        if (QuickFind.WeatherHandler.CurrentSeason != WeatherHandler.Seasons.Winter) return;
        if (QuickFind.SnowHandler == null) return;
        if (!QuickFind.SnowHandler.isActiveAndEnabled) return;
        if (!AllowSnowUpdate) return;
        AllowSnowUpdate = false;

        isSnow = (weatherType.snowLevel > 0);

        QuickFind.SnowHandler.maxExposure = BaseExposureLevel + (V * ExposureMultiplier);

        if (isFog || isSnow)
            QuickFind.SnowHandler.smoothness = 0;
        else
            QuickFind.SnowHandler.smoothness = BaseRoughness + (V * RoughnessMulitplier);


    }
}
