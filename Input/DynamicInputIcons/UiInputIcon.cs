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

        /*[Serializable]
        private class InputDeviceIsCompositePair{
            public ControllerInputTypes _inputDevice;
            public bool _isPartOfComposite;
        }*/


        private enum CompositeType{
            Positive,
            Negative,
            Up,
            Down,
            Left,
            Right,
        }

        [HideIf("_assignActionPerControlType")]
        [SerializeField] protected InputActionReference _actionReference;

        public InputActionReference ActionReference {
            get => _actionReference;
            set => _actionReference = value;
        }

        [SerializeField] private bool _assignActionPerControlType;
        [ShowIf("_assignActionPerControlType")]
        [SerializeField] private List<KeyValuePair> _controlTypeActions = new List<KeyValuePair>();


        [Space]
        [SerializeField] private List<InputDeviceScalePair> _inputTypeScale;


        [Space]
        [Header("Composite Bindings")]
        [SerializeField] private bool _usingCompositeBindings;

        [ShowIf("_usingCompositeBindings")]
        [SerializeField] private CompositeType _compositeType;

        [ShowIf("_usingCompositeBindings")]
        [SerializeField] private List<ControllerInputTypes> _compositeControlTypes;


        [Header("Text Instead of Image")]
        [SerializeField] private List<ControllerInputTypes> _usesTextInsteadOfImage;
        [SerializeField] private TextMeshProUGUI _inputText;

        [SerializeField] private InputIconsSprites _keyboardIcons;
        [SerializeField] private InputIconsSprites _ps4Icons;
        [SerializeField] private InputIconsSprites _xboxIcons;
        [SerializeField] private InputIconsSprites _ps5Icons;
        [SerializeField] private Image _image;


        [Space]
        [Tooltip("This is the control type which will be used in the editor. Does not effect the icon used when the game is run")]
        [SerializeField] private ControllerInputTypes _editorType = ControllerInputTypes.KeyboardMouse;


        private Dictionary<CompositeType, string> _compositeTypeNames = new Dictionary<CompositeType, string> {
            { CompositeType.Positive, "positive" },
            { CompositeType.Negative, "negative" },
            { CompositeType.Up, "up" },
            { CompositeType.Down, "down" },
            { CompositeType.Left, "left" },
            { CompositeType.Right, "right" },
        };

        [Button]
        public void SetIconInEditor() {
            _image ??= GetComponent<Image>();
            var inputBindingMask = _editorType switch {
                ControllerInputTypes.KeyboardMouse => InputBinding.MaskByGroup("Keyboard&Mouse"),
                ControllerInputTypes.PS4Controller => InputBinding.MaskByGroup("GamePad"),
                ControllerInputTypes.XboxController => InputBinding.MaskByGroup("GamePad"),
                ControllerInputTypes.PS5Controller => InputBinding.MaskByGroup("GamePad"),
                _ => throw new ArgumentOutOfRangeException()
            };
            SetImageSprite(_editorType, inputBindingMask);
        }

        private void Awake() {
            _image ??= GetComponent<Image>();
        }

        /// <summary>
        /// Gets the action reference and sets the image to to the sprite which coresponds the the input the the action
        /// is using given the current controller type. If an action reference is not found then disable the game object. 
        /// </summary>
        public void SetImageSprite(ControllerInputTypes types, InputBinding mask) {
            if (_inputText) {
                _inputText.gameObject.SetActive(false);
            }

            _image.enabled = true;


            var actionReference = GetActionReference(types);
            if (actionReference == null) {
                _image.enabled = false;
                return;
            }


            var displayStringOptions = InputBinding.DisplayStringOptions.DontIncludeInteractions;
            int bindingIndex = GetBindingIndex(types, mask, actionReference);
            if (bindingIndex == -1) {
                _image.enabled = false;
                return;
            }

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
            float scale = 1.0f;
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
            //return controlTypeActions.ContainsKey(types) ? controlTypeActions[types] : null;
        }


        private int GetBindingIndex(ControllerInputTypes types, InputBinding mask, InputActionReference actionReference) {
            if (_usingCompositeBindings && _compositeControlTypes.Contains(types)) {
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
    }
}