﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class DG_StorageGUI : MonoBehaviour {



    [Header("Canvases")]
    public CanvasGroup UICanvas = null;
    [Header("Transforms")]
    public RectTransform HotbarGrid = null;


    [System.NonSerialized] public NetworkObject ActiveStorage;
    [System.NonSerialized] public DG_InventoryItem[] StorageSlots;

    [System.NonSerialized] public bool isTreasureUI = false;
    [System.NonSerialized] public bool StorageUIOpen = false;


    bool Wait1 = false;

    private void Awake()
    {
        QuickFind.StorageUI = this;

        StorageSlots = new DG_InventoryItem[HotbarGrid.childCount];

        for (int i = 0; i < HotbarGrid.childCount; i++)
        {
            DG_InventoryItem II = HotbarGrid.GetChild(i).GetComponent<DG_InventoryItem>();
            II.SlotID = i;
            II.IsStorageSlot = true;
            StorageSlots[i] = II;
        }
    }
    private void Start()
    {
        QuickFind.EnableCanvas(UICanvas, false);
        transform.localPosition = Vector3.zero;
    }

    private void Update()
    {
        if (Wait1) { Wait1 = false; return; }


        if (!StorageUIOpen || QuickFind.GUI_Inventory.isFloatingInventoryItem || QuickFind.ShippingBinGUI.BinUIisOpen) return;

        bool AllowHold = false;
        if (QuickFind.GameSettings.AllowUIOnHold && QuickFind.InputController.MainPlayer.ButtonSet.Interact.Held) AllowHold = true;

        if (QuickFind.InputController.MainPlayer.ButtonSet.Interact.Up || AllowHold)
            QuickFind.InventoryManager.ShiftStackToFromStorage();
    }










    public void OpenStorageUI(NetworkObject NO, bool isTreasure)
    {
        Wait1 = true;
        StorageUIOpen = true;
        isTreasureUI = isTreasure;
        ActiveStorage = NO;
        QuickFind.EnableCanvas(UICanvas, true);
        QuickFind.GUI_Inventory.OpenStorageUI();
        UpdateStorageVisuals();
    }

    public void CloseStorageUI()
    {
        StorageUIOpen = false;
        QuickFind.EnableCanvas(UICanvas, false);
        QuickFind.GUI_Inventory.CloseStorageUI();
    }

    public void UpdateStorageVisuals()
    {
        if (ActiveStorage == null) return;

        for (int i = 0; i < ActiveStorage.StorageSlots.Length; i++)
            QuickFind.GUI_Inventory.UpdateRucksackSlotVisual(StorageSlots[i], ActiveStorage.StorageSlots[i]);
    }
}