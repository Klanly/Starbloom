using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_SkillsGUI : MonoBehaviour {

    [System.Serializable]
    public class SkillTagLocalization
    {
        public DG_SkillTracker.SkillTags Tag;
        public int LocalizationID;
    }
    public enum GUICatgories
    {
        NonCombat
    }

    [System.Serializable]
    public class PlayerSkillGUI
    {
        [Header("Canvases")]
        public CanvasGroup UICanvas = null;
        public UnityEngine.UI.GraphicRaycaster Raycaster;
        [Header("Reference")]
        public Transform SkillGrid = null;

        [System.NonSerialized] public GUICatgories OpenSkillTab = GUICatgories.NonCombat;
    }


    public PlayerSkillGUI[] PlayerSkills;

    [Header("Localization")]
    public SkillTagLocalization[] Localization;



    private void Awake() { QuickFind.GUI_Skills = this; }
    private void Start()
    {
        QuickFind.EnableCanvas(PlayerSkills[0].UICanvas, false, PlayerSkills[0].Raycaster);
        QuickFind.EnableCanvas(PlayerSkills[1].UICanvas, false, PlayerSkills[1].Raycaster);
        transform.localPosition = Vector3.zero;
    }



    public void OpenUI(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        QuickFind.GUI_OverviewTabs.CloseAllTabs(ArrayNum, PlayerID);
        QuickFind.EnableCanvas(PlayerSkills[ArrayNum].UICanvas, true, PlayerSkills[ArrayNum].Raycaster);
        LoadSkillGrid(PlayerID);
    }
    public void CloseUI(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;
        QuickFind.EnableCanvas(PlayerSkills[ArrayNum].UICanvas, false, PlayerSkills[ArrayNum].Raycaster);
    }



    void LoadSkillGrid(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        int index = 0;
        if (PlayerSkills[ArrayNum].OpenSkillTab == GUICatgories.NonCombat)  //Non-Combat Skills GUI Order shown
        {
            LoadSkill(DG_SkillTracker.SkillTags.Farming, index, PlayerID); index++;
            LoadSkill(DG_SkillTracker.SkillTags.Fishing, index, PlayerID); index++;
            LoadSkill(DG_SkillTracker.SkillTags.Foraging, index, PlayerID); index++;
            LoadSkill(DG_SkillTracker.SkillTags.Mining, index, PlayerID); index++;
        }
    }




    void LoadSkill(DG_SkillTracker.SkillTags SkillTag, int index, int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        Transform GridItem;
        if (index < PlayerSkills[ArrayNum].SkillGrid.childCount) GridItem = PlayerSkills[ArrayNum].SkillGrid.GetChild(index);
        else
        {
            GridItem = Instantiate(PlayerSkills[ArrayNum].SkillGrid.GetChild(0));
            GridItem.SetParent(PlayerSkills[ArrayNum].SkillGrid);
        }

        DG_SkillTrackerItem STI = GridItem.GetComponent<DG_SkillTrackerItem>();
        STI.SkillTag = SkillTag;
        DG_ItemsDatabase.GenericIconDatabaseItem GDI = QuickFind.ItemDatabase.GetGenericIconByString(SkillTag.ToString());
        STI.Icon.sprite = GDI.Icon;
        for (int i = 0; i < Localization.Length; i++)
        {
            if (Localization[i].Tag == SkillTag)
                STI.TitleText.text = QuickFind.WordDatabase.GetWordFromID(Localization[i].LocalizationID);
        }

        int SkillExp = QuickFind.SkillTracker.GetSkillExp(SkillTag, PlayerID);
        int SkillLevel = QuickFind.SkillTracker.GetSkillLevel(SkillTag, PlayerID, SkillExp);

        int Floor = 0;
        if (SkillLevel != 0) Floor = QuickFind.SkillTracker.EXPLevels[SkillLevel - 1];
        float Percentage = ((float)SkillExp - (float)Floor) / ((float)QuickFind.SkillTracker.EXPLevels[SkillLevel] - (float)Floor);
        if (Percentage > 1) Percentage = 1;

        UnityEngine.UI.Image I = STI.LevelGrid.GetChild(SkillLevel).GetChild(0).GetComponent<UnityEngine.UI.Image>();
        I.fillAmount = Percentage;
        I.color = GDI.ColorVariations[1];
        //Max All Previous Levels
        for (int i = 0; i < SkillLevel; i++)
        {
            I = STI.LevelGrid.GetChild(i).GetChild(0).GetComponent<UnityEngine.UI.Image>();
            I.fillAmount = 1;
            I.color = GDI.ColorVariations[0];
        }

        for (int i = SkillLevel + 1; i < STI.LevelGrid.childCount; i++)
            STI.LevelGrid.GetChild(i).GetChild(0).GetComponent<UnityEngine.UI.Image>().fillAmount = 0;

        STI.LevelText.text = (SkillLevel + 1).ToString();
    }

}
