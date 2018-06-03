using UnityEngine;
using System;

namespace PixelCrushers.DialogueSystem.TextMeshPro
{

    /// <summary>
    /// Contains all dialogue (conversation) controls for a TextMeshPro Dialogue UI.
    /// </summary>
    [System.Serializable]
    public class TextMeshProDialogueControls : AbstractDialogueUIControls
    {

        /// <summary>
        /// The panel containing the dialogue controls. A panel is optional, but you may want one
        /// so you can include a background image, panel-wide effects, etc.
        /// </summary>
        [Tooltip("Panel containing the entire conversation UI")]
        public UnityEngine.UI.Graphic panel;

        [Tooltip("Optional animation transitions; panel should have an Animator")]
        public UIAnimationTransitions animationTransitions = new UIAnimationTransitions();

        /// <summary>
        /// The NPC subtitle controls.
        /// </summary>
        public TextMeshProSubtitleControls npcSubtitle;

        /// <summary>
        /// The PC subtitle controls.
        /// </summary>
        public TextMeshProSubtitleControls pcSubtitle;

        /// <summary>
        /// The response menu controls.
        /// </summary>
        public TextMeshProResponseMenuControls responseMenu;

        public override AbstractUISubtitleControls NPCSubtitle
        {
            get { return npcSubtitle; }
        }

        public override AbstractUISubtitleControls PCSubtitle
        {
            get { return pcSubtitle; }
        }

        public override AbstractUIResponseMenuControls ResponseMenu
        {
            get { return responseMenu; }
        }

        private UIShowHideController m_showHideController = null;
        private UIShowHideController showHideController
        {
            get
            {
                if (m_showHideController == null) m_showHideController = new UIShowHideController(null, panel, animationTransitions.transitionMode, animationTransitions.debug);
                return m_showHideController;
            }
        }

        public override void SetActive(bool value) // Must also play animation transitions to set active/inactive.
        {
            if (value == true)
            {
                ShowPanel();
            }
            else
            {
                HidePanel();
            }
        }

        public override void ShowPanel()
        {
            ActivateUIElements();
            animationTransitions.ClearTriggers(showHideController);
            showHideController.Show(animationTransitions.showTrigger, false, null);
        }

        private void HidePanel()
        {
            animationTransitions.ClearTriggers(showHideController);
            showHideController.Hide(animationTransitions.hideTrigger, DeactivateUIElements);
        }

        public void ActivateUIElements()
        {
            Tools.SetGameObjectActive(panel, true);
            //base.SetActive(true); // Don't show NPC, PC, Response Menu subpanels in case overrides supercede them.
        }

        public void DeactivateUIElements()
        {
            Tools.SetGameObjectActive(panel, false);
            base.SetActive(false); // Hides subpanels.
        }

    }

}
