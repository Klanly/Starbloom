using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_ScatterPointReference : MonoBehaviour {

    public Transform[] SpawnPoints;

    [Header("Velocity")]
    public bool RandomizeVelocity;

    [ShowIf("RandomizeVelocity")]
    public float XMin;
    [ShowIf("RandomizeVelocity")]
    public float XMax;
    [ShowIf("RandomizeVelocity")]
    public float YMin;
    [ShowIf("RandomizeVelocity")]
    public float YMax;
    [ShowIf("RandomizeVelocity")]
    public float ZMin;
    [ShowIf("RandomizeVelocity")]
    public float ZMax;



    public Vector3 GetSpawnPoint()
    {
        return SpawnPoints[Random.Range(0, SpawnPoints.Length)].position;
    }
    public Vector3 RandomVelocity()
    {
        Vector3 Vec;
        if (!RandomizeVelocity) Vec = Vector3.zero;
        else
        {
            Vec.x = Random.Range(-XMin, XMax);
            Vec.y = Random.Range(-YMin, YMax);
            Vec.z = Random.Range(-ZMin, ZMax);
        }
        return Vec;
    }
}
