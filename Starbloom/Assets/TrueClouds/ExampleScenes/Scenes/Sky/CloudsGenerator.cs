using System.Net.Mime;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class CloudsGenerator : MonoBehaviour
{
    [System.Serializable]
    public class GenerationValue
    {
        public float Density = 2;
        public float ScaleMultiplier = 1;
        public Vector3 StartPos = Vector3.zero;
        public Vector3 EndPos = new Vector3(100, 0, 100);
        public GameObject[] Prefabs = new GameObject[0];
    }


    [Header("Generation")]
    public bool GenerationEnabled = false;
    public GenerationValue GenerationValues;
    public Texture2D HeightMap;
    public Transform[] GeneratedClouds;
    [Header("Runtime")]
    public Vector3 CloudDirection;
    public float CloudSpeed;
    [Header("Debug")]
    public bool GenerateOnStart;



    private void Start()
    {
        if (GenerateOnStart) GenerateClouds();
    }


    [Button(ButtonSizes.Large)]
    void GenerateClouds()
    {
        if (!GenerationEnabled) return;

        int count = GeneratedClouds.Length;
        if (Application.isPlaying)
        { for (int i = 0; i < count; i++) Destroy(GeneratedClouds[i].gameObject); }
        else
        { for (int i = 0; i < count; i++) DestroyImmediate(GeneratedClouds[0].gameObject); }

        List<Transform> LoadedClouds = new List<Transform>();

        int cnt = 0;
        Vector3 curPos = GenerationValues.StartPos;
        while (curPos.z < GenerationValues.EndPos.z)
        {
            curPos.z += Random.Range(GenerationValues.Density / 2, GenerationValues.Density * 1.5f);
            curPos.x = GenerationValues.StartPos.x;
            while (curPos.x < GenerationValues.EndPos.x)
            {
                curPos.x += Random.Range(GenerationValues.Density / 5, GenerationValues.Density * 5);
                int x = (int)(HeightMap.width * curPos.x / (GenerationValues.EndPos - GenerationValues.StartPos).x);
                int y = (int)(HeightMap.height * curPos.z / (GenerationValues.EndPos - GenerationValues.StartPos).x);
                if (HeightMap.GetPixel(x, y).g < 0.75f)
                {
                    continue;
                }
                float height = HeightMap.GetPixel(x, y).g * 46 - 30;
                float width = HeightMap.GetPixel(x, y).b * 40;
                height *= 5;
                width *= 5;
                curPos.y = 150;
                int id = Random.Range(0, GenerationValues.Prefabs.Length);
                cnt++;
                GameObject placed = (GameObject)Instantiate(GenerationValues.Prefabs[id], curPos, Quaternion.identity);
                LoadedClouds.Add(placed.transform);
                placed.transform.localScale = new Vector3(
                    width * GenerationValues.ScaleMultiplier,
                    10 * GenerationValues.ScaleMultiplier,
                    width * GenerationValues.ScaleMultiplier
                    );
                placed.transform.parent = transform;
            }
        }

        GeneratedClouds = LoadedClouds.ToArray();
        Debug.Log(cnt);
    }

    [Button(ButtonSizes.Small)]
    public void EnableShadowCasters()
    {
        for (int i = 0; i < GeneratedClouds.Length; i++)
            GeneratedClouds[i].GetChild(1).gameObject.SetActive(true);
    }
    [Button(ButtonSizes.Small)]
    public void DisableShadowCasters()
    {
        for (int i = 0; i < GeneratedClouds.Length; i++)
            GeneratedClouds[i].GetChild(1).gameObject.SetActive(false);
    }

    void Update()
    {
        for(int i = 0; i < GeneratedClouds.Length; i++)
        {
            Transform Child = GeneratedClouds[i];
            Vector3 Pos = Child.localPosition;
            if (Pos.x < GenerationValues.StartPos.x) Pos.x = GenerationValues.EndPos.x;
            if (Pos.z < GenerationValues.StartPos.z) Pos.z = GenerationValues.EndPos.z;
            if (Pos.x > GenerationValues.EndPos.x) Pos.x = GenerationValues.StartPos.x;
            if (Pos.z > GenerationValues.EndPos.z) Pos.z = GenerationValues.StartPos.z;
            Child.localPosition = Pos;
        }
    }
}
