using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;



public class DG_ContextObject : MonoBehaviour
{
    public enum ContextTypes
    {
        Conversation,
        Treasure,
        NameChange,
        Vehicle,
        PickupItem,
        Breakable,
        MoveableStorage,
        Soil,
        Pick_And_Break,
        PickOnly
    }

    [System.Serializable]
    public class BreakData
    {
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
    }



    public bool ThisIsGrowthItem;
    public int ContextID;
    public ContextTypes Type;


    public BreakData[] BreakValues;



    public Vector3 GetSpawnPoint()
    {
        BreakData BD = BreakValues[0];
        return BD.SpawnPoints[Random.Range(0, BD.SpawnPoints.Length)].position;
    }
    public Vector3 RandomVelocity()
    {
        Vector3 Vec;
        BreakData BD = BreakValues[0];
        if (!BD.RandomizeVelocity) Vec = Vector3.zero;
        else
        {
            Vec.x = Random.Range(-BD.XMin, BD.XMax);
            Vec.y = Random.Range(-BD.YMin, BD.YMax);
            Vec.z = Random.Range(-BD.ZMin, BD.ZMax);
        }
        return Vec;
    }
 }
