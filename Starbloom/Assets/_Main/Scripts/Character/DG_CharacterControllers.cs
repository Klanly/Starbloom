using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_CharacterControllers : Photon.MonoBehaviour
{

    public GameObject DebugCharacter = null;

    private void Awake()
    {
        QuickFind.CharacterManager = this;
    }
}
