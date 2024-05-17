using System;
using System.Collections;
using System.Collections.Generic;
using ScottEwing.Input;
using ScottEwing.Input.DynamicInputIcons;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

namespace ScottEwing
{
    public class HoldInputUIIcon : UiInputIcon{
        
        
        [Header("Hold Outline Shape")]
        [SerializeField] private Sprite _squareSprite;
        [SerializeField] private Sprite _circleSprite;
        
        
        
        [SerializeField] private Image _filledImage;
        [SerializeField] private UnityEvent _holdComplete;
        
        private void OnEnable() {
            _actionReference.action.actionMap.Enable();
            _actionReference.action.started += OnHoldStarted;
            _actionReference.action.canceled += OnHoldCancelled;
            _actionReference.action.performed += OnHoldComplete;



        }


        private void OnDisable() {
            _actionReference.action.started -= OnHoldStarted;
            _actionReference.action.canceled -= OnHoldCancelled;
            _actionReference.action.performed -= OnHoldComplete;
            _actionReference.action.actionMap.Disable();
        }


        private void OnHoldStarted(InputAction.CallbackContext obj) {
            var duration = ((HoldInteraction)obj.interaction).duration;
            _filledImage.gameObject.SetActive(true);
            _filledImage.fillAmount = 0;
            StartCoroutine(FillImage(duration));
        }
        private void OnHoldCancelled(InputAction.CallbackContext obj) {
            StopAllCoroutines();
            _filledImage.gameObject.SetActive(false);
        }
        
        private void OnHoldComplete(InputAction.CallbackContext obj) => _holdComplete?.Invoke();
        

        IEnumerator FillImage(float duration) {
            float timer = 0;
            while (timer < duration) {
                timer += Time.unscaledDeltaTime;
                _filledImage.fillAmount = Mathf.Lerp(0,1, timer / duration);
                yield return null;
            }
            
        }

        public override void SetImageSprite(ControllerInputTypes types, InputBinding mask) {
            base.SetImageSprite(types, mask);

            switch (types) {
                case ControllerInputTypes.KeyboardMouse:
                    _filledImage.sprite = _squareSprite;
                    break;
                case ControllerInputTypes.PS4Controller:
                case ControllerInputTypes.XboxController:
                case ControllerInputTypes.PS5Controller:
                    _filledImage.sprite = _circleSprite;
                    break;
            }
        }
    }
}
