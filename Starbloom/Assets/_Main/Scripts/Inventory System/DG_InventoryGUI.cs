using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DG_InventoryGUI : MonoBehaviour
{

    [Header("Null Item")]
    public Sprite DefaultNullSprite = null;


    [System.Serializable]
    public class PlayerInventory
    {
        [Header("Canvases")]
        public CanvasGroup UICanvas = null;
        public UnityEngine.UI.GraphicRaycaster Raycaster;
        [Header("Grid Parents")]
        public Transform HotbarGrid;
        public Transform NonHotbarGrid;
        public Transform HotbarMirrorGrid;

        [Header("Floating Inventory Item")]
        public RectTransform FloatingRect;
        public DG_InventoryItem FloatingItem;

        [Header("Shifing UI Position")]
        public RectTransform InventoryFrame;
        public Vector3 StoragePosition;
        public Vector3 ShopPosition;
        public Vector3 ShippingBinPosition;
        [System.NonSerialized] public Vector3 StartingPosition;

        [System.NonSerialized] public bool InventoryIsOpen;
        [System.NonSerialized] public bool OuterInventoryIsOpen;


        [System.NonSerialized] public DG_InventoryItem[] GuiItemSlots;
        [System.NonSerialized] public DG_InventoryItem[] HotbarSlots;

        //InventorySwap Stuff
        [System.NonSerialized] public DG_InventoryItem CurrentHoverItem;
        [System.NonSerialized] public bool isFloatingInventoryItem;
        [System.NonSerialized] public DG_InventoryItem PickedUpItemSlot;

        //Equiped Hotbar Slot
        [System.NonSerialized] public int EquippedHotbarSlot = 0;
    }

    public PlayerInventory[] PlayersInventory;


    private void Awake()
    {
        QuickFind.GUI_Inventory = this;

        for (int iN = 0; iN < PlayersInventory.Length; iN++)
        {
            PlayerInventory PI = PlayersInventory[iN];

            bool isPlayer1 = (iN == 0);

            int index = 0;
            int Count = PI.HotbarGrid.childCount + PI.NonHotbarGrid.childCount;
            PI.GuiItemSlots = new DG_InventoryItem[Count];
            for (int i = 0; i < PI.HotbarGrid.childCount; i++)
            { PI.GuiItemSlots[index] = PI.HotbarGrid.GetChild(i).GetComponent<DG_InventoryItem>(); PI.GuiItemSlots[index].SlotID = index; PI.GuiItemSlots[index].isPlayer1 = isPlayer1; index++; }
            for (int i = 0; i < PI.NonHotbarGrid.childCount; i++)
            { PI.GuiItemSlots[index] = PI.NonHotbarGrid.GetChild(i).GetComponent<DG_InventoryItem>(); PI.GuiItemSlots[index].SlotID = index; PI.GuiItemSlots[index].isPlayer1 = isPlayer1; index++; }

            PI.HotbarSlots = new DG_InventoryItem[PI.HotbarMirrorGrid.childCount];
            for (int i = 0; i < PI.HotbarSlots.Length; i++)
            { PI.HotbarSlots[i] = PI.HotbarMirrorGrid.GetChild(i).GetComponent<DG_InventoryItem>(); PI.HotbarSlots[i].isMirror = true; PI.HotbarSlots[i].SlotID = i; PI.HotbarSlots[i].Disabled.enabled = false; PI.HotbarSlots[i].isPlayer1 = isPlayer1; }

            PI.StartingPosition = PI.InventoryFrame.localPosition;
        }
    }



    private void Start()
    {
        transform.localPosition = Vector3.zero;

        for (int iN = 0; iN < PlayersInventory.Length; iN++)
        {
            PlayerInventory PI = PlayersInventory[iN];

            bool isPlayer1 = (iN == 0);
            QuickFind.EnableCanvas(PI.UICanvas, false, PI.Raycaster);
            AdjustFloatingInventoryUI(false, isPlayer1);
        }
    }

    private void Update()
    {
        for (int iN = 0; iN < PlayersInventory.Length; iN++)
        {
            if (QuickFind.InputController.Players[iN].CharLink == null) continue;

            PlayerInventory PI = PlayersInventory[iN];

            bool isPlayer1 = (iN == 0);

            int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
            if (!isPlayer1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

            if (PI.isFloatingInventoryItem)
                HandleFloatingInventoryItem(PlayerID);

            if (QuickFind.NetworkSync != null)
            {
                if (!PI.InventoryIsOpen)
                {


                    //float ZoomAxis = QuickFind.InputController.GetPlayerByPlayerID(PlayerID).CamZoomAxis;
                    //if (ZoomAxis != 0 && !QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID).AnimationSync.MidAnimation)
                    //{
                    //    bool Add = false;
                    //    if (ZoomAxis < 0)
                    //        Add = true;
                    //    int NextEquippedHotbarSlot = QuickFind.GetValueInArrayLoop(PI.EquippedHotbarSlot, PI.HotbarSlots.Length, Add, true);
                    //    SetHotbarSlot(PI.HotbarSlots[NextEquippedHotbarSlot]);
                    //}
                }
                else
                {
                    bool Quality = QuickFind.TooltipHandler.TooltipGuis[iN].IsQualitySelection;
                    if (Quality)
                    {
                        //float ZoomAxis = QuickFind.InputController.GetPlayerByPlayerID(PlayerID).CamZoomAxis;
                        //if (ZoomAxis != 0)
                        //    QuickFind.TooltipHandler.UpdateEquippedNum(-(int)ZoomAxis, false, PlayerID);
                    }
                    if (QuickFind.InputController.Players[iN].ToolButton == DG_GameButtons.ButtonState.Up && !QuickFind.ShippingBinGUI.ShippingGUIS[iN].BinUIisOpen)
                    {
                        int Playerindex = 0;
                        if (PlayerID != QuickFind.NetworkSync.Player1PlayerCharacter) Playerindex = 1;

                        if (PI.isFloatingInventoryItem && PI.CurrentHoverItem != null)
                            QuickFind.InventoryManager.DropOne(PI.PickedUpItemSlot, PI.CurrentHoverItem, PlayerID);
                        else if (!QuickFind.StorageUI.StorageGuis[Playerindex].StorageUIOpen && Quality)
                            QuickFind.TooltipHandler.UpdateEquippedNum(1, true, PlayerID);
                    }
                }
            }
        }
    }


    public void OpenUI(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        QuickFind.GUI_OverviewTabs.CloseAllTabs(ArrayNum, PlayerID);
        QuickFind.EnableCanvas(PlayersInventory[ArrayNum].UICanvas, true, PlayersInventory[ArrayNum].Raycaster);
        UpdateInventoryVisuals(PlayerID);
        PlayersInventory[ArrayNum].InventoryIsOpen = true;
    }
    public void CloseUI(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        PlayersInventory[ArrayNum].InventoryIsOpen = false;
        QuickFind.EnableCanvas(PlayersInventory[ArrayNum].UICanvas, false, PlayersInventory[ArrayNum].Raycaster);
    }



    public void OpenStorageUI(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;
        PlayersInventory[ArrayNum].InventoryFrame.localPosition = PlayersInventory[ArrayNum].StoragePosition; OpenOuterUI(PlayerID);
    }
    public void OpenShopUI(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;
        PlayersInventory[ArrayNum].InventoryFrame.localPosition = PlayersInventory[ArrayNum].ShopPosition; OpenOuterUI(PlayerID);
    }
    public void OpenShippingUI(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;
        PlayersInventory[ArrayNum].InventoryFrame.localPosition = PlayersInventory[ArrayNum].ShippingBinPosition; OpenOuterUI(PlayerID);
    }

    public void CloseStorageUI(int PlayerID) { CloseOuterUI(PlayerID); }
    public void CloseShopUI(int PlayerID) { CloseOuterUI(PlayerID); }
    public void CloseShippingUI(int PlayerID) { CloseOuterUI(PlayerID); }

    void OpenOuterUI(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;
        DG_CharacterLink CharLink = QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID);

        QuickFind.PlayerCam.EnableCamera(CharLink.PlayerCam, false, true);

        QuickFind.ContextDetectionHandler.Contexts[ArrayNum].COEncountered = null;
        QuickFind.ContextDetectionHandler.Contexts[ArrayNum].LastEncounteredContext = null;
        QuickFind.EnableCanvas(PlayersInventory[ArrayNum].UICanvas, true, PlayersInventory[ArrayNum].Raycaster);
        UpdateInventoryVisuals(PlayerID);
        PlayersInventory[ArrayNum].InventoryIsOpen = true;
        PlayersInventory[ArrayNum].OuterInventoryIsOpen = true;
    }
    void CloseOuterUI(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        DG_CharacterLink CharLink = QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID);

        QuickFind.PlayerCam.EnableCamera(CharLink.PlayerCam, true);

        ClearFloatingObject(PlayerID);
        QuickFind.TooltipHandler.HideToolTip(PlayerID);

        PlayersInventory[ArrayNum].InventoryFrame.localPosition = PlayersInventory[ArrayNum].StartingPosition;
        PlayersInventory[ArrayNum].InventoryIsOpen = false;
        PlayersInventory[ArrayNum].OuterInventoryIsOpen = false;
        QuickFind.EnableCanvas(PlayersInventory[ArrayNum].UICanvas, false, PlayersInventory[ArrayNum].Raycaster);
    }







    public void UpdateInventoryVisuals(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        DG_PlayerCharacters.CharacterEquipment Equipment = QuickFind.Farm.PlayerCharacters[PlayerID].Equipment;
        for (int i = 0; i < Equipment.RucksackSlots.Length; i++)
        {
            if (i < Equipment.RuckSackUnlockedSize)
                UpdateRucksackSlotVisual(PlayersInventory[ArrayNum].GuiItemSlots[i], Equipment.RucksackSlots[i], PlayerID);
            else //Disable Locked Rucksack Slots
                PlayersInventory[ArrayNum].GuiItemSlots[i].Disabled.enabled = true;
        }
        UpdateMirrorGrid(PlayerID);
        UpdateFloatingItemVisuals(PlayerID);
    }
    public void UpdateRucksackSlotVisual(DG_InventoryItem GuiSlot, DG_PlayerCharacters.RucksackSlot RucksackSlot, int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        if (PlayersInventory[ArrayNum].isFloatingInventoryItem){if (PlayersInventory[ArrayNum].PickedUpItemSlot == GuiSlot) { return; }}

        GuiSlot.Disabled.enabled = false;
        if (RucksackSlot.GetStackValue() == 0)
        {
            GuiSlot.ContainsItem = false;

            GuiSlot.isPlayer1 = (ArrayNum == 0);
            GuiSlot.Icon.sprite = DefaultNullSprite;
            GuiSlot.AmountText.text = string.Empty;
            GuiSlot.QualityAmountText.text = string.Empty;
            GuiSlot.QualityLevelOverlay.enabled = false;
        }
        else
        {
            DG_ItemObject Object = QuickFind.ItemDatabase.GetItemFromID(RucksackSlot.ContainedItem);
            GuiSlot.ContainsItem = true;
            GuiSlot.Icon.sprite = Object.GetItemSpriteByQuality(RucksackSlot.CurrentStackActive);
            if (RucksackSlot.GetStackValue() > 1)
            {
                GuiSlot.AmountText.text = RucksackSlot.GetStackValue().ToString();
                GuiSlot.QualityAmountText.text = RucksackSlot.GetNumberOfQuality((DG_ItemObject.ItemQualityLevels)RucksackSlot.CurrentStackActive).ToString();
            }
            else
            {
                GuiSlot.AmountText.text = string.Empty;
                GuiSlot.QualityAmountText.text = string.Empty;
            }

            if (Object.MaxStackSize > 1)
            {
                GuiSlot.QualityLevelOverlay.enabled = true;
                DG_ItemsDatabase.GenericIconDatabaseItem ICD = QuickFind.ItemDatabase.GetGenericIconByString("QualityMarker");
                GuiSlot.QualityLevelOverlay.sprite = ICD.Icon;
                GuiSlot.QualityLevelOverlay.color = ICD.ColorVariations[RucksackSlot.CurrentStackActive];
            }
            else
                GuiSlot.QualityLevelOverlay.enabled = false;
        }
    }


    public void UpdateMirrorGrid(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        for (int i = 0; i < PlayersInventory[ArrayNum].HotbarMirrorGrid.childCount; i++)
            UpdateMirrorSlot(i, ArrayNum);
    }
    public void UpdateMirrorSlot(int i, int ArrayNum)
    {
        DG_InventoryItem Main = PlayersInventory[ArrayNum].GuiItemSlots[i];
        DG_InventoryItem Mirror = PlayersInventory[ArrayNum].HotbarMirrorGrid.GetChild(i).GetComponent<DG_InventoryItem>();
        Mirror.Icon.sprite = Main.Icon.sprite;
        Mirror.AmountText.text = Main.AmountText.text;
        Mirror.ContainsItem = Main.ContainsItem;

        Mirror.QualityLevelOverlay.enabled = Main.QualityLevelOverlay.isActiveAndEnabled;
        Mirror.QualityLevelOverlay.sprite = Main.QualityLevelOverlay.sprite;
        Mirror.QualityLevelOverlay.color = Main.QualityLevelOverlay.color;
        Mirror.QualityAmountText.text = Main.QualityAmountText.text;
    }







    public void InventoryItemPressed(DG_InventoryItem PressedItem)
    {
        //This is a mirror Item, and This is for Setting Current Equipped Item
        if (PressedItem.isMirror) SetHotbarSlot(PressedItem);

        //This is an Inventory Item, and we want to handle user moving item somewhere.
        else
        {
            int ArrayNum = 0;
            if (!PressedItem.isPlayer1) ArrayNum = 1;

            int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
            if (!PressedItem.isPlayer1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

            if (PlayersInventory[ArrayNum].isFloatingInventoryItem) //We already have an item held, so we must Swap Items
            {
                AdjustFloatingInventoryUI(false, PressedItem.isPlayer1);

                DG_PlayerCharacters.RucksackSlot RucksackSlotA = QuickFind.InventoryManager.GetRuckSackSlotInventoryItem(PlayersInventory[ArrayNum].PickedUpItemSlot, PlayerID);
                DG_PlayerCharacters.RucksackSlot RucksackSlotB = QuickFind.InventoryManager.GetRuckSackSlotInventoryItem(PressedItem, PlayerID);
                int IndexA = PlayerID;
                int IndexB = PlayerID;

                int Playerindex = 0;
                if (PlayerID != QuickFind.NetworkSync.Player1PlayerCharacter) Playerindex = 1;

                if (PlayersInventory[ArrayNum].PickedUpItemSlot.IsStorageSlot) IndexA = QuickFind.StorageUI.StorageGuis[Playerindex].ActiveStorage.transform.GetSiblingIndex();
                if (PressedItem.IsStorageSlot) IndexB = QuickFind.StorageUI.StorageGuis[Playerindex].ActiveStorage.transform.GetSiblingIndex();

                if(RucksackSlotA.ContainedItem != RucksackSlotB.ContainedItem)
                    QuickFind.InventoryManager.SwapInventoryItems(RucksackSlotA, RucksackSlotB, PlayersInventory[ArrayNum].PickedUpItemSlot, PressedItem, IndexA, IndexB, PlayerID);
                else
                    QuickFind.InventoryManager.DropStackOntoStack(RucksackSlotA, RucksackSlotB, PlayersInventory[ArrayNum].PickedUpItemSlot, PressedItem, IndexA, IndexB, PlayerID);

                PressedItem.ItemHoverIn();
            }
            else
            {
                DG_PlayerCharacters.RucksackSlot RucksackSlotA = QuickFind.InventoryManager.GetRuckSackSlotInventoryItem(PressedItem, PlayerID);
                if (RucksackSlotA.GetStackValue() != 0)
                { AdjustFloatingInventoryUI(true, PressedItem.isPlayer1); PickupItem(PressedItem); }
            }
        }
    }



    public void ClearFloatingObject(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        PlayersInventory[ArrayNum].PickedUpItemSlot = null;
        PlayersInventory[ArrayNum].isFloatingInventoryItem = false;
        PlayersInventory[ArrayNum].FloatingRect.position = new Vector3(8000, 0, 0);
    }


    void UpdateFloatingItemVisuals(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        if (PlayersInventory[ArrayNum].PickedUpItemSlot == null) return;

        DG_PlayerCharacters.RucksackSlot RucksackSlot = QuickFind.InventoryManager.GetRuckSackSlotInventoryItem(PlayersInventory[ArrayNum].PickedUpItemSlot, PlayerID);
        DG_ItemObject Object = QuickFind.ItemDatabase.GetItemFromID(RucksackSlot.ContainedItem);
        PlayersInventory[ArrayNum].FloatingItem.AmountText.text = RucksackSlot.GetStackValue().ToString();
        PlayersInventory[ArrayNum].FloatingItem.Icon.sprite = Object.GetItemSpriteByQuality(RucksackSlot.CurrentStackActive);
    }






    void PickupItem(DG_InventoryItem PressedItem)
    {
        if (PressedItem.isTrash) { return; }

        int ArrayNum = 0;
        if (!PressedItem.isPlayer1) ArrayNum = 1;

        PlayersInventory[ArrayNum].PickedUpItemSlot = PressedItem;

        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if(!PressedItem.isPlayer1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        UpdateFloatingItemVisuals(PlayerID);

        PressedItem.AmountText.text = string.Empty;
        PressedItem.Icon.sprite = DefaultNullSprite;
        PressedItem.QualityLevelOverlay.enabled = false;
        PressedItem.QualityAmountText.text = string.Empty;
    }
    void AdjustFloatingInventoryUI(bool Enabled, bool isPlayer1)
    {
        int ArrayNum = 0;
        if (!isPlayer1) ArrayNum = 1;

        if (PlayersInventory[ArrayNum].CurrentHoverItem != null && PlayersInventory[ArrayNum].CurrentHoverItem.isTrash) return;

        PlayersInventory[ArrayNum].isFloatingInventoryItem = Enabled;
        if (QuickFind.ShippingBinGUI.ShippingGUIS[ArrayNum].BinUIisOpen) QuickFind.ShippingBinGUI.OpenDropPanel(Enabled, isPlayer1);
        if (!Enabled)
            PlayersInventory[ArrayNum].FloatingRect.position = new Vector3(8000, 0, 0);
    }

    void HandleFloatingInventoryItem(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        PlayersInventory[ArrayNum].FloatingRect.position = Input.mousePosition;
    }





    public void ResetHotbarSlot(int PlayerID)
    {
        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        SetHotbarSlot(PlayersInventory[ArrayNum].HotbarSlots[PlayersInventory[ArrayNum].EquippedHotbarSlot]);
    }


    public void SetHotbarSlot(DG_InventoryItem PressedItem)
    {
        int PlayerID = QuickFind.NetworkSync.Player1PlayerCharacter;
        if (!PressedItem.isPlayer1) PlayerID = QuickFind.NetworkSync.Player2PlayerCharacter;

        if (QuickFind.NetworkSync.GetCharacterLinkByPlayerID(PlayerID).AnimationSync.DisableWeaponSwitching) return;

        int ArrayNum = 0;
        if (PlayerID == QuickFind.NetworkSync.Player2PlayerCharacter) ArrayNum = 1;

        PlayersInventory[ArrayNum].HotbarSlots[PlayersInventory[ArrayNum].EquippedHotbarSlot].ActiveHotbarItem.enabled = false;
        PlayersInventory[ArrayNum].EquippedHotbarSlot = PressedItem.SlotID;
        PressedItem.ActiveHotbarItem.enabled = true;


        DG_PlayerCharacters.CharacterEquipment Equipment = QuickFind.Farm.PlayerCharacters[PlayerID].Equipment;
        if (Equipment.RucksackSlots[PlayersInventory[ArrayNum].EquippedHotbarSlot].GetStackValue() == 0)
            QuickFind.ItemActivateableHandler.SetNoActiveItem(ArrayNum);
        else
        {
            DG_ItemObject Object = QuickFind.ItemDatabase.GetItemFromID(Equipment.RucksackSlots[PlayersInventory[ArrayNum].EquippedHotbarSlot].ContainedItem);
            QuickFind.ItemActivateableHandler.SetCurrentActiveItem(Equipment.RucksackSlots[PlayersInventory[ArrayNum].EquippedHotbarSlot], Object, PlayersInventory[ArrayNum].EquippedHotbarSlot, PlayerID);
        }
    }
}
