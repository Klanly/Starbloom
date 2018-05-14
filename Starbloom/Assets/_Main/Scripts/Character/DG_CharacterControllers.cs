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


    //Network Call
    public void OnJoinedRoom()
    {
        if (DebugCharacter != null)
        {
            for(int i = 0; i< transform.childCount; i++)
                Destroy(DebugCharacter);
        }

        GameObject newPlayerObject = PhotonNetwork.Instantiate("MainPlayer", Vector3.zero, Quaternion.identity, 0);
        DG_CharacterLink CL = newPlayerObject.GetComponent<DG_CharacterLink>();
        CL.ActivatePlayer();
    }
}
