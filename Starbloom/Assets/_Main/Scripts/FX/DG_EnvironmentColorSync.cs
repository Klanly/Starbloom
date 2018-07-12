using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_EnvironmentColorSync : MonoBehaviour {

    [ReadOnly] public float V;
    [Header("Cloud Values")]
    public TrueClouds.CloudCamera CloudPostScript;
    public AQUAS_Reflection ReflectionScript;
    public float LightLighten;
    public float LightenVMultiplier;
    public Color RainLightColor;
    public float ShadowDarken;
    public float DarkenVMultiplier;
    public Color RainDarkColor;
    public float RainSunHeightMultiplier;
    [Header("Water Values")]
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

    bool isFog;
    bool isRain;
    float SunHeight;

    private void Update()
    {
        if (QuickFind.WeatherController == null || QuickFind.WeatherController.Weather == null || QuickFind.WeatherController.Weather.currentActiveWeatherPreset == null) return;
        EnviroWeatherPreset weatherType = QuickFind.WeatherController.Weather.currentActiveWeatherPreset;

        isFog = (weatherType.fogDistance < 100);
        isRain = (weatherType.wetnessLevel > 0);
        SunHeight = Sun.position.y;

        //Disable Clouds if necessary
        if (QuickFind.CloudGeneration.CurrentCloudType == CloudsGenerator.CloudTypes.None || QuickFind.UserSettings.GlobalDisableCloudRendering) CloudPostScript.enabled = false;
        else CloudPostScript.enabled = true;

        //Disable Reflections if necessary
        if (isFog || isRain || QuickFind.UserSettings.GlobalDisableWaterReflection) ReflectionScript.enabled = false;
        else ReflectionScript.enabled = true;


        Color MainLight = QuickFind.WeatherController.MainLight.color;
        Color AmbientLight = RenderSettings.ambientSkyColor;
        float H;
        float S;
        Color.RGBToHSV(AmbientLight, out H, out S, out V);

        if (CloudPostScript.enabled) AdjustClouds(MainLight, V);
        if (WaterRend.gameObject.activeInHierarchy) AdjustWater(V, AmbientLight);

        AdjustUnderWater();

    }
    void AdjustClouds(Color MainLight, float V)
    {
        //
        float AdditiveLit = LightLighten * (V + LightenVMultiplier);

        Color LitLighting;
        if (!isRain)
        {
            LitLighting = MainLight;
            LitLighting.r += AdditiveLit;
            LitLighting.g += AdditiveLit;
            LitLighting.b += AdditiveLit;
        }
        else
        {
            float TimeOfDayOffset = RainSunHeightMultiplier * SunHeight;
            LitLighting = RainLightColor;
            LitLighting.r += TimeOfDayOffset;
            LitLighting.g += TimeOfDayOffset;
            LitLighting.b += TimeOfDayOffset;
        }


        CloudPostScript.LightColor = LitLighting;

        //
        float AdditiveDark = ShadowDarken * (V + DarkenVMultiplier);

        Color ShadowLighting;
        if (!isRain)
        {
            ShadowLighting = MainLight;
            ShadowLighting.r -= AdditiveDark;
            ShadowLighting.g -= AdditiveDark;
            ShadowLighting.b -= AdditiveDark;
        }
        else
        {
            float TimeOfDayOffset = RainSunHeightMultiplier * SunHeight;
            ShadowLighting = RainDarkColor;
            ShadowLighting.r += TimeOfDayOffset;
            ShadowLighting.g += TimeOfDayOffset;
            ShadowLighting.b += TimeOfDayOffset;
        }

        CloudPostScript.ShadowColor = ShadowLighting;
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
}
