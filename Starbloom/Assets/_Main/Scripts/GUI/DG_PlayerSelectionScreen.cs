using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_PlayerSelectionScreen : MonoBehaviour {

    [System.Serializable]
    public class PlayerSelectionScreen
    {
        public CanvasGroup UICanvas = null;
        public UnityEngine.UI.GraphicRaycaster Raycaster;
        public Transform PlayerSelectGrid;
    }

    public PlayerSelectionScreen[] SelectionScreens;


    private void Awake()
    {
        QuickFind.PlayerSelectionScreen = this;
    }

    private void Start()
    {
        QuickFind.EnableCanvas(SelectionScreens[0].UICanvas, false, SelectionScreens[0].Raycaster);
        QuickFind.EnableCanvas(SelectionScreens[1].UICanvas, false, SelectionScreens[1].Raycaster);
        transform.localPosition = Vector3.zero;
    }

    public void OpenPlayerSelectionScreen()
    {
        int UserCount = QuickFind.NetworkSync.UserList.Count;

        if (UserCount == 4) Debug.Log("Group is full. This can only happen if in group of 4, and trying to add local player.");
        if (UserCount == 1) QuickFind.GameStartHandler.CreateNewPlayer(QuickFind.GameStartHandler.GetAvailablePlayerID());
        else
        {
            int ArrayNum = 0;
            if (QuickFind.NetworkSync.Player1PlayerCharacter != -1) ArrayNum = 1;

            QuickFind.EnableCanvas(SelectionScreens[ArrayNum].UICanvas, false, SelectionScreens[ArrayNum].Raycaster);

            List<int> CreatedChars = new List<int>();

            for(int i = 0; i < QuickFind.Farm.PlayerCharacters.Count; i++)
            {
                if (QuickFind.Farm.PlayerCharacters[i].CharacterCreated) CreatedChars.Add(i);
            }
            for(int i = 0; i < QuickFind.NetworkSync.UserList.Count; i++)
                CreatedChars.Remove(QuickFind.NetworkSync.UserList[i].PlayerCharacterID);

            Transform SelectionGrid = SelectionScreens[ArrayNum].PlayerSelectGrid;

            bool newPlayerButtonSet = false;
            for (int i = 0; i < 4; i++)
            {
                Transform GridChild = SelectionGrid.GetChild(i);

                if (CreatedChars.Count > i)
                {
                    GridChild.gameObject.SetActive(true);
                    DG_PlayerSelectButton SelectButton = GridChild.GetComponent<DG_PlayerSelectButton>();
                    SelectButton.PlayerID = CreatedChars[i];
                    SetPlayerData(SelectButton, CreatedChars[i]);
                }
                else if(!newPlayerButtonSet)
                {
                    GridChild.gameObject.SetActive(true);
                    DG_PlayerSelectButton SelectButton = GridChild.GetComponent<DG_PlayerSelectButton>();
                    SelectButton.PlayerID = -1;
                    SetNewPlayer(SelectButton);
                }
                else
                    SelectionGrid.GetChild(i).gameObject.SetActive(false);
            }
        }
    }


    void SetPlayerData(DG_PlayerSelectButton PSB, int PlayerID)
    {     
        PSB.NewPlayerText.enabled = false;

        PSB.PlayerText.enabled = true;
        PSB.PlayerText.text = QuickFind.Farm.PlayerCharacters[PlayerID].Name;
    }
    void SetNewPlayer(DG_PlayerSelectButton PSB)
    {
        PSB.PlayerText.enabled = false;

        PSB.NewPlayerText.enabled = true;
        PSB.NewPlayerText.GetComponent<DG_TextStatic>().ManualLoad();
    }




    public void ReturnButton(DG_PlayerSelectButton SelectButton)
    {
        if(SelectButton.PlayerID == -1)
            QuickFind.GameStartHandler.CreateNewPlayer(0);
        else
            QuickFind.GameStartHandler.CreateNewPlayer(SelectButton.PlayerID);
    }


}
