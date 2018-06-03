using UnityEngine;

namespace PixelCrushers.DialogueSystem.TextMeshPro
{

    /// <summary>
    /// Optional component to provide references to selector UI elements.
    /// </summary>
    [AddComponentMenu("Dialogue System/Third Party/TextMesh Pro/Selector/TextMesh Pro Selector Elements")]
    public class TextMeshProSelectorElements : MonoBehaviour
    {

        /// <summary>
        /// The main graphic (optional). Assign this if you have created an entire 
        /// panel for the selector.
        /// </summary>
        public UnityEngine.UI.Graphic mainGraphic = null;

        /// <summary>
        /// The text for the name of the current selection.
        /// </summary>
        public TMPro.TextMeshProUGUI nameText = null;

        /// <summary>
        /// The text for the use message (e.g., "Press spacebar to use").
        /// </summary>
        public TMPro.TextMeshProUGUI useMessageText = null;

        public Color inRangeColor = Color.yellow;

        public Color outOfRangeColor = Color.gray;

        /// <summary>
        /// The graphic to show if the selection is in range.
        /// </summary>
        public UnityEngine.UI.Graphic reticleInRange = null;

        /// <summary>
        /// The graphic to show if the selection is out of range.
        /// </summary>
        public UnityEngine.UI.Graphic reticleOutOfRange = null;

        public TextMeshProSelectorDisplay.AnimationTransitions animationTransitions = new TextMeshProSelectorDisplay.AnimationTransitions();

        public static TextMeshProSelectorElements instance = null;

        private void Awake()
        {
            instance = this;
        }
    }

}
