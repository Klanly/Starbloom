using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_CharacterControllers : Photon.MonoBehaviour
{

    public GameObject[] DebugCharacters = null;



    private void Awake()
    {
        QuickFind.CharacterManager = this;
    }
    public int GetAvailablePlayerID()
    {
        for(int i = 0; i < int.MaxValue; i++)
        {
            bool Allow = true;
            for(int iN = 0; iN < QuickFind.NetworkSync.UserList.Count; iN++)
            {
                if(QuickFind.NetworkSync.UserList[iN].PlayerCharacterID == i)
                {
                    Allow = false;
                    break;
                }
            }

            if (Allow)
                return i;
        }
        return 0;
    }
}
