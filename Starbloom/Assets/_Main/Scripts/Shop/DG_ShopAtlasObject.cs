using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DG_ShopAtlasObject : MonoBehaviour {

    [HideInInspector] public int DatabaseID;
    [HideInInspector] public bool LockItem;
    public string Name;

    [ListDrawerSettings(ListElementLabelName = "Season")]
    public SeasonalSelection[] Seasons;



    [System.Serializable]
    public class SeasonalSelection
    {
        public WeatherHandler.Seasons Season;
        [ListDrawerSettings(ListElementLabelName = "Name")]
        public SeasonalGood[] Items;
    }
    [System.Serializable]
    public class SeasonalGood
    {
        [Header("---------------------------------------------------")]
        public string Name;
        public int ItemDatabaseRef;

        [Header("Situational")]
        public bool FiniteStock;
        [ShowIf("FiniteStock")]
        public int StockNumber;


#if UNITY_EDITOR
        [Button(ButtonSizes.Small)] void GoToItemDatabaseReference() { Selection.activeGameObject = QuickFindInEditor.GetEditorItemDatabase().GetItemFromID(ItemDatabaseRef).gameObject; }
        [Button(ButtonSizes.Small)] void SyncNamesToItemName() { Name = QuickFindInEditor.GetEditorItemDatabase().GetItemFromID(ItemDatabaseRef).name; }
#endif
    }
}
