using System.Net.Mime;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class CloudsGenerator : MonoBehaviour
{
    public enum CloudTypes
    {
        None,
        Basic,
        Cloudy,
        Storm
    }
    [System.Serializable]
    public class CloudGenerationModule
    {
        public CloudTypes Type;
        public GenerationValue GenValues;
    }

    [System.Serializable]
    public class GenerationValue
    {
        public float Density = 2;
        public float ScaleMultiplier = 1;
    }
    [System.Serializable]
    public class CloudObject
    {
        public Transform _T;
        public float Speed;
    }


    [Header("Generation")]
    public CloudTypes CurrentCloudType;
    public CloudGenerationModule[] GenerationModules;
    public GameObject CloudPrefab;
    public Vector3 StartPos = Vector3.zero;
    public Vector3 EndPos = new Vector3(100, 0, 100);
    public Texture2D HeightMap;
    public CloudObject[] GeneratedClouds;
    [Header("Runtime")]
    public Vector3 CloudDirection;
    public float CloudSpeedLow;
    public float CloudSpeedHigh;
    [Header("Debug")]
    public bool GenerateOnStart;

    GenerationValue CurrentGenValue;

    private void Awake()
    {
        QuickFind.CloudGeneration = this;
    }

    private void Start()
    {
        if (GenerateOnStart) { GenerateCloudsByType(CurrentCloudType); }
    }


    void CloudGeneration()
    {
        int count = GeneratedClouds.Length;
        if (Application.isPlaying)
        { for (int i = 0; i < count; i++) {if (GeneratedClouds[i]._T != null) Destroy(GeneratedClouds[i]._T.gameObject); } }
        else
        { for (int i = 0; i < count; i++) { if (GeneratedClouds[0]._T != null) DestroyImmediate(GeneratedClouds[0]._T.gameObject); } }

        List<CloudObject> LoadedClouds = new List<CloudObject>();

        int cnt = 0;
        Vector3 curPos = StartPos;
        while (curPos.z < EndPos.z)
        {
            curPos.z += Random.Range(CurrentGenValue.Density / 2, CurrentGenValue.Density * 1.5f);
            curPos.x = StartPos.x;
            while (curPos.x < EndPos.x)
            {
                curPos.x += Random.Range(CurrentGenValue.Density / 5, CurrentGenValue.Density * 5);
                int x = (int)(HeightMap.width * curPos.x / (EndPos - StartPos).x);
                int y = (int)(HeightMap.height * curPos.z / (EndPos - StartPos).x);
                if (HeightMap.GetPixel(x, y).g < 0.75f)
                {
                    continue;
                }
                float height = HeightMap.GetPixel(x, y).g * 46 - 30;
                float width = HeightMap.GetPixel(x, y).b * 40;
                height *= 5;
                width *= 5;
                curPos.y = Random.Range(StartPos.y, EndPos.y);
                cnt++;
                GameObject placed = (GameObject)Instantiate(CloudPrefab, curPos, Quaternion.identity);
                CloudObject CO = new CloudObject();
                CO._T = placed.transform;
                LoadedClouds.Add(CO);
                GenerateCloudSpeed(CO);

                placed.transform.localScale = new Vector3(
                    width * CurrentGenValue.ScaleMultiplier,
                    10 * CurrentGenValue.ScaleMultiplier,
                    width * CurrentGenValue.ScaleMultiplier
                    );
                placed.transform.parent = transform;
            }
        }

        GeneratedClouds = LoadedClouds.ToArray();
        Debug.Log(cnt);
    }



    void Update()
    {
        float DeltaTime = Time.deltaTime;

        for (int i = 0; i < GeneratedClouds.Length; i++)
        {
            CloudObject CO = GeneratedClouds[i];
            Vector3 Pos = CO._T.localPosition;
            Pos += CloudDirection * GeneratedClouds[i].Speed * DeltaTime;
            if (Pos.x < StartPos.x) { Pos.x = EndPos.x; GenerateCloudSpeed(CO); }
            if (Pos.z < StartPos.z) { Pos.z = EndPos.z; GenerateCloudSpeed(CO); }
            if (Pos.x > EndPos.x) { Pos.x = StartPos.x; GenerateCloudSpeed(CO); }
            if (Pos.z > EndPos.z) { Pos.z = StartPos.z; GenerateCloudSpeed(CO); }
            CO._T.localPosition = Pos;
        }
    }

    void GenerateCloudSpeed(CloudObject CO)
    {
        CO.Speed = Random.Range(CloudSpeedLow, CloudSpeedHigh);
    }




    public void GenerateCloudsByType(CloudTypes Type)
    {
        CurrentCloudType = Type;
        if (CurrentCloudType != CloudTypes.None)
        {
            GetCurrentGenValue();
            CloudGeneration();
        }
    }
    void GetCurrentGenValue()
    {
        for(int i = 0; i < GenerationModules.Length; i++ )
        {
            if (GenerationModules[i].Type == CurrentCloudType)
                CurrentGenValue = GenerationModules[i].GenValues;
        }
    }



    [Button(ButtonSizes.Small)]
    public void RefreshCloudType()
    {
        if(Application.isPlaying)
            GenerateCloudsByType(CurrentCloudType);
    }
}
