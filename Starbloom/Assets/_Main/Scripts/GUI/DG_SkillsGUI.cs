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




    [Header("Canvases")]
    public CanvasGroup UICanvas = null;
    [Header("Reference")]
    public Transform SkillGrid = null;
    [Header("Localization")]
    public SkillTagLocalization[] Localization;

    [System.NonSerialized] public GUICatgories OpenSkillTab = GUICatgories.NonCombat;



    private void Awake() { QuickFind.GUI_Skills = this; }
    private void Start() { QuickFind.EnableCanvas(UICanvas, false); transform.localPosition = Vector3.zero; }



    public void OpenUI()
    {
        QuickFind.GUI_OverviewTabs.CloseAllTabs();
        QuickFind.EnableCanvas(UICanvas, true);
        LoadSkillGrid();
    }
    public void CloseUI()
    {
        QuickFind.EnableCanvas(UICanvas, false);
    }



    void LoadSkillGrid()
    {
        int index = 0;
        if (OpenSkillTab == GUICatgories.NonCombat)  //Non-Combat Skills GUI Order shown
        {
            LoadSkill(DG_SkillTracker.SkillTags.Farming, index); index++;
            LoadSkill(DG_SkillTracker.SkillTags.Fishing, index); index++;
            LoadSkill(DG_SkillTracker.SkillTags.Foraging, index); index++;
            LoadSkill(DG_SkillTracker.SkillTags.Mining, index); index++;
        }
    }




    void LoadSkill(DG_SkillTracker.SkillTags SkillTag, int index)
    {
        Transform GridItem;
        if (index < SkillGrid.childCount) GridItem = SkillGrid.GetChild(index);
        else
        {
            GridItem = Instantiate(SkillGrid.GetChild(0));
            GridItem.SetParent(SkillGrid);
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

        int PlayerID = QuickFind.NetworkSync.PlayerCharacterID;
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
