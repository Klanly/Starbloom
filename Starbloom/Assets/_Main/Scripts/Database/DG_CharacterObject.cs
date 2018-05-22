using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DG_CharacterObject : MonoBehaviour {

    [HideInInspector] public int DatabaseID;
    [HideInInspector] public bool LockItem;

    public string ObjectName;
    [Header("Localization")]
    public int NameWordID;
}
