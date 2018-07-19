using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_InteractionGuiPopup : MonoBehaviour {

    [Header("Floating Inventory Item")]
    public RectTransform FloatingRect;
    [Header("Refs")]
    public UnityEngine.UI.Image DisplayImage;
    public TMPro.TextMeshProUGUI DisplayText;

    DG_ContextObject ActiveContext;



    private void Awake()
    {
        QuickFind.GUIPopup = this;
        transform.localPosition = Vector3.zero;
        FloatingRect.position = new Vector3(8000, 0, 0);
        this.enabled = false;
    }



    public void ShowPopup(DG_ContextObject NewContext)
    {
        ActiveContext = NewContext;

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

        DisplayText.text = DisplayValue;

        //Display Image disabled for now.
        DisplayImage.enabled = false;

        this.enabled = true;
    }
    public void HideToolTip()
    {
        FloatingRect.position = new Vector3(8000, 0, 0);
        this.enabled = false;
    }


    private void Update()
    {
        for (int i = 0; i < QuickFind.InputController.Players.Length; i++)
        {
            DG_PlayerInput.Player P = QuickFind.InputController.Players[i];
            if (P.CharLink == null) continue;

            Vector3 screenPos = P.CharLink.PlayerCam.MainCam.WorldToScreenPoint(ActiveContext.transform.position);
            FloatingRect.position = screenPos;
        }
    }
}
