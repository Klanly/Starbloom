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
        HarvestablePlant,
        HarvestableTree,
        ShopInterface,
        ShippingBin,
        BreakableTree,
        ScenePortal,
        Enemy,
        Bed,
        MagneticItem
    }

    public ContextTypes Type;
}
