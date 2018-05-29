using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class DG_FishingGUI : MonoBehaviour
{

    [Header("Canvases")]
    public CanvasGroup UICanvas = null;
    public CanvasGroup ChargeBarCanvas = null;
    public CanvasGroup FishingGUICanvas = null;

    [Header("Charge Bar")]
    public Image EnergyBar;

    [Header("Event Displays")]
    public TMPro.TextMeshProUGUI MaxText = null;
    public TMPro.TextMeshProUGUI HookedText = null;
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


    private void Awake()
    {
        QuickFind.FishingGUI = this;
    }

    private void Start()
    {
        QuickFind.EnableCanvas(UICanvas, false);
        QuickFind.EnableCanvas(ChargeBarCanvas, false);
        QuickFind.EnableCanvas(FishingGUICanvas, false);
        transform.localPosition = Vector3.zero;
    }

    public void SetEnergyBar(float Value)
    {
        if (UICanvas.alpha == 0) QuickFind.EnableCanvas(UICanvas, true);
        if (ChargeBarCanvas.alpha == 0) QuickFind.EnableCanvas(ChargeBarCanvas, true);
        EnergyBar.fillAmount = Value;
    }

    public void DisableEnergyBar()
    {
        QuickFind.EnableCanvas(ChargeBarCanvas, false);
    }

    public void DisplayMaxText()
    {
        DG_UI_WobbleAndFade WobbleScript = MaxText.GetComponent<DG_UI_WobbleAndFade>();
        MaxText.GetComponent<DG_TextStatic>().ManualLoad();
        WobbleScript.enabled = true;
    }

    public void DisplayFishEventAlert()
    {
        FishTimerEventImage.enabled = true;
    }

    public void DisplayHookedText(Transform Bobber)
    {
        Vector3 screenPos = QuickFind.PlayerCam.MainCam.WorldToScreenPoint(Bobber.position);
        RectTransform HookedTextTransform = HookedText.GetComponent<RectTransform>();
        HookedTextTransform.position = screenPos;

        DG_UI_WobbleAndFade WobbleScript = HookedText.GetComponent<DG_UI_WobbleAndFade>();
        HookedText.GetComponent<DG_TextStatic>().ManualLoad();
        WobbleScript.enabled = true;
    }

    public void EnableFishingGUI()
    {
        QuickFind.EnableCanvas(ChargeBarCanvas, false);
        QuickFind.EnableCanvas(FishingGUICanvas, true);
    }

    public void SpinReel(bool isHeld)
    {
        Vector3 CurrentRotation = SpinningSprite.localEulerAngles;
        if (isHeld)
            CurrentRotation.z -= ReelinSpeed;
        else
            CurrentRotation.z += InactiveSpeed;

        SpinningSprite.localEulerAngles = CurrentRotation;
    }

    public void SetFishCaughtProgress(float ProgressLevel)
    {
        CaughtProgressBar.fillAmount = ProgressLevel;
    }

    public void SetCaptureZoneSize(float CaptureSize)
    {
        CapturePosHeight.sizeDelta = new Vector2(CapturePosHeight.rect.width, CaptureSize);
        CapturePosHeight.localPosition = new Vector3(0, (CaptureSize / 2), 0);
    }
}
