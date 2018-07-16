using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiMainGameplay : MonoBehaviour {


    [Header("Canvases")]
    public CanvasGroup UICanvas = null;
    public CanvasGroup InventoryHotbar = null;

    [Header("Inventory Hotbar")]
    public Transform InventoryHotbarGrid = null;

    [Header("Money Grid")]
    public Transform MoneyGrid = null;
    public float NewDayCycleTime;
    public float CycleTime;
    TMPro.TextMeshProUGUI[] TextDisplays;
    bool CyclingMoney = false;
    float CycleTimer;
    float CycleTimeMax;
    int ToMoney;
    int FromMoney;

    [Header("Time")]
    public TMPro.TextMeshProUGUI SeasonText = null;
    public TMPro.TextMeshProUGUI DayText = null;
    public TMPro.TextMeshProUGUI TimeText = null;
    public DG_TextStatic StaticText = null;

    [Header("Energy/Health/Mana Bars")]
    public UnityEngine.UI.Image EnergyBar;
    public UnityEngine.UI.Image HealthBar;





    private void Awake()
    {
        QuickFind.GUI_MainOverview = this;
        TextDisplays = new TMPro.TextMeshProUGUI[MoneyGrid.childCount];
        for (int i = 0; i < TextDisplays.Length; i++)
            TextDisplays[i] = MoneyGrid.GetChild(i).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
    }
    private void Start()
    {
        QuickFind.EnableCanvas(UICanvas, false);
        transform.localPosition = Vector3.zero;
        this.enabled = false;
    }

    private void Update()
    {
        float UpdateTime = Time.deltaTime;
        if(CyclingMoney)
        {
            if (MoneyRollComplete(UpdateTime))
                CyclingMoney = false;
        }

        if (!CyclingMoney)
            this.enabled = false;
    }




    public void OpenUI(bool UIisOpen) { QuickFind.EnableCanvas(UICanvas, UIisOpen); }



    //Money Display
    public void SetMoneyValue(int PreviousMoneyValue, int NewMoneyValueToDisplay, bool isNewDay)
    {
        FromMoney = PreviousMoneyValue;
        ToMoney = NewMoneyValueToDisplay;
        if (isNewDay) { CycleTimeMax = NewDayCycleTime; CycleTimer = NewDayCycleTime; }
        else { CycleTimeMax = CycleTime; CycleTimer = CycleTime; }
        CyclingMoney = true;
        this.enabled = true;
    }
    bool MoneyRollComplete(float UpdateTime)
    {
        bool isFinal = false;

        CycleTimer -= UpdateTime;
        float Percentage = 1 - (CycleTimer / CycleTimeMax);
        if(Percentage > 1)
        {
            isFinal = true;
            Percentage = 1;
        }

        float NewValue = ((float)ToMoney - (float)FromMoney);
        float NewValPercent = NewValue * Percentage;
        int AddValue = Mathf.RoundToInt(NewValPercent);
        int FinalValue = FromMoney + AddValue;
        string FinalValueString = FinalValue.ToString();
        char[] ValueArray = FinalValueString.ToCharArray();
        for (int i = 0; i < TextDisplays.Length; i++)
        {
            if (i < ValueArray.Length)
            {
                int TextArrayValue = (ValueArray.Length - 1) - i;
                TextDisplays[i].text = ValueArray[TextArrayValue].ToString();
            }
            else
                TextDisplays[i].text = string.Empty;
        }

        return isFinal;
    }






    public void SetGuiDayValue(int Month, int Day)
    {
        switch (Month)
        {
            case 1: SeasonText.text = QuickFind.WordDatabase.GetWordFromID(115); break;
            case 2: SeasonText.text = QuickFind.WordDatabase.GetWordFromID(116); break;
            case 3: SeasonText.text = QuickFind.WordDatabase.GetWordFromID(117); break;
            case 4: SeasonText.text = QuickFind.WordDatabase.GetWordFromID(118); break;
        }
        DayText.text = Day.ToString();
    }
    public void SetGuiTimeValue(int Hour, int Minute)
    {
        StaticText.ManualLoad();
        if (Hour > 12)
            Hour = Hour - 12;
        string MinuteString = Minute.ToString();
        if (MinuteString == "0")
            MinuteString = "00";

        TimeText.text = Hour.ToString() + ":" + MinuteString;
    }





    public void SetGuiEnergyValue(float NewEnergyPercentage)
    {
        EnergyBar.fillAmount = NewEnergyPercentage;
    }

    public void SetGuiHealthValue(float NewHealthPercentage)
    {
        HealthBar.fillAmount = NewHealthPercentage;
    }

}
