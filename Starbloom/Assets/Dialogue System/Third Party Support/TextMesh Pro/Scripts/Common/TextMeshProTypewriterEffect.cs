using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem.TextMeshPro
{

    /// <summary>
    /// This is a typewriter effect for TextMesh Pro.
    /// 
    /// Note: Handles RPGMaker codes, but not two codes next to each other.
    /// </summary>
    [AddComponentMenu("Dialogue System/Third Party/TextMesh Pro/TextMesh Pro Typewriter Effect")]
    [DisallowMultipleComponent]
    public class TextMeshProTypewriterEffect : MonoBehaviour
    {

        /// <summary>
        /// How fast to "type."
        /// </summary>
        [Tooltip("How fast to type. This is separate from Dialogue Manager > Subtitle Settings > Chars Per Second.")]
        public float charactersPerSecond = 50;

        /// <summary>
        /// The audio clip to play with each character.
        /// </summary>
        [Tooltip("Optional audio clip to play with each character.")]
        public AudioClip audioClip = null;

        /// <summary>
        /// The audio source through which to play the clip. If unassigned, will look for an
        /// audio source on this GameObject.
        /// </summary>
        [Tooltip("Optional audio source through which to play the clip.")]
        public AudioSource audioSource = null;

        /// <summary>
        /// If audio clip is still playing from previous character, stop and restart it when typing next character.
        /// </summary>
        [Tooltip("If audio clip is still playing from previous character, stop and restart it when typing next character.")]
        public bool interruptAudioClip = false;

        /// <summary>
        /// Ensures this GameObject has only one typewriter effect.
        /// </summary>
        [Tooltip("Ensure this GameObject has only one typewriter effect.")]
        public bool removeDuplicateTypewriterEffects = true;

        /// <summary>
        /// Play using the current text content whenever component is enabled.
        /// </summary>
        [Tooltip("Play using the current text content whenever component is enabled.")]
        public bool playOnEnable = true;

        /// <summary>
        /// Wait one frame to allow layout elements to setup first.
        /// </summary>
        [Tooltip("Wait one frame to allow layout elements to setup first.")]
        public bool waitOneFrameBeforeStarting = false;

        /// <summary>
        /// Don't play audio on these characters.
        /// </summary>
        [Tooltip("Don't play audio on these characters.")]
        public string silentCharacters = string.Empty;

        /// <summary>
        /// Duration to pause on when text contains '\\.'
        /// </summary>
        [Tooltip("Duration to pause on when text contains '\\.'")]
        public float fullPauseDuration = 1f;

        /// <summary>
        /// Duration to pause when text contains '\\,'
        /// </summary>
        [Tooltip("Duration to pause when text contains '\\,'")]
        public float quarterPauseDuration = 0.25f;

        [System.Serializable]
        public class AutoScrollSettings
        {
            [Tooltip("Automatically scroll to bottom of scroll rect. Useful for long text. Works best with left justification.")]
            public bool autoScrollEnabled = false;
            public UnityEngine.UI.ScrollRect scrollRect = null;
            public UnityUIScrollbarEnabler scrollbarEnabler = null;
        }

        /// <summary>
        /// Optional auto-scroll settings.
        /// </summary>
        public AutoScrollSettings autoScrollSettings = new AutoScrollSettings();

        public UnityEvent onBegin = new UnityEvent();
        public UnityEvent onCharacter = new UnityEvent();
        public UnityEvent onEnd = new UnityEvent();

        /// <summary>
        /// Indicates whether the effect is playing.
        /// </summary>
        /// <value><c>true</c> if this instance is playing; otherwise, <c>false</c>.</value>
        //public bool IsPlaying { get; private set; }
        public bool IsPlaying { get { return typewriterCoroutine != null; } }

        private const string RPGMakerCodeQuarterPause = @"\,";
        private const string RPGMakerCodeFullPause = @"\.";
        private const string RPGMakerCodeSkipToEnd = @"\^";
        private const string RPGMakerCodeInstantOpen = @"\>";
        private const string RPGMakerCodeInstantClose = @"\<";

        private enum RPGMakerTokenType
        {
            None,
            QuarterPause,
            FullPause,
            SkipToEnd,
            InstantOpen,
            InstantClose
        }

        private Dictionary<int, List<RPGMakerTokenType>> rpgMakerTokens = new Dictionary<int, List<RPGMakerTokenType>>();

        private TMPro.TMP_Text m_textComponent = null;
        private TMPro.TMP_Text textComponent
        {
            get
            {
                if (m_textComponent == null) m_textComponent = GetComponent<TMPro.TMP_Text>();
                return m_textComponent;
            }
        }

        private AudioSource runtimeAudioSource
        {
            get
            {
                if (audioSource == null) audioSource = GetComponent<AudioSource>();
                if (audioSource == null && (audioClip != null))
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                    audioSource.playOnAwake = false;
#if UNITY_4_6 || UNITY_4_7
                    audioSource.pan = 0;
#else
                    audioSource.panStereo = 0;
#endif
                }
                return audioSource;
            }
        }

        private bool started = false;
        private bool paused = false;
        private int charactersTyped = 0;
        private Coroutine typewriterCoroutine = null;
        private MonoBehaviour coroutineController = null;

        public void Awake()
        {
            
            if (removeDuplicateTypewriterEffects) RemoveIfDuplicate();
        }

        private void RemoveIfDuplicate()
        {
            var effects = GetComponents<TextMeshProTypewriterEffect>();
            if (effects.Length > 1)
            {
                TextMeshProTypewriterEffect keep = effects[0];
                for (int i = 1; i < effects.Length; i++)
                {
                    if (effects[i].GetInstanceID() < keep.GetInstanceID())
                    {
                        keep = effects[i];
                    }
                }
                for (int i = 0; i < effects.Length; i++)
                {
                    if (effects[i] != keep)
                    {
                        Destroy(effects[i]);
                    }
                }
            }
        }

        public void Start()
        {
            if (!IsPlaying && playOnEnable)
            {
                StopTypewriterCoroutine();
                StartTypewriterCoroutine();
            }
            started = true;
        }

        public void OnEnable()
        {
            if (!IsPlaying && playOnEnable && started)
            {
                StopTypewriterCoroutine();
                StartTypewriterCoroutine();
            }
        }

        public void OnDisable()
        {
            Stop();
        }

        /// <summary>
        /// Pauses the effect.
        /// </summary>
        public void Pause()
        {
            paused = true;
        }

        /// <summary>
        /// Unpauses the effect. The text will resume at the point where it
        /// was paused; it won't try to catch up to make up for the pause.
        /// </summary>
        public void Unpause()
        {
            paused = false;
        }

        public void Rewind()
        {
            charactersTyped = 0;
        }

        /// <summary>
        /// Play typewriter on text immediately.
        /// </summary>
        /// <param name="text"></param>
        public void PlayText(string text)
        {
            StopTypewriterCoroutine();
            textComponent.text = text;
            StartTypewriterCoroutine();
        }

        private void StartTypewriterCoroutine()
        {
            if (coroutineController == null || !coroutineController.gameObject.activeInHierarchy)
            {
                // This MonoBehaviour might not be enabled yet, so use one that's guaranteed to be enabled:
                MonoBehaviour controller = GetComponentInParent<TextMeshProDialogueUI>();
                if (controller == null) controller = DialogueManager.Instance;
                coroutineController = controller;
            }
            typewriterCoroutine = coroutineController.StartCoroutine(Play());
        }

        /// <summary>
        /// Plays the typewriter effect.
        /// </summary>
        public IEnumerator Play()
        {
            if ((textComponent != null) && (charactersPerSecond > 0))
            {
                if (waitOneFrameBeforeStarting) yield return null;
                ProcessRPGMakerCodes();
                if (runtimeAudioSource != null) runtimeAudioSource.clip = audioClip;
                onBegin.Invoke();
                paused = false;
                float delay = 1 / charactersPerSecond;
                float lastTime = DialogueTime.time;
                float elapsed = 0;
                textComponent.maxVisibleCharacters = 0;
                textComponent.ForceMeshUpdate();
                yield return null;
                textComponent.maxVisibleCharacters = 0;
                textComponent.ForceMeshUpdate();
                TMPro.TMP_TextInfo textInfo = textComponent.textInfo;
                int totalVisibleCharacters = textInfo.characterCount; // Get # of Visible Character in text object
                charactersTyped = 0;
                int skippedCharacters = 0;
                while (charactersTyped < totalVisibleCharacters)
                {
                    if (!paused)
                    {
                        var deltaTime = DialogueTime.time - lastTime;
                        elapsed += deltaTime;
                        var goal = (elapsed * charactersPerSecond) - skippedCharacters;
                        while (charactersTyped < goal)
                        {
                            if (rpgMakerTokens.ContainsKey(charactersTyped))
                            {
                                var tokens = rpgMakerTokens[charactersTyped];
                                for (int i = 0; i < tokens.Count; i++)
                                {
                                    var token = tokens[i];
                                    switch (token)
                                    {
                                        case RPGMakerTokenType.QuarterPause:
                                            yield return new WaitForSeconds(quarterPauseDuration);
                                            break;
                                        case RPGMakerTokenType.FullPause:
                                            yield return new WaitForSeconds(fullPauseDuration);
                                            break;
                                        case RPGMakerTokenType.SkipToEnd:
                                            charactersTyped = totalVisibleCharacters - 1;
                                            break;
                                        case RPGMakerTokenType.InstantOpen:
                                            var close = false;
                                            while (!close && charactersTyped < totalVisibleCharacters)
                                            {
                                                charactersTyped++;
                                                skippedCharacters++;
                                                if (rpgMakerTokens.ContainsKey(charactersTyped) && rpgMakerTokens[charactersTyped].Contains(RPGMakerTokenType.InstantClose))
                                                {
                                                    close = true;
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                            if (charactersTyped < totalVisibleCharacters && !IsSilentCharacter(textComponent.text[charactersTyped])) PlayCharacterAudio();
                            onCharacter.Invoke();
                            charactersTyped++;
                            textComponent.maxVisibleCharacters = charactersTyped;
                        }
                    }
                    textComponent.maxVisibleCharacters = charactersTyped;
                    HandleAutoScroll();
                    //---Uncomment the line below to debug: 
                    //Debug.Log(textComponent.text.Substring(0, charactersTyped).Replace("<", "[").Replace(">", "]") + " (typed=" + charactersTyped + ")");
                    lastTime = DialogueTime.time;
                    var delayTime = DialogueTime.time + delay;
                    int delaySafeguard = 0;
                    while (DialogueTime.time < delayTime && delaySafeguard < 999)
                    {
                        delaySafeguard++;
                        yield return null;
                    }
                }
            }
            Stop();
        }

        private void ProcessRPGMakerCodes()
        {
            rpgMakerTokens.Clear();
            var source = textComponent.text;
            var result = string.Empty;
            if (!source.Contains("\\")) return;
            int safeguard = 0;
            while (!string.IsNullOrEmpty(source) && safeguard < 9999)
            {
                safeguard++;
                RPGMakerTokenType token;
                if (PeelRPGMakerTokenFromFront(ref source, out token))
                {
                    int i = result.Length;
                    if (!rpgMakerTokens.ContainsKey(i))
                    {
                        rpgMakerTokens.Add(i, new List<RPGMakerTokenType>());
                    }
                    rpgMakerTokens[i].Add(token);
                }
                else
                {
                    result += source[0];
                    source = source.Remove(0, 1);
                }
            }
            textComponent.text = result;
        }

        private bool PeelRPGMakerTokenFromFront(ref string source, out RPGMakerTokenType token)
        {
            token = RPGMakerTokenType.None;
            if (string.IsNullOrEmpty(source) || source.Length < 2 || source[0] != '\\') return false;
            var s = source.Substring(0, 2);
            if (string.Equals(s, RPGMakerCodeQuarterPause))
            {
                token = RPGMakerTokenType.QuarterPause;
            }
            else if (string.Equals(s, RPGMakerCodeFullPause))
            {
                token = RPGMakerTokenType.FullPause;
            }
            else if (string.Equals(s, RPGMakerCodeSkipToEnd))
            {
                token = RPGMakerTokenType.SkipToEnd;
            }
            else if (string.Equals(s, RPGMakerCodeInstantOpen))
            {
                token = RPGMakerTokenType.InstantOpen;
            }
            else if (string.Equals(s, RPGMakerCodeInstantClose))
            {
                token = RPGMakerTokenType.InstantClose;
            }
            else
            {
                return false;
            }
            source = source.Remove(0, 2);
            return true;
        }

        private bool IsSilentCharacter(char c)
        {
            if (string.IsNullOrEmpty(silentCharacters)) return false;
            return silentCharacters.Contains(c.ToString());
        }

        private void PlayCharacterAudio()
        {
            if (audioClip == null || runtimeAudioSource == null) return;
            if (interruptAudioClip)
            {
                if (runtimeAudioSource.isPlaying) runtimeAudioSource.Stop();
                runtimeAudioSource.Play();
            }
            else
            {
                if (!runtimeAudioSource.isPlaying) runtimeAudioSource.Play();
            }
        }

        private void StopTypewriterCoroutine()
        {
            if (typewriterCoroutine == null) return;
            if (coroutineController == null)
            {
                StopCoroutine(typewriterCoroutine);
            }
            else
            {
                coroutineController.StopCoroutine(typewriterCoroutine);
            }
            typewriterCoroutine = null;
            coroutineController = null;
        }

        /// <summary>
        /// Stops the effect.
        /// </summary>
        public void Stop()
        {
            if (IsPlaying) onEnd.Invoke();
            StopTypewriterCoroutine();
            if (textComponent != null) textComponent.maxVisibleCharacters = textComponent.textInfo.characterCount;
            HandleAutoScroll();
        }

        private void HandleAutoScroll()
        {
            if (!autoScrollSettings.autoScrollEnabled) return;
            if (autoScrollSettings.scrollRect != null)
            {
                autoScrollSettings.scrollRect.normalizedPosition = new Vector2(0, 0);
            }
            if (autoScrollSettings.scrollbarEnabler != null)
            {
                autoScrollSettings.scrollbarEnabler.CheckScrollbar();
            }
        }

    }

}
