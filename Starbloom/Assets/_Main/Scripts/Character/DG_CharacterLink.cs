using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_CharacterLink : MonoBehaviour {


    public MoveInput CharInput = null;
    public Locomotion PlayerChar = null;
    public LocomotionAnim CharAnim = null;

    public Transform PlayerT;

    bool Allow = false;



    private void Awake()
    {
        CharInput.enabled = false;
        PlayerChar.enabled = false;
        CharAnim.enabled = false;
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


        CharInput.enabled = true;
        PlayerChar.enabled = true;
        CharAnim.enabled = true;

        QuickFind.InputController.MainPlayer.CharLink = this;
        QuickFind.PlayerCam.CharTrans = PlayerChar.transform;
        QuickFind.NetworkSync.SetPhotonViewID(transform.GetComponent<PhotonView>().viewID);
        QuickFind.ContextDetectionHandler.CharacterLink = this;

        this.enabled = false;
    }
}
