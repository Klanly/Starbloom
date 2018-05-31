using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DG_ContextObject : MonoBehaviour
{
    public enum ContextTypes
    {
        Conversation,
        Treasure,
        NameChange,
        Vehicle,
        PickupItem,
        Growable,
        MoveableStorage,
        Soil
    }



    public int ContextID;
    public ContextTypes Type;
    [Header("Scale Object on Contact")]
    public bool AllowScaling;
    public float ScaleAmount;
    public float ScaleSpeed;
}
