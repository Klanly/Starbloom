using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class DG_FishingGUI : MonoBehaviour
{
    [System.Serializable]
    public class PlayerFishingGUI
    {
        [Header("Canvases")]
        public CanvasGroup ChargeBarCanvas = null;
        public CanvasGroup FishingGUICanvas = null;
        public UnityEngine.UI.GraphicRaycaster Raycaster;
        public CanvasGroup CaughtDisplay = null;

        [Header("Charge Bar")]
        public Image EnergyBar;

        [Header("Event Displays")]
        public TMPro.TextMeshProUGUI MaxText = null;
        public TMPro.TextMeshProUGUI HookedText = null;
        public TMPro.TextMeshProUGUI NewRecordText = null;
        public DG_UI_WobbleAndFade FishTimerEventImage = null;

        [Header("Spinner")]
        public RectTransform SpinningSprite = null;

        [Header("Fishing Region Stuff")]
        public RectTransform FishRegionMain;
        public RectTransform Fish;
        public RectTransform CaptureZonePos;
        public RectTransform CapturePosHeight;

        [Header("Catch Progress Bar")]
        public Image CaughtProgressBar = null;

        [Header("Caught Display")]
        public Image CaughtIcon = null;
        public TMPro.TextMeshProUGUI FishName = null;
        public TMPro.TextMeshProUGUI FishLength = null;
    }

    [Header("Spinner")]
    public float ReelinSpeed;
    public float InactiveSpeed;

    public PlayerFishingGUI[] FishingGuis;


    private void Awake()
    {
        QuickFind.FishingGUI = this;
    }

    private void Start()
    {
        QuickFind.EnableCanvas(FishingGuis[0].ChargeBarCanvas, false, null);
        QuickFind.EnableCanvas(FishingGuis[0].FishingGUICanvas, false, FishingGuis[0].Raycaster);
        QuickFind.EnableCanvas(FishingGuis[0].CaughtDisplay, false, null);
        QuickFind.EnableCanvas(FishingGuis[1].ChargeBarCanvas, false, null);
        QuickFind.EnableCanvas(FishingGuis[1].FishingGUICanvas, false, FishingGuis[1].Raycaster);
        QuickFind.EnableCanvas(FishingGuis[1].CaughtDisplay, false, null);
        transform.localPosition = Vector3.zero;
    }


    public void EnableFishingGUI(int PlayerID)
    {
        int ArrayNum = 0; if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;
        QuickFind.EnableCanvas(FishingGuis[ArrayNum].ChargeBarCanvas, false, null);
        QuickFind.EnableCanvas(FishingGuis[ArrayNum].FishingGUICanvas, true, FishingGuis[ArrayNum].Raycaster);
    }
    public void CloseCaughtGui(int PlayerID) { int ArrayNum = 0; if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1; QuickFind.EnableCanvas(FishingGuis[ArrayNum].CaughtDisplay, false, null); }

    public void DisplayMaxText(int PlayerID) { int ArrayNum = 0; if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1; DisplayWobbleText(FishingGuis[ArrayNum].MaxText); }
    public void DisplayFishRecordText(int PlayerID) { int ArrayNum = 0; if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1; DisplayWobbleText(FishingGuis[ArrayNum].NewRecordText); }

    public void SetEnergyBar(float Value, int PlayerID) { int ArrayNum = 0; if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1; if (FishingGuis[ArrayNum].ChargeBarCanvas.alpha == 0) QuickFind.EnableCanvas(FishingGuis[ArrayNum].ChargeBarCanvas, true, null); FishingGuis[ArrayNum].EnergyBar.fillAmount = Value;}
    public void DisableEnergyBar(int PlayerID) { int ArrayNum = 0; if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1; QuickFind.EnableCanvas(FishingGuis[ArrayNum].ChargeBarCanvas, false, null);}
    public void DisplayFishEventAlert(int PlayerID) { int ArrayNum = 0; if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1; FishingGuis[ArrayNum].FishTimerEventImage.enabled = true; }
    public void SetFishCaughtProgress(float ProgressLevel, int PlayerID) { int ArrayNum = 0; if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1; FishingGuis[ArrayNum].CaughtProgressBar.fillAmount = ProgressLevel;}




    public void DisplayHookedText(Transform Bobber, int PlayerID)
    {
        int ArrayNum = 0; if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;
        Vector3 screenPos = QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID).PlayerCam.MainCam.WorldToScreenPoint(Bobber.position);
        RectTransform HookedTextTransform = FishingGuis[ArrayNum].HookedText.GetComponent<RectTransform>();
        HookedTextTransform.position = screenPos;
        DisplayWobbleText(FishingGuis[ArrayNum].HookedText);
    }

    public void SpinReel(bool isHeld, int PlayerID)
    {
        int ArrayNum = 0; if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;
        Vector3 CurrentRotation = FishingGuis[ArrayNum].SpinningSprite.localEulerAngles;
        if (isHeld) CurrentRotation.z -= ReelinSpeed;
        else CurrentRotation.z += InactiveSpeed;
        FishingGuis[ArrayNum].SpinningSprite.localEulerAngles = CurrentRotation;
    }

    public void SetCaptureZoneSize(float CaptureSize, int PlayerID)
    {
        int ArrayNum = 0; if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;
        FishingGuis[ArrayNum].CapturePosHeight.sizeDelta = new Vector2(FishingGuis[ArrayNum].CapturePosHeight.rect.width, CaptureSize);
        FishingGuis[ArrayNum].CapturePosHeight.localPosition = new Vector3(0, (CaptureSize / 2), 0);
    }

    public void OpenObjectCaughtGUI(Sprite ObjectSprite, string ObjectName, string ObjectLength, int PlayerID)
    {
        int ArrayNum = 0; if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;
        QuickFind.EnableCanvas(FishingGuis[ArrayNum].FishingGUICanvas, false, FishingGuis[ArrayNum].Raycaster);
        QuickFind.EnableCanvas(FishingGuis[ArrayNum].CaughtDisplay, true, null);
        FishingGuis[ArrayNum].CaughtIcon.sprite = ObjectSprite;
        FishingGuis[ArrayNum].FishName.text = ObjectName;
        FishingGuis[ArrayNum].FishLength.text = ObjectLength;
    }


    public void DisplayWobbleText(TMPro.TextMeshProUGUI TextObject)
    {
        DG_UI_WobbleAndFade WobbleScript = TextObject.GetComponent<DG_UI_WobbleAndFade>();
        TextObject.GetComponent<DG_TextStatic>().ManualLoad();
        WobbleScript.enabled = true;
    }
}
