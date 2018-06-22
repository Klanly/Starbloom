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
    public bool AutoAddHairOnStart;
    public bool AutoSetPlayerLinkAtStart;


    public void Start()
    {
        if (AutoSetPlayerLinkAtStart) AutoSetPlayerLink();
        if (AutoAddHairOnStart) { QuickFind.ClothingHairManager.AddClothingItem(CharLink, HairID); }
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


        [Button(ButtonSizes.Medium)]
        public void CharacterSwap()
        {
            if (Application.isPlaying) return;
            if (Male.activeInHierarchy) { Male.SetActive(false); Female.SetActive(true); Vector9.Vector9ToTransform(DebugSceneHair, FHairPoint, false); }
            else { Male.SetActive(true); Female.SetActive(false); Vector9.Vector9ToTransform(DebugSceneHair, MHairPoint, false); }
        }
    }

}
