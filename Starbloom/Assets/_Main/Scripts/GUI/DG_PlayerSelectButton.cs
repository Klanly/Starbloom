using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_PlayerSelectButton : MonoBehaviour {


    [System.NonSerialized] public int PlayerID;

    [Header("New Player")]
    public TMPro.TextMeshProUGUI NewPlayerText;
    [Header("Existing Player")]
    public TMPro.TextMeshProUGUI PlayerText;

    public void ButtonPressed()
    {
        QuickFind.PlayerSelectionScreen.ReturnButton(this);
    }
}
