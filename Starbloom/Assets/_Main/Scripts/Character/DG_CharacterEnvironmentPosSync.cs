using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_CharacterEnvironmentPosSync : MonoBehaviour {


    Transform _T;

    private void Awake()
    {
        _T = transform;
    }

    // Update is called once per frame
    void Update ()
    {
        if (QuickFind.PlayerTrans == null) return;
        else _T.position = QuickFind.PlayerTrans.position;

    }
}
