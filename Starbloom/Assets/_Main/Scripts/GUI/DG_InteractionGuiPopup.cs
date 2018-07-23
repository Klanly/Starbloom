using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_InteractionGuiPopup : MonoBehaviour {

    [System.Serializable]
    public class PlayerPopupGui
    {
        [Header("Floating Inventory Item")]
        public RectTransform FloatingRect;
        [Header("Refs")]
        public UnityEngine.UI.Image DisplayImage;
        public TMPro.TextMeshProUGUI DisplayText;

        [System.NonSerialized] public DG_ContextObject ActiveContext;
    }

    public PlayerPopupGui[] Popups;



    private void Awake()
    {
        QuickFind.GUIPopup = this;
        transform.localPosition = Vector3.zero;
        Popups[0].FloatingRect.localPosition = new Vector3(8000, 0, 0);
        Popups[1].FloatingRect.localPosition = new Vector3(8000, 0, 0);
        this.enabled = false;
    }



    public void ShowPopup(DG_ContextObject NewContext, int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        Popups[ArrayNum].ActiveContext = NewContext;

        string DisplayValue = string.Empty;
        switch (NewContext.Type)
        {
            case DG_ContextObject.ContextTypes.ScenePortal:
                {
                    DG_ScenePortalTrigger SPT = NewContext.GetComponent<DG_ScenePortalTrigger>();
                    DisplayValue = QuickFind.SceneList.GetLocalizedSceneName(SPT.SceneString, SPT);
                }
                break;
        }

        Popups[ArrayNum].DisplayText.text = DisplayValue;

        //Display Image disabled for now.
        Popups[ArrayNum].DisplayImage.enabled = false;

        this.enabled = true;
    }
    public void HideToolTip(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        Popups[ArrayNum].FloatingRect.position = new Vector3(8000, 0, 0);
        this.enabled = false;
    }


    private void Update()
    {
        for (int i = 0; i < QuickFind.InputController.Players.Length; i++)
        {
            DG_PlayerInput.Player P = QuickFind.InputController.Players[i];
            if (P.CharLink == null) continue;

            Vector3 screenPos = P.CharLink.PlayerCam.MainCam.WorldToScreenPoint(Popups[i].ActiveContext.transform.position);
            Popups[i].FloatingRect.position = screenPos;
        }
    }
}
