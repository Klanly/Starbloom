using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem.TextMeshPro
{

    /// <summary>
    /// This component allows actors to override TextMesh Pro dialogue controls. It's 
    /// particularly useful to assign world space UIs such as speech bubbles above
    /// actors' heads.
    /// </summary>
    [AddComponentMenu("Dialogue System/Third Party/TextMesh Pro/Dialogue/Override TextMesh Pro Dialogue Controls")]
    public class OverrideTextMeshProDialogueControls : MonoBehaviour
    {

        [Tooltip("Use these controls when playing subtitles through this actor")]
        public TextMeshProSubtitleControls subtitle;

        [Tooltip("Use these controls when showing subtitle reminders for actor")]
        public TextMeshProSubtitleControls subtitleReminder;

        [Tooltip("Use these controls when showing a response menu involving this actor")]
        public TextMeshProResponseMenuControls responseMenu;

        private bool checkedContinueButton = false;

        public virtual void Start()
        {
            if (subtitle != null) subtitle.SetActive(false);
            if (subtitleReminder != null) subtitleReminder.SetActive(false);
            if (responseMenu != null) responseMenu.SetActive(false);
        }

        public virtual void ApplyToDialogueUI(TextMeshProDialogueUI ui)
        {
            if (checkedContinueButton) return;
            if (subtitle != null && subtitle.continueButton != null)
            {
                // Make sure continue button is connected:
                if (subtitle.continueButton.onClick.GetPersistentEventCount() == 0 || // OnClick() unassigned or
                subtitle.continueButton.onClick.GetPersistentTarget(0) == null) // OnClick() target is null:
                {
                    subtitle.continueButton.onClick.AddListener(ui.OnContinue);
                }
                // If continue button has fast forward, make sure its UI reference is connected:
                var ffwd = subtitle.continueButton.GetComponent<TextMeshProContinueButtonFastForward>();
                if (ffwd != null && ffwd.dialogueUI == null)
                {
                    ffwd.dialogueUI = ui;
                }
            }
            checkedContinueButton = true;
        }

    }

}
