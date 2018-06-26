using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_CharacterControllers : Photon.MonoBehaviour
{

    public GameObject MalePrefabRef;
    public GameObject FemalePrefabRef;


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

    public void SpawnCharController(int Gender, DG_NetworkSync.Users NewUser)
    {
        GameObject newPlayerObject;
        if (Gender == 0) newPlayerObject = Instantiate(MalePrefabRef, Vector3.zero, Quaternion.identity);
        else newPlayerObject = Instantiate(FemalePrefabRef, Vector3.zero, Quaternion.identity);

        NewUser.CharacterLink = newPlayerObject.GetComponent<DG_CharacterLink>();
        NewUser.MoveSync = newPlayerObject.transform.GetComponent<DG_MovementSync>();

        QuickFind.ClothingHairManager.PlayerJoined(NewUser);
    }
}
