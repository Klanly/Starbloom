using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class DG_FishingGUI : MonoBehaviour
{

    [Header("Canvases")]
    public CanvasGroup ChargeBarCanvas = null;
    public CanvasGroup FishingGUICanvas = null;
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
    public float ReelinSpeed;
    public float InactiveSpeed;

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


    private void Awake()
    {
        QuickFind.FishingGUI = this;
    }

    private void Start()
    {
        QuickFind.EnableCanvas(ChargeBarCanvas, false);
        QuickFind.EnableCanvas(FishingGUICanvas, false);
        QuickFind.EnableCanvas(CaughtDisplay, false);
        transform.localPosition = Vector3.zero;
    }


    public void EnableFishingGUI(){QuickFind.EnableCanvas(ChargeBarCanvas, false);QuickFind.EnableCanvas(FishingGUICanvas, true);}
    public void CloseCaughtGui(){QuickFind.EnableCanvas(CaughtDisplay, false);}


    public void DisplayMaxText() { DisplayWobbleText(MaxText); }
    public void DisplayFishRecordText() { DisplayWobbleText(NewRecordText); }

    public void SetEnergyBar(float Value){if (ChargeBarCanvas.alpha == 0) QuickFind.EnableCanvas(ChargeBarCanvas, true);EnergyBar.fillAmount = Value;}
    public void DisableEnergyBar(){QuickFind.EnableCanvas(ChargeBarCanvas, false);}
    public void DisplayFishEventAlert() { FishTimerEventImage.enabled = true; }
    public void SetFishCaughtProgress(float ProgressLevel){CaughtProgressBar.fillAmount = ProgressLevel;}




    public void DisplayHookedText(Transform Bobber, int PlayerID)
    {
        Vector3 screenPos = QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID).PlayerCam.MainCam.WorldToScreenPoint(Bobber.position);
        RectTransform HookedTextTransform = HookedText.GetComponent<RectTransform>();
        HookedTextTransform.position = screenPos;
        DisplayWobbleText(HookedText);
    }

    public void SpinReel(bool isHeld)
    {
        Vector3 CurrentRotation = SpinningSprite.localEulerAngles;
        if (isHeld) CurrentRotation.z -= ReelinSpeed;
        else CurrentRotation.z += InactiveSpeed;
        SpinningSprite.localEulerAngles = CurrentRotation;
    }

    public void SetCaptureZoneSize(float CaptureSize)
    {
        CapturePosHeight.sizeDelta = new Vector2(CapturePosHeight.rect.width, CaptureSize);
        CapturePosHeight.localPosition = new Vector3(0, (CaptureSize / 2), 0);
    }

    public void OpenObjectCaughtGUI(Sprite ObjectSprite, string ObjectName, string ObjectLength)
    {
        QuickFind.EnableCanvas(FishingGUICanvas, false);
        QuickFind.EnableCanvas(CaughtDisplay, true);
        CaughtIcon.sprite = ObjectSprite;
        FishName.text = ObjectName;
        FishLength.text = ObjectLength;
    }


    public void DisplayWobbleText(TMPro.TextMeshProUGUI TextObject)
    {
        DG_UI_WobbleAndFade WobbleScript = TextObject.GetComponent<DG_UI_WobbleAndFade>();
        TextObject.GetComponent<DG_TextStatic>().ManualLoad();
        WobbleScript.enabled = true;
    }
}
