using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_CharacterLink : MonoBehaviour {


    bool Allow = false;


    private void Awake()
    {
        Transform Child = transform.GetChild(0);
        Child.GetComponent<MoveInput>().enabled = false;
        Child.GetComponent<Locomotion>().enabled = false;
        Child.GetComponent<LocomotionAnim>().enabled = false;
    }


    private void Start()
    {
        transform.SetParent(QuickFind.CharacterManager.transform);
    }



    public void ActivatePlayer()
    {
        Allow = true;
        this.enabled = true;
    }
    private void Update()
    {
        if (QuickFind.NetworkSync == null || QuickFind.NetworkSync.UserID == 0)
            return;

        if(!Allow)
        {
            this.enabled = false;
            return;
        }

        Transform Child = transform.GetChild(0);
        Child.GetComponent<MoveInput>().enabled = true;
        Child.GetComponent<Locomotion>().enabled = true;
        Child.GetComponent<LocomotionAnim>().enabled = true;
        Child.GetComponent<DG_MagnetAttraction>().isOwner = true;
        Child.GetComponent<DG_MovementSync>().isPlayer = true;

        QuickFind.PlayerTrans = Child;
        QuickFind.InputController.MainPlayer.CharLink = this;
        QuickFind.NetworkSync.SetPhotonViewID(transform.GetComponent<PhotonView>().viewID);

        this.enabled = false;
    }
}
