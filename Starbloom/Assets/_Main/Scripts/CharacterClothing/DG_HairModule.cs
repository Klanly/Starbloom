using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kalagaan.HairDesignerExtension;

public class DG_HairModule : MonoBehaviour {


    public HairDesignerBase HD;

    private void Start(){if (transform.parent == null) transform.gameObject.SetActive(false);}

    public void LoadHairColliders(DG_CharacterLink CharLink)
    {
        HD.m_capsuleColliders = CharLink.HairColliders;
    }
}
