using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_BreakableObjectItem : MonoBehaviour {

    public enum OnHitEffectType
    {
        Stone,
        Tree,
        Plant
    }

    [System.Serializable]
    public class ItemRoll
    {
        [Header("-----------------------------------------------------------")]
        public float RollPercent;
        public ItemClump[] ItemGifts;    
    }
    [System.Serializable]
    public class ItemClump
    {
        public int ItemID;
        public int ItemQuality;
        public int Value;
    }


    [HideInInspector] public int DatabaseID;
    [HideInInspector] public bool LockItem;
    

    public string Name;

    public ItemRoll[] RewardRolls;




    public ItemClump[] GetBreakReward()
    {
        float RewardRoll = Random.Range(0f, 1f);
        for (int i = 0; i < RewardRolls.Length; i++)
            if (RewardRoll < RewardRolls[i].RollPercent) return RewardRolls[i].ItemGifts;

        Debug.Log("Something wrong happened");
        return null;
    }
}
