using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_SkillTrackerItem : MonoBehaviour {

    [System.NonSerialized] public DG_SkillTracker.SkillTags SkillTag;

    public UnityEngine.UI.Image Icon;
    public TMPro.TextMeshProUGUI TitleText;
    public TMPro.TextMeshProUGUI LevelText;
    public Transform LevelGrid;
}
