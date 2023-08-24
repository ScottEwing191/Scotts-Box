using System;
using System.Collections.Generic;
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

        [SerializeField] private bool _assignActionPerControlType;


        [HideIf("_assignActionPerControlType")]
        [SerializeField] private InputActionReference _actionReference;

        [ShowIf("_assignActionPerControlType")]
        [SerializeField] private List<KeyValuePair> _controlTypeActions = new List<KeyValuePair>();

        //public Dictionary<ControllerInputTypes, InputActionReference> controlTypeActions = new Dictionary<ControllerInputTypes, InputActionReference>();

        [SerializeField] private InputIconsSprites _keyboardIcons;
        [SerializeField] private InputIconsSprites _ps4Icons;
        [SerializeField] private InputIconsSprites _xboxIcons;
        [SerializeField] private InputIconsSprites _ps5Icons;

        [SerializeField] private Image _image;


        [Tooltip("This is the control type which will be used in the editor. Does not effect the icon used when the game is run")]
        [SerializeField] private ControllerInputTypes _editorType = ControllerInputTypes.KeyboardMouse;


        [Button]
        private void SetIconInEditor() {
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
            _image.enabled = true;

            var actionReference = GetActionReference(types);
            if (actionReference == null) {
                //gameObject.SetActive(false);
                //_image.sprite = null;
                _image.enabled = false;
                return;
            }

            var displayStringOptions = InputBinding.DisplayStringOptions.DontIncludeInteractions;
            var bindingIndex = actionReference.action.GetBindingIndex(mask);
            if (bindingIndex == -1) {
                //gameObject.SetActive(false);
                //_image.sprite = null;
                _image.enabled = false;
                return;
            }

            var bindingDisplay = actionReference.action.GetBindingDisplayString(bindingIndex, out var deviceLayoutName, out var controlPath, displayStringOptions);

            _image.sprite = types switch {
                ControllerInputTypes.KeyboardMouse => _keyboardIcons.GetSprite(controlPath),
                ControllerInputTypes.PS4Controller => _ps4Icons.GetSprite(controlPath),
                ControllerInputTypes.XboxController => _xboxIcons.GetSprite(controlPath),
                ControllerInputTypes.PS5Controller => _ps5Icons.GetSprite(controlPath),

                _ => throw new ArgumentOutOfRangeException(nameof(types), types, null)
            };
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
    }
}