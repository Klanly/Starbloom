using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_LocalCoopMaster : MonoBehaviour {

    [ReadOnly] public bool LocalCoopActive;


    private void Awake()
    {
        QuickFind.LocalCoopController = this;
    }

    public void TriggerLocalCoop(bool isEnable)
    {
        LocalCoopActive = isEnable;
        UpdateCameras();
        UpdateUIs();

        if (QuickFind.GameSettings.BypassMainMenu)
        {
            if (LocalCoopActive && QuickFind.SnowHandler.isActiveAndEnabled) QuickFind.SnowHandler.EnableTrigger();
        }

        if (LocalCoopActive)
            QuickFind.GameStartHandler.Connected();
        else
            Debug.Log("Destroy Extra Player");
    }
    void UpdateCameras()
    {
        QuickFind.PlayerCam.CameraRiggs[1].Rigg.gameObject.SetActive(LocalCoopActive);
        Camera C = QuickFind.PlayerCam.CameraRiggs[0].MainCam;
        Rect R = C.rect;
        if (LocalCoopActive)
        { R.y = .5f; R.width = .7f; R.height = .5f; }
        else
        { R.y = 0f; R.width = 1f; R.height = 1f; }
        C.rect = R;
    }
    void UpdateUIs()
    {
        QuickFind.GUI_MainOverview.BlankSpaceLeft.SetActive(LocalCoopActive);
        QuickFind.GUI_MainOverview.BlankSpaceRight.SetActive(LocalCoopActive);
        QuickFind.GUI_MainOverview.Player2.gameObject.SetActive(LocalCoopActive);

        if (LocalCoopActive)
        {
            for (int i = 0; i < QuickFind.GUI_MainOverview.Player1.childCount; i++)
            {
                Transform Child = QuickFind.GUI_MainOverview.Player1.GetChild(i);
                Transform AltChild = QuickFind.GUI_MainOverview.Player2.GetChild(i);
                if (Child.childCount > 0)
                {
                    Transform GrandChild = Child.GetChild(0);
                    Transform AltGrandChild = AltChild.GetChild(0);

                    Vector3 Pos = AltGrandChild.localPosition;
                    float Height = AltGrandChild.GetComponent<RectTransform>().rect.height;
                    Pos.x = -Pos.x;
                    Pos.y = (Height / 2) + Pos.y;
                    GrandChild.localPosition = Pos;
                    GrandChild.localScale = AltGrandChild.localScale;
                }
            }
        }
        else
        {
            Vector3 BaseScale = new Vector3(1, 1, 1);
            for (int i = 0; i < QuickFind.GUI_MainOverview.Player1.childCount; i++)
            {
                Transform Child = QuickFind.GUI_MainOverview.Player1.GetChild(i);
                if (Child.childCount > 0)
                {
                    Transform GrandChild = Child.GetChild(0);
                    GrandChild.localPosition = Vector3.zero;
                    GrandChild.localScale = BaseScale;
                }
            }
        }
    }
}
