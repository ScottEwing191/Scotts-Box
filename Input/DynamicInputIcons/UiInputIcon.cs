using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using NaughtyAttributes;
#endif
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ScottEwing.Input.DynamicInputIcons{
    public class UiInputIcon : MonoBehaviour{
        [Serializable]
        private class KeyValuePair{
            public ControllerInputTypes _key;
            public InputActionReference _value;
        }

        [Serializable]
        private class InputDeviceScalePair{
            public ControllerInputTypes _inputType;
            public float _scale = 1.0f;
        }
        
        private enum CompositeType{
            Positive,
            Negative,
            Up,
            Down,
            Left,
            Right,
        }
        
        private enum NoIconFoundBehaviour{ DisableImage, DisableGameObject }
        
        // --- Input Actions ---
        [Title("Input Actions")]
        [HideIf("_assignActionPerControlType")]
        [SerializeField] protected InputActionReference _actionReference;

        public InputActionReference ActionReference {
            get => _actionReference;
            set => _actionReference = value;
        }
        
        [SerializeField] private NoIconFoundBehaviour _noIconFoundBehaviour = NoIconFoundBehaviour.DisableImage;
        
        [SerializeField] private bool _assignActionPerControlType;
        [ShowIf("_assignActionPerControlType")]
        [SerializeField] private List<KeyValuePair> _controlTypeActions = new List<KeyValuePair>();
        
        // --- Input Type Scaling ---
        [Title("Input Type Scaling")]
        [SerializeField] private List<InputDeviceScalePair> _inputTypeScale;


        // --- Composite Bindings ---
        [Title("Composite Bindings")]
        [SerializeField] private bool _usingCompositeBindings;

        [ShowIf("_usingCompositeBindings")]
        [SerializeField] private CompositeType _compositeType;
        
        [ShowIf("_usingCompositeBindings")]
        [SerializeField] private List<ControllerInputTypes> _nonCompositeControlTypes;
        
        // --- Interaction Text ---  
        [Title("Interaction Text")]
        [ValueDropdown(nameof(GetControlSchemes))]
        [SerializeField] private List<string> _interactionTextControlSchemes;
        [SerializeField] private TextMeshProUGUI _interactionText;
        

        //[Header("Text Instead of Image")]
        // --- Text Instead of Image ---
        [Title("Text Instead of Image")]
        [SerializeField] private List<ControllerInputTypes> _usesTextInsteadOfImage;
        [PropertySpace(0, 10)] // Adds 10 pixels after this field
        [SerializeField] private TextMeshProUGUI _inputText;
        
        [FoldoutGroup("Input Icons")]
        [SerializeField] private InputIconsSprites _keyboardIcons;
        [FoldoutGroup("Input Icons")]
        [SerializeField] private InputIconsSprites _ps4Icons;
        [FoldoutGroup("Input Icons")]
        [SerializeField] private InputIconsSprites _xboxIcons;
        [FoldoutGroup("Input Icons")]
        [SerializeField] private InputIconsSprites _ps5Icons;
        [FoldoutGroup("Input Icons")]
        [SerializeField] private Image _image;
        
        // --- Editor Settings (Grouped at the Bottom) ---
        //[Title("Editor Settings")]
        [Space]
        [BoxGroup("Editor Settings")]
        [ValueDropdown(nameof(GetControlSchemes))]
        public string selectedControlScheme;
        [BoxGroup("Editor Settings")]
        [Tooltip("This is the control type which will be used in the editor. Does not effect the icon used when the game is run")]
        [SerializeField] private ControllerInputTypes _editorType = ControllerInputTypes.KeyboardMouse;

        [BoxGroup("Editor Settings")]
        [Button]
        public void SetIconInEditor()
        {
            _image ??= GetComponent<Image>();

            InputBinding inputBindingMask;

            if (!string.IsNullOrEmpty(selectedControlScheme))
            {
                // Use the selected control scheme
                inputBindingMask = InputBinding.MaskByGroup(selectedControlScheme);
                print("inputBindingMask: " + inputBindingMask);
            }
            else {
                // Fall back to the default mapping based on the editor type
                inputBindingMask = _editorType switch {
                    ControllerInputTypes.KeyboardMouse => InputBinding.MaskByGroup("Keyboard&Mouse"),
                    ControllerInputTypes.PS4Controller => InputBinding.MaskByGroup("GamePad"),
                    ControllerInputTypes.XboxController => InputBinding.MaskByGroup("GamePad"),
                    ControllerInputTypes.PS5Controller => InputBinding.MaskByGroup("GamePad"),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            SetImageSprite(_editorType, inputBindingMask);
        }
        
        private IEnumerable<string> GetControlSchemes()
        {
            // Find a valid action reference
            var actionRef = _actionReference;
            if (actionRef == null)
            {
                // Look for the first non-null action in the collection
                actionRef = _controlTypeActions.FirstOrDefault(action => action != null)?._value;
            }
            
            
            if (actionRef != null && actionRef.asset != null)
            {
                yield return "";
                foreach (var scheme in actionRef.asset.controlSchemes)
                {
                    yield return scheme.name;
                }
            }
        }
        
        private Dictionary<CompositeType, string> _compositeTypeNames = new Dictionary<CompositeType, string> {
            { CompositeType.Positive, "positive" },
            { CompositeType.Negative, "negative" },
            { CompositeType.Up, "up" },
            { CompositeType.Down, "down" },
            { CompositeType.Left, "left" },
            { CompositeType.Right, "right" },
        };

        private void Awake() {
            _image ??= GetComponent<Image>();
        }

        /// <summary>
        /// NAME SHOULD BE CHANGED TO UPDATE ICON. Dont want to mess up other games using this
        /// Gets the action reference and sets the image to to the sprite which coresponds the the input the the action
        /// is using given the current controller type. If an action reference is not found then disable the game object. 
        /// </summary>
        public virtual void SetImageSprite(ControllerInputTypes types, InputBinding mask) {
            if (_inputText) {
                _inputText.gameObject.SetActive(false);
            }
            
            
            
            // Activate Image or GameObject
            SetImageOrGameObjectActive(true);
            _image.enabled = true;  // ensure image is enabled (may have been disable independently of SetImageOrGameObjectActive by SetTextNotImage)
            

            var actionReference = GetActionReference(types);
            
            Debug.Log("Interaction: " + actionReference.action.interactions, this);
            
            if (!actionReference) {
                //_image.enabled = false;
                SetImageOrGameObjectActive(false);
                return;
            }


            var displayStringOptions = InputBinding.DisplayStringOptions.DontIncludeInteractions;
            int bindingIndex = GetBindingIndex(types, mask, actionReference);
            if (bindingIndex == -1) {
                //_image.enabled = false;
                SetImageOrGameObjectActive(false);
                return;
            }
            
            //-- Display Interaction Text
            SetInteractionText(mask, actionReference, bindingIndex);

            var bindingDisplay = actionReference.action.GetBindingDisplayString(bindingIndex, out var deviceLayoutName, out var controlPath, displayStringOptions);
            
            
            
            if (_usesTextInsteadOfImage.Contains(types)) {
                SetTextNotImage(bindingDisplay);
                return;
            }

            _image.sprite = types switch {
                ControllerInputTypes.KeyboardMouse => _keyboardIcons.GetSprite(controlPath),
                ControllerInputTypes.PS4Controller => _ps4Icons.GetSprite(controlPath),
                ControllerInputTypes.XboxController => _xboxIcons.GetSprite(controlPath),
                ControllerInputTypes.PS5Controller => _ps5Icons.GetSprite(controlPath),

                _ => throw new ArgumentOutOfRangeException(nameof(types), types, null)
            };
            
            SetImageScale(types);
        }

        /// <summary>
        /// Sets the scale of the image to 1 unless the input type is in the inputTypeScale list in which case it will set the scale to the value in the list.
        /// </summary>
        private void SetImageScale(ControllerInputTypes types) {
            var scale = 1.0f;
            var scaleEntry = _inputTypeScale.Find(x => x._inputType == types);
            if (scaleEntry != null) {
                scale = scaleEntry._scale;
            }
            _image.rectTransform.localScale = Vector3.one * scale;
        }
        
        private void SetTextNotImage(string text) {
            _image.enabled = false;
            _inputText.SetText("" + text + "");
            _inputText.gameObject.SetActive(true);
        }

        private InputActionReference GetActionReference(ControllerInputTypes types) {
            if (!_assignActionPerControlType) {
                return _actionReference;
            }

            foreach (var keyValuePair in _controlTypeActions) {
                if (keyValuePair._key == types) {
                    return keyValuePair._value;
                }
            }

            return null;
        }

        private int GetBindingIndex(ControllerInputTypes types, InputBinding mask, InputActionReference actionReference) {
            //if (_usingCompositeBindings && _compositeControlTypes.Contains(types)) {
            if (_usingCompositeBindings && !_nonCompositeControlTypes.Contains(types)) {
                return GetCompositeBindingIndex(actionReference.action, mask, _compositeType);
            }
            
            return actionReference.action.GetBindingIndex(mask);
        }
        
        private int GetCompositeBindingIndex(InputAction action, InputBinding bindingMask, CompositeType direction) {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var bindings = action.bindings;
            for (var i = 0; i < bindings.Count; ++i)
                if (bindingMask.Matches(bindings[i])) {
                    if (_compositeTypeNames.TryGetValue(direction, out var compositeName) && compositeName == bindings[i].name) {
                        return i;
                    }
                }

            return -1;
        }

        private void SetInteractionText(InputBinding inputBindingMask, InputActionReference actionReference, int bindingIndex) {
            _interactionText.gameObject.SetActive(false);
            
            if (!_interactionTextControlSchemes.Contains(inputBindingMask.groups))
                return;

            var binding = actionReference.action.bindings[bindingIndex];
            var interaction = string.IsNullOrEmpty(binding.interactions) ? actionReference.action.interactions : binding.interactions;

            if (string.IsNullOrEmpty(interaction))
                return;

            _interactionText.SetText(interaction.Split(',')[0]);
            _interactionText.gameObject.SetActive(true);
            
            /*if (!_interactionTextControlSchemes.Contains(inputBindingMask.groups)) 
                return;
            var bindingInteractions = actionReference.action.bindings[bindingIndex].interactions;
            if (string.IsNullOrEmpty(bindingInteractions)) {
                bindingInteractions = actionReference.action.interactions;
            }

            if (string.IsNullOrEmpty(bindingInteractions) ) {
                return;
            }
            
            bindingInteractions = bindingInteractions.Split(',', StringSplitOptions.None)[0];
            
            _interactionText.SetText(bindingInteractions);
            _interactionText.gameObject.SetActive(true);
            */
            
        }
        
        private void SetImageOrGameObjectActive(bool active) {
            if (_noIconFoundBehaviour == NoIconFoundBehaviour.DisableImage) {
                _image.enabled = active;
            }else if (_noIconFoundBehaviour == NoIconFoundBehaviour.DisableGameObject) {
                gameObject.SetActive(active);
            }
        }
    }
}