using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_AcheivementTracker : MonoBehaviour {



    private void Awake()
    {
        QuickFind.AcheivementTracker = this;
    }







    public void CheckFishAchevement(DG_FishingRoller.FishRollValues ActiveFishReference)
    {
        DG_PlayerCharacters.CharacterAchievements Acheivements = QuickFind.Farm.PlayerCharacters[QuickFind.NetworkSync.PlayerCharacterID].Acheivements;
        int CurrentFishMax = Acheivements.LargestFishCaught[ActiveFishReference.AtlasObject.DatabaseID];
        if (CurrentFishMax < ActiveFishReference.Weight)
        {
            QuickFind.FishingGUI.DisplayFishRecordText();
            Acheivements.LargestFishCaught[ActiveFishReference.AtlasObject.DatabaseID] = ActiveFishReference.Weight;
        }
    }
}
