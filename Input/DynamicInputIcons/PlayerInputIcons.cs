using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ScottEwing.Input.DynamicInputIcons{
    
    public enum ControllerInputTypes{
        KeyboardMouse, PS4Controller, XboxController, PS5Controller
    }
    
    public class PlayerInputIcons : MonoBehaviour{
        [SerializeField] protected PlayerInput _playerInput;
        private InputBinding _inputBindingMask;
        private ControllerInputTypes _types;

        private UiInputIcon[] iconsActiveOnstart; 


        private void Awake() {
            _playerInput ??= GetComponent<PlayerInput>();
        }

        protected virtual void Start() {

            var  iconsActiveOnstart = GetComponentsInChildren<UiInputIcon>(true);
            
            StartCoroutine(SetUpIconsAfterOneFrame());
        }

        protected virtual void OnEnable() {
            _playerInput.onControlsChanged += SetUpIcons;
        }

        protected virtual void OnDisable() {
            _playerInput.onControlsChanged -= SetUpIcons;
        }

        private void OnDestroy() {
        }

        private IEnumerator SetUpIconsAfterOneFrame() {
            yield return null;
            SetUpIcons(_playerInput);
        }

        public void SetUpIcons() => SetUpIcons(_playerInput);


        public void SetUpIcons(PlayerInput playerInput) {
            if (playerInput.devices.Count == 0) return;
            var controllerName = playerInput.devices[0].name;

            if (controllerName == "DualSenseGamepadHID") {
                _inputBindingMask = InputBinding.MaskByGroup("GamePad");
                _types = ControllerInputTypes.PS5Controller;
            }
            else if (InputSystem.IsFirstLayoutBasedOnSecond(controllerName, "DualShockGamepad")) {
                _inputBindingMask = InputBinding.MaskByGroup("GamePad");
                _types = ControllerInputTypes.PS4Controller;
            }
            else if (InputSystem.IsFirstLayoutBasedOnSecond(controllerName, "Gamepad")) {
                _inputBindingMask = InputBinding.MaskByGroup("GamePad");
                _types = ControllerInputTypes.XboxController;
            }
            else if (InputSystem.IsFirstLayoutBasedOnSecond(controllerName, "Keyboard")) {
                _inputBindingMask = InputBinding.MaskByGroup("Keyboard&Mouse");
                _types = ControllerInputTypes.KeyboardMouse;
            }
            else {
                _inputBindingMask = InputBinding.MaskByGroup("GamePad");
                _types = ControllerInputTypes.XboxController;
            }

            foreach (var icon in GetComponentsInChildren<UiInputIcon>(true)) {
            //foreach (var icon in iconsActiveOnstart) {
                icon.SetImageSprite(_types, _inputBindingMask);
            }
        }
    }
}
