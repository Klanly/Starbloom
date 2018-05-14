using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_CharacterLink : MonoBehaviour {

    public DG_BasicCharMovement PlayerChar = null;
    public DG_ContextCheckHandler ContextCheck = null;
    public Transform PlayerT;
    public GameObject CharacterFacing = null;

    bool Allow = false;



    private void Awake()
    {
        PlayerChar.enabled = false;
        ContextCheck.enabled = false;
        CharacterFacing.SetActive(false);
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
        if (QuickFind.IDMaster == null || QuickFind.IDMaster.UserID == 0)
            return;

        if(!Allow)
        {
            this.enabled = false;
            return;
        }

        PlayerChar.enabled = true;
        ContextCheck.enabled = true;
        CharacterFacing.SetActive(true);

        QuickFind.InputController.MainPlayer.CharLink = this;

        QuickFind.PlayerCam.CharTrans = PlayerChar.transform;

        this.enabled = false;
    }
}
