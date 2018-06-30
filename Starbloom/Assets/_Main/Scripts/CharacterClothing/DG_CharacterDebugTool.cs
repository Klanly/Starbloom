using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_CharacterDebugTool : MonoBehaviour {

    [Header("Editor")]
    public ScenePositionAdjust PositionAdjustTool;

    [Header("Clothing")]
    public DG_CharacterLink CharLink;
    public int HairID;

    [Header("Debug")]
    public bool AutoSetPlayerLinkAtStart;


    public void Start()
    {
        if (AutoSetPlayerLinkAtStart) AutoSetPlayerLink();
    }
    void AutoSetPlayerLink()
    {
        if (GameObject.Find("MainPlayer_Male") != null)
            CharLink = GameObject.Find("MainPlayer_Male").GetComponent<DG_CharacterLink>();
        else
            CharLink = GameObject.Find("MainPlayer_Female").GetComponent<DG_CharacterLink>();

        Destroy(CharLink.transform.GetChild(0).GetComponent<DG_MovementSync>());
    }

    [System.Serializable]
    public class ScenePositionAdjust
    {
        public GameObject Male;
        public GameObject Female;
        public Transform DebugSceneHair;
        public Vector9 MHairPoint;
        public Vector9 FHairPoint;


        [Button(ButtonSizes.Small)]
        public void CharacterSwap()
        {
            if (Application.isPlaying) return;
            if (Male.activeInHierarchy) { Male.SetActive(false); Female.SetActive(true); Vector9.Vector9ToTransform(DebugSceneHair, FHairPoint, false); }
            else { Male.SetActive(true); Female.SetActive(false); Vector9.Vector9ToTransform(DebugSceneHair, MHairPoint, false); }
        }
        [Button(ButtonSizes.Small)]
        public void AlignHair()
        {
            if (Application.isPlaying) return;
            if (Male.activeInHierarchy)
            {
                Transform AttachPoint = Male.GetComponent<DG_CharacterLink>().GetAttachmentByType(DG_ClothingHairManager.ClothHairType.Hair).AttachmentPoint;
                DebugSceneHair.transform.SetParent(AttachPoint);
                Vector9.Vector9ToTransform(DebugSceneHair, MHairPoint, true);
                DebugSceneHair.transform.SetParent(DebugSceneHair.root.parent);
            }
            else
            {
                Transform AttachPoint = Female.GetComponent<DG_CharacterLink>().GetAttachmentByType(DG_ClothingHairManager.ClothHairType.Hair).AttachmentPoint;
                DebugSceneHair.transform.SetParent(AttachPoint);
                Vector9.Vector9ToTransform(DebugSceneHair, FHairPoint, true);
                DebugSceneHair.transform.SetParent(DebugSceneHair.root.parent);
            }
        }
    }

}
