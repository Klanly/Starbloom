using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class IconValues : MonoBehaviour {

    [System.Serializable]
    public class Pivots
    {
        public GameObject _models;
        public Vector3 _pivotPosition;
        public Vector3 _pivotRotation;
        public Vector3 _pivotScale;
        public bool AdjustMaterialForIcon;
        [ShowIf("AdjustMaterialForIcon")]
        public Material IconMaterial;
    }

    [ListDrawerSettings(ListElementLabelName = "_models", NumberOfItemsPerPage = 8)]
    public List<Pivots> ModelPivots;
}
