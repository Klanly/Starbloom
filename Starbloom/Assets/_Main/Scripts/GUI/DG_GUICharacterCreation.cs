using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_GUICharacterCreation : MonoBehaviour {

    [System.Serializable]
    public class PlayerCharCreation
    {
        [Header("Canvases")]
        public CanvasGroup UICanvas = null;
        public UnityEngine.UI.GraphicRaycaster Raycaster;
        public GameObject FarmNameObject = null;
        [Header("Static Text")]
        public DG_TextStatic[] StaticTextArray;
        [System.NonSerialized] public bool FirstCreation = true;
    }

    public PlayerCharCreation[] PlayerCreations;


    private void Awake()
    {
        QuickFind.CharacterCreation = this;
        QuickFind.EnableCanvas(PlayerCreations[0].UICanvas, false, PlayerCreations[0].Raycaster);
        QuickFind.EnableCanvas(PlayerCreations[1].UICanvas, false, PlayerCreations[1].Raycaster);
    }
    private void Start()
    {
        transform.localPosition = Vector3.zero;
    }


    public void OpenCharacterCreation(bool InitialCreation, int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        PlayerCreations[ArrayNum].FirstCreation = InitialCreation;

        QuickFind.InputController.Players[ArrayNum].CurrentInputState = DG_PlayerInput.CurrentInputState.InCinema;

        foreach (DG_TextStatic TS in PlayerCreations[ArrayNum].StaticTextArray) TS.ManualLoad();
        PlayerCreations[ArrayNum].FarmNameObject.SetActive(QuickFind.Farm.FarmName == string.Empty);
        QuickFind.EnableCanvas(PlayerCreations[ArrayNum].UICanvas, true, PlayerCreations[ArrayNum].Raycaster);
    }



    public void FarmNameFieldChanged(TMPro.TMP_InputField InputField)
    {
        QuickFind.Farm.FarmName = InputField.text;
    }


    public void Character1NameFieldChanged(TMPro.TMP_InputField InputField)
    {
        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        QuickFind.Farm.PlayerCharacters[PlayerID].Name = InputField.text;
    }
    public void Character2NameFieldChanged(TMPro.TMP_InputField InputField)
    {
        int PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;
        QuickFind.Farm.PlayerCharacters[PlayerID].Name = InputField.text;
    }



    public void Player1GoodToGo()
    {
        GoodToGOButton(true);
    }
    public void Player2GoodToGo()
    {
        GoodToGOButton(false);
    }


    void GoodToGOButton(bool Player1)
    {
        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (!Player1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        if (PhotonNetwork.isMasterClient && Player1) QuickFind.SaveHandler.SaveFileName = QuickFind.Farm.FarmName;

        int ArrayNum = 0;
        if (!Player1) ArrayNum = 1;

        QuickFind.InputController.Players[ArrayNum].CurrentInputState = DG_PlayerInput.CurrentInputState.Default;

        QuickFind.EnableCanvas(PlayerCreations[ArrayNum].UICanvas, false, PlayerCreations[ArrayNum].Raycaster);
        QuickFind.GameStartHandler.CharacterFinishedBeingCreated();
    }
}
