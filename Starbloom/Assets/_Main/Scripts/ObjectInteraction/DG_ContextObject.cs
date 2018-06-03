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
        Pick_And_Break
    }


    public bool ThisIsGrowthItem;
    public int ContextID;
    public ContextTypes Type;



    public NetworkObject ScanUpTree(Transform T)
    {
        NetworkObject NO = T.GetComponent<NetworkObject>();
        if (NO == null)
            return ScanUpTree(T.parent);
        else
            return NO;
    }
}
