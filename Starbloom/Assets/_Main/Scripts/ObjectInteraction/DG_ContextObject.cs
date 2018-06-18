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
<<<<<<< .merge_file_a10996
        ScenePortal
=======
        ScenePortal,
        Enemy
>>>>>>> .merge_file_a14280
    }

    public ContextTypes Type;
}
