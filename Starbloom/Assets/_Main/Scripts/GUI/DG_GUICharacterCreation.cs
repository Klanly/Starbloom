using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_GUICharacterCreation : MonoBehaviour {

    public bool isPlayer1;

    [Header("Canvases")]
    public CanvasGroup UICanvas = null;
    public GameObject FarmNameObject = null;
    [Header("Static Text")]
    public DG_TextStatic[] StaticTextArray;



    private void Awake()
    {
        QuickFind.CharacterCreation = this;
        QuickFind.EnableCanvas(UICanvas, false);
    }
    private void Start()
    {
        transform.localPosition = Vector3.zero;
    }


    public void OpenCharacterCreation(bool InitialCreation)
    {
        foreach (DG_TextStatic TS in StaticTextArray) TS.ManualLoad();
        FarmNameObject.SetActive(QuickFind.Farm.FarmName == string.Empty);
        QuickFind.EnableCanvas(UICanvas, true);
    }




    public void CharacterNameFieldChanged(TMPro.TMP_InputField InputField)
    {
        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if(!isPlayer1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;
        QuickFind.Farm.PlayerCharacters[PlayerID].Name = InputField.text;
    }
    public void FarmNameFieldChanged(TMPro.TMP_InputField InputField)
    {
        QuickFind.Farm.FarmName = InputField.text;
    }


    public void GoodToGOButton()
    {
        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (!isPlayer1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;
        if (QuickFind.Farm.PlayerCharacters[PlayerID].Name == string.Empty) QuickFind.Farm.PlayerCharacters[PlayerID].Name = "Default Name " + PlayerID.ToString();
        if (QuickFind.Farm.FarmName == string.Empty) QuickFind.Farm.FarmName = "Default Farm Name";

        if (PhotonNetwork.isMasterClient) QuickFind.SaveHandler.SaveFileName = QuickFind.Farm.FarmName;

        QuickFind.EnableCanvas(UICanvas, false);
        QuickFind.GameStartHandler.CharacterFinishedBeingCreated(PlayerID);
    }
}
