using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ScottEwing.Input.DynamicInputIcons{
    public class UiInputIcon : MonoBehaviour{
        [SerializeField] private InputActionReference _actionReference;
        [SerializeField] private InputIconsSprites _keyboardIcons;
        [SerializeField] private InputIconsSprites _ps4Icons;
        [SerializeField] private InputIconsSprites _xboxIcons;

        [SerializeField] private Image _image;
        
        private void Awake() {
            _image ??= GetComponent<Image>();
        }

        public void SetImageSprite(ControllerInputTypes types, InputBinding mask) {

            var displayStringOptions = InputBinding.DisplayStringOptions.DontIncludeInteractions;
            var bindingIndex = _actionReference.action.GetBindingIndex(mask);
            var bindingDisplay = _actionReference.action.GetBindingDisplayString(bindingIndex, out var deviceLayoutName, out var controlPath, displayStringOptions);
            
            _image.sprite = types switch {
                ControllerInputTypes.KeyboardMouse => _keyboardIcons.GetSprite(controlPath),
                ControllerInputTypes.PS4Controller => _ps4Icons.GetSprite(controlPath),
                ControllerInputTypes.XboxController => _xboxIcons.GetSprite(controlPath),
                _ => throw new ArgumentOutOfRangeException(nameof(types), types, null)
            };
        }
    }
}
