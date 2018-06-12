using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DG_FXObject : MonoBehaviour {

    [HideInInspector] public int DatabaseID;
    [HideInInspector] public bool LockItem;
    public string Name;



    [Header("FX Clusters")]
    public float GlobalScale = 1;
    [ListDrawerSettings(ListElementLabelName = "Name")]
    public FXClusterObject[] ClusterObjects;




    [System.Serializable]
    public class FXClusterObject
    {
        public string Name;
        public int PoolID;
        public float ObjectScale = 1f;

        [Header("Custom Particle Settings")]

        public bool UseCustomSphereRadius;
        [ShowIf("UseCustomSphereRadius")] public float CustomSphereRadius;
        public bool UseCustomGravityModifier;
        [ShowIf("UseCustomGravityModifier")] public float CustomGravityModifier;


#if UNITY_EDITOR
        [Button(ButtonSizes.Small)] public void SetNameToPoolIDName() { Name = QuickFindInEditor.GetPrefabPool().GetItemFromID(PoolID).Name; }
        [Button(ButtonSizes.Small)] public void JumpToPoolID() { Selection.activeGameObject = QuickFindInEditor.GetPrefabPool().GetItemFromID(PoolID).gameObject; }
#endif
    }
}
