using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DG_GUIContextHandler : MonoBehaviour {

    //Menu Controller Selection.
    [HideInInspector] public bool isDialogueOption;
    [HideInInspector] public int ControllerMenuPosition;
    [HideInInspector] public Image CurrentSelectedObject;
    [HideInInspector] public Image[] CurrentSelectableObjects;

    private void Awake()
    {
        QuickFind.GUIContextHandler = this;
    }
    public void OpenNewContextMenuSelectionState(Image[] NewSelectionOptions)
    {
        if (CurrentSelectedObject != null)
            CurrentSelectedObject.enabled = false;
        CurrentSelectableObjects = NewSelectionOptions;
        //
        CurrentSelectedObject = CurrentSelectableObjects[0];
        CurrentSelectedObject.enabled = true;
    }
    public void ContextButtonViaMouse(DG_MenuContextItem ButtonPressed, int PlayerValue)
    {
        QuickFind.InputController.MainPlayer.NoActionThisFrame = true;

        int buttonIndex = ButtonPressed.transform.GetSiblingIndex();
        if (buttonIndex != ControllerMenuPosition)
        {
            ControllerMenuPosition = buttonIndex;
            SetNewMenuPosition(true, buttonIndex);
        }
        else
            ContextButtonViaKey();
    }
    public void SetNewMenuPosition(bool isSet, int NewSetPosition = 0)
    {
        int SavedNewSetPosition = NewSetPosition;

        CurrentSelectedObject.enabled = false;
        int NewMenuPosition;

        if (!isSet)
        {
            NewMenuPosition = ControllerMenuPosition;
            NewMenuPosition = NewMenuPosition + NewSetPosition;
            int Count = CurrentSelectableObjects.Length;
            if (NewMenuPosition >= Count)
                NewMenuPosition = 0;
            else if (NewMenuPosition < 0)
                NewMenuPosition = Count - 1;



            NewSetPosition = NewMenuPosition;
            ControllerMenuPosition = NewSetPosition;
        }

        int SetPos = NewSetPosition;

        CurrentSelectedObject = CurrentSelectableObjects[SetPos];
        CurrentSelectedObject.enabled = true;

        if (!CurrentSelectedObject.isActiveAndEnabled)
            SetNewMenuPosition(isSet, SavedNewSetPosition);
    }
    public void ContextButtonViaKey()
    {
        if (isDialogueOption)
        {
            NA_DialogueGUIController GuiController = QuickFind.DialogueGUIController;
            NodeLink Active = GuiController.ActiveDialogue;
            NodeLink.Window WindowOption = GuiController.PathChoices[ControllerMenuPosition];

            Active.SetChoiceWindow(WindowOption);
            GuiController.NextTrigger();
            GuiController.ClearAllOptions();
         }
    }
}
