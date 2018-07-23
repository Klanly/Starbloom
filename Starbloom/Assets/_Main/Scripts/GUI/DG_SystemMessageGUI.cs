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


    [System.Serializable]
    public class PlayerMessageGUI
    {
        [Header("Ref")]
        public Transform MessageGrid = null;
        [System.NonSerialized] public List<SystemMessageDisplay> DisplayGroup;
    }

    public PlayerMessageGUI[] MessageGUIs;

    [Header("Values")]
    public float MessageDisplayTime;
    public float FadeOutTime;


    [Button(ButtonSizes.Small)]
    public void DebugButton(){ GenerateSystemMessage(Random.Range(5, 7), QuickFind.NetworkSync.Player1PlayerCharacter); }





    private void Awake()
    {
        QuickFind.SystemMessageGUI = this;
        MessageGUIs[0].DisplayGroup = new List<SystemMessageDisplay>();
        MessageGUIs[1].DisplayGroup = new List<SystemMessageDisplay>();
    }

    private void Start()
    {
        transform.localPosition = Vector3.zero;

        MessageGUIs[0].MessageGrid.GetChild(0).gameObject.SetActive(false);
        MessageGUIs[1].MessageGrid.GetChild(0).gameObject.SetActive(false);
    }

    private void Update()
    {
        for (int iN = 0; iN < MessageGUIs.Length; iN++)
        {
            if (QuickFind.InputController.Players[iN].CharLink == null) continue;

            PlayerMessageGUI PMG = MessageGUIs[iN];

            if (PMG.DisplayGroup.Count > 0)
            {
                float DeltaTime = Time.deltaTime;
                for (int i = 0; i < PMG.DisplayGroup.Count; i++)
                {
                    SystemMessageDisplay SMD = PMG.DisplayGroup[i];
                    SMD.TimeRemaining -= DeltaTime;
                    if (SMD.TimeRemaining < 0)
                    {
                        SMD.Object.gameObject.SetActive(false);
                        PMG.DisplayGroup.Remove(SMD);
                    }
                    else
                        UpdateSMDAlpha(SMD);
                }
            }
            else
                this.enabled = false;
        }
    }




    public void GenerateSystemMessage(int ItemID, int PlayerID)
    {
        this.enabled = true;

        SystemMessageDisplay SMD = null;

        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        PlayerMessageGUI PMG = MessageGUIs[ArrayNum];

        for (int i = 0; i < PMG.DisplayGroup.Count; i++)
        {

            SystemMessageDisplay InnerSMD = PMG.DisplayGroup[i];
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
            PMG.DisplayGroup.Add(SMD);
            Transform ObjectTransform;
            if (PMG.MessageGrid.childCount < PMG.DisplayGroup.Count)
            {
                ObjectTransform = Instantiate(PMG.MessageGrid.GetChild(0));
                ObjectTransform.SetParent(PMG.MessageGrid);
            }
            else
                ObjectTransform = PMG.MessageGrid.GetChild(PMG.DisplayGroup.Count - 1);

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
