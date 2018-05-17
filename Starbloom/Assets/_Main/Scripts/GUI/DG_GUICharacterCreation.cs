using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_GUICharacterCreation : MonoBehaviour {

    [Header("Canvases")]
    public CanvasGroup UICanvas = null;
    public GameObject FarmNameObject = null;
    [Header("Static Text")]
    public DG_TextStatic[] StaticTextArray;

    bool InitialCreate;
    int CharID;

    private void Awake()
    {
        QuickFind.CharacterCreation = this;
        QuickFind.EnableCanvas(UICanvas, false);
    }
    private void Start()
    {
        transform.localPosition = Vector3.zero;

        foreach (DG_TextStatic TS in StaticTextArray)
            TS.ManualLoad();
    }


    public void OpenCharacterCreation(bool InitialCreation)
    {
        if (QuickFind.NetworkMaster.RequestedCharacterNum == -1)
            QuickFind.NetworkMaster.RequestedCharacterNum = QuickFind.NetworkSync.UserList.Count - 1;

        QuickFind.NetworkSync.PlayerCharacterID = QuickFind.NetworkMaster.RequestedCharacterNum;

        QuickFind.NetworkSync.GetUserByID(QuickFind.NetworkSync.UserID).PlayerCharacterID = QuickFind.NetworkSync.PlayerCharacterID;
        if (QuickFind.NetworkMaster.RequestedCharacterNum != 0)
            FarmNameObject.SetActive(false);

        CharID = QuickFind.NetworkMaster.RequestedCharacterNum;
        InitialCreate = InitialCreation;
        QuickFind.EnableCanvas(UICanvas, true);
    }




    public void CharacterNameFieldChanged(TMPro.TMP_InputField InputField)
    {
        QuickFind.Farm.PlayerCharacters[CharID].Name = InputField.text;
    }
    public void FarmNameFieldChanged(TMPro.TMP_InputField InputField)
    {
        QuickFind.Farm.FarmName = InputField.text;
    }


    public void GoodToGOButton()
    {
        if (QuickFind.Farm.PlayerCharacters[CharID].Name == string.Empty)
            QuickFind.Farm.PlayerCharacters[CharID].Name = "Default Name " + QuickFind.NetworkSync.PlayerCharacterID.ToString();
        if (QuickFind.Farm.FarmName == string.Empty)
            QuickFind.Farm.FarmName = "Default Farm Name";

        if (PhotonNetwork.isMasterClient)
            QuickFind.SaveHandler.SaveFileName = QuickFind.Farm.FarmName;

        QuickFind.EnableCanvas(UICanvas, false);
        QuickFind.MainMenuUI.TriggerGameStart();
    }
}
