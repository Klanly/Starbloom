using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DG_SystemMessageGUI : MonoBehaviour {


    public class SystemMessageDisplay
    {
        public float TimeRemaining;
        public DG_SystemMessageObject Object;
        public int ItemID;
        public int value;
    }

    [Header("Ref")]
    public Transform MessageGrid = null;

    [Header("Values")]
    public float MessageDisplayTime;
    public float FadeOutTime;


    List<SystemMessageDisplay> DisplayGroup;

    [Button(ButtonSizes.Small)]
    public void DebugButton(){ GenerateSystemMessage(Random.Range(5, 7)); }





    private void Awake()
    {
        QuickFind.SystemMessageGUI = this;
        DisplayGroup = new List<SystemMessageDisplay>();
    }

    private void Start()
    {
        transform.localPosition = Vector3.zero;
        MessageGrid.GetChild(0).gameObject.SetActive(false);
    }

    private void Update()
    {
        if (DisplayGroup.Count > 0)
        {
            float DeltaTime = Time.deltaTime;
            for (int i = 0; i < DisplayGroup.Count; i++)
            {
                SystemMessageDisplay SMD = DisplayGroup[i];
                SMD.TimeRemaining -= DeltaTime;
                if (SMD.TimeRemaining < 0)
                {
                    SMD.Object.gameObject.SetActive(false);
                    DisplayGroup.Remove(SMD);
                }
                else
                    UpdateSMDAlpha(SMD);
            }
        }
        else
            this.enabled = false;
    }




    public void GenerateSystemMessage(int ItemID)
    {
        this.enabled = true;

        SystemMessageDisplay SMD = null;

        for (int i = 0; i < DisplayGroup.Count; i++)
        {

            SystemMessageDisplay InnerSMD = DisplayGroup[i];
            if (InnerSMD.ItemID != ItemID) continue;
            else
            {
                InnerSMD.value++;
                InnerSMD.TimeRemaining = MessageDisplayTime;
                SMD = InnerSMD;
                break;
            }
        }
        if(SMD == null)
        {
            SMD = new SystemMessageDisplay();
            SMD.value = 1;
            SMD.ItemID = ItemID;
            SMD.TimeRemaining = MessageDisplayTime;
            DisplayGroup.Add(SMD);
            Transform ObjectTransform;
            if (MessageGrid.childCount < DisplayGroup.Count)
            {
                ObjectTransform = Instantiate(MessageGrid.GetChild(0));
                ObjectTransform.SetParent(MessageGrid);
            }
            else
                ObjectTransform = MessageGrid.GetChild(DisplayGroup.Count - 1);

            SMD.Object = ObjectTransform.GetComponent<DG_SystemMessageObject>();        
        }
        UpdateSMDDisplay(SMD);
    }
    void UpdateSMDDisplay(SystemMessageDisplay SMD)
    {
        SMD.Object.gameObject.SetActive(true);
        SMD.Object.Canvas.alpha = 1;
        DG_ItemObject IO = QuickFind.ItemDatabase.GetItemFromID(SMD.ItemID);
        SMD.Object.Icon.sprite = IO.Icon;
        SMD.Object.DisplayNameText.text = QuickFind.WordDatabase.GetWordFromID(IO.ToolTipType.MainLocalizationID);
        if (SMD.value > 1) SMD.Object.DisplayValueText.text = SMD.value.ToString();
        else SMD.Object.DisplayValueText.text = string.Empty;
        SMD.Object.IconWobble.enabled = true;
    }
    void UpdateSMDAlpha(SystemMessageDisplay SMD)
    {
        if (SMD.TimeRemaining > FadeOutTime)
            SMD.Object.Canvas.alpha = 1;
        else
        {
            float Percent = SMD.TimeRemaining / FadeOutTime;
            SMD.Object.Canvas.alpha = Percent;
        }
    }
}
