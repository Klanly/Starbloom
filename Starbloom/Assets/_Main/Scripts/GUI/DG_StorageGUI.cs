using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class DG_StorageGUI : MonoBehaviour {


    [System.Serializable]
    public class PlayerStorageGUI
    {
        [Header("Canvases")]
        public CanvasGroup UICanvas = null;
        public UnityEngine.UI.GraphicRaycaster Raycaster;
        [Header("Transforms")]
        public RectTransform HotbarGrid = null;


        [System.NonSerialized] public NetworkObject ActiveStorage;
        [System.NonSerialized] public DG_InventoryItem[] StorageSlots;

        [System.NonSerialized] public bool isTreasureUI = false;
        [System.NonSerialized] public bool StorageUIOpen = false;

        [System.NonSerialized] public bool Wait1 = false;
    }

    public PlayerStorageGUI[] StorageGuis;


    private void Awake()
    {
        QuickFind.StorageUI = this;

        for (int iN = 0; iN < StorageGuis.Length; iN++)
        {
            PlayerStorageGUI PSG = StorageGuis[iN];

            PSG.StorageSlots = new DG_InventoryItem[PSG.HotbarGrid.childCount];

            for (int i = 0; i < PSG.HotbarGrid.childCount; i++)
            {
                DG_InventoryItem II = PSG.HotbarGrid.GetChild(i).GetComponent<DG_InventoryItem>();
                II.SlotID = i;
                II.isPlayer1 = (iN == 0);
                II.IsStorageSlot = true;
                PSG.StorageSlots[i] = II;
            }
        }
    }
    private void Start()
    {
        QuickFind.EnableCanvas(StorageGuis[0].UICanvas, false, StorageGuis[0].Raycaster);
        QuickFind.EnableCanvas(StorageGuis[1].UICanvas, false, StorageGuis[1].Raycaster);
        transform.localPosition = Vector3.zero;
    }

    private void Update()
    {
        for (int iN = 0; iN < StorageGuis.Length; iN++)
        {
            if (QuickFind.InputController.Players[iN].CharLink == null) continue;

            PlayerStorageGUI PSG = StorageGuis[iN];

            if (PSG.Wait1) { PSG.Wait1 = false; return; }

            if (!PSG.StorageUIOpen || QuickFind.GUI_Inventory.PlayersInventory[iN].isFloatingInventoryItem || QuickFind.ShippingBinGUI.ShippingGUIS[iN].BinUIisOpen) return;

            int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
            if (iN == 1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

            bool AllowHold = false;
            if (QuickFind.GameSettings.AllowUIOnHold && QuickFind.InputController.Players[iN].InteractButton == DG_GameButtons.ButtonState.Held) AllowHold = true;

            if (QuickFind.InputController.Players[iN].InteractButton == DG_GameButtons.ButtonState.Held || AllowHold)
                QuickFind.InventoryManager.ShiftStackToFromStorage(PlayerID);
        }
    }










    public void OpenStorageUI(NetworkObject NO, bool isTreasure, int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        StorageGuis[ArrayNum].Wait1 = true;
        StorageGuis[ArrayNum].StorageUIOpen = true;
        StorageGuis[ArrayNum].isTreasureUI = isTreasure;
        StorageGuis[ArrayNum].ActiveStorage = NO;
        QuickFind.EnableCanvas(StorageGuis[ArrayNum].UICanvas, true, StorageGuis[ArrayNum].Raycaster);
        QuickFind.GUI_Inventory.OpenStorageUI(PlayerID);
        UpdateStorageVisuals(PlayerID);
    }

    public void CloseStorageUI(int Index)
    {
        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (Index == 1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        StorageGuis[Index].StorageUIOpen = false;
        QuickFind.EnableCanvas(StorageGuis[Index].UICanvas, false, StorageGuis[Index].Raycaster);
        QuickFind.GUI_Inventory.CloseStorageUI(PlayerID);
    }

    public void UpdateStorageVisuals(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        PlayerStorageGUI PSG = StorageGuis[ArrayNum];

        if (PSG.ActiveStorage == null) return;

        for (int i = 0; i < PSG.ActiveStorage.StorageSlots.Length; i++)
            QuickFind.GUI_Inventory.UpdateRucksackSlotVisual(PSG.StorageSlots[i], PSG.ActiveStorage.StorageSlots[i], PlayerID);
    }
}