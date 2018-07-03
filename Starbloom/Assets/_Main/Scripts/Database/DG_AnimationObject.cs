using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_AnimationObject : MonoBehaviour
{

    [HideInInspector] public int DatabaseID;
    [HideInInspector] public bool LockItem;
    public string Name;


    [Header("Animation States")]
    public DG_AnimationSync.AnimationActionStates AnimationActionState;
    public DG_AnimationSync.AnimationSubStates[] AnimationSubstateValues;

    [Header("Equip/Unequip FX")]
    public DG_AnimationSync.AnimationResponseType ResponseType;
    public DG_EquipmentAnimationHandler.EquipmentAnimationData SheathUnSheathTransition;
    public bool isWeapon;
}
