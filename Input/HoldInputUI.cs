using ScottEwing.Input.DynamicInputIcons;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

//==============================================================================================
//--Checkpoint Reset UI show a visual representation of the players input while holding the reset button.
//--The Reset UI will also be displayed if the player gets further away from the checkpoint to prompt
//--the player to use the Reset feature.
//==============================================================================================


namespace ScottEwing.Input{
    public class HoldInputUI : MonoBehaviour{
        [SerializeField] private Image _filledImage;
        private bool _isButtonHeld;
        private float _timer = 0.0f;
        private float _holdTime;

        //protected Player ThisPlayer;
        [SerializeField] protected InputActionReference _actionReference;


        [Button]
        private void AssignActionReference() {
            var uiInputIcon =  transform.parent.GetComponentInChildren<UiInputIcon>();
            if (uiInputIcon && uiInputIcon.ActionReference){
                _actionReference = uiInputIcon.ActionReference; 
            }
        }
        
        private void Awake() {
            /*if (_actionReference == null) {
                AssignActionReference(); 
            }*/
        }

        private void OnEnable() {
            //_actionReference.action.actionMap.Enable();
            if (!_actionReference) return;
            
            _actionReference.action.started += OnHoldStarted;
            _actionReference.action.canceled += OnHoldCancelled;
            _actionReference.action.performed += OnHoldComplete;
            
            IsHoldAlreadyStart();
        }

        private void OnDisable() {
            if (!_actionReference) return;
            
            _actionReference.action.started -= OnHoldStarted;
            _actionReference.action.canceled -= OnHoldCancelled;
            _actionReference.action.performed -= OnHoldComplete;
            if (_isButtonHeld) {
                StopButtonHold();
            }
            
            //_actionReference.action.actionMap.Disable();
        }

        private void OnHoldStarted(InputAction.CallbackContext obj) {
            HoldInteraction interaction = (HoldInteraction)obj.interaction;
            if (_actionReference != null && interaction != null) {
                StartButtonHold(interaction.duration);
            }
        }

        private void IsHoldAlreadyStart() {
            if (_actionReference.action.phase == InputActionPhase.Started){
                _isButtonHeld = true;
                _filledImage.gameObject.SetActive(true);
                _filledImage.fillAmount = _actionReference.action.GetTimeoutCompletionPercentage();
                
            }
        }

        private void OnHoldComplete(InputAction.CallbackContext obj) {
            HoldInteraction interaction = (HoldInteraction)obj.interaction;
            if (_actionReference != null && interaction != null) {
                StopButtonHold();
            }
        }

        private void OnHoldCancelled(InputAction.CallbackContext obj) {
            if (_actionReference != null) {
                StopButtonHold();
            }
        }


        public virtual void StartButtonHold(float holdTime) {
            //_filledImage.gameObject.SetActive(true);
            _timer = 0;
            _holdTime = holdTime;
            _isButtonHeld = true;
            _filledImage.fillAmount = 0;
        }

        public virtual void StopButtonHold() {
            _isButtonHeld = false;
            _filledImage.fillAmount = 0;
            //_filledImage.gameObject.SetActive(false);
        }

        public virtual void ShowHoldButtonUI() {
            _filledImage.gameObject.SetActive(true);
            _filledImage.fillAmount = 0;
        }

        public virtual void HideHoldButtonUI() {
            _filledImage.gameObject.SetActive(false);
        }

        /*private void Update() {
            if (!_isButtonHeld) {
                return;
            }

            if (_timer > _holdTime) {
                _filledImage.fillAmount = 0;
                return;
            }

            _timer += Time.deltaTime;
            _filledImage.fillAmount = Mathf.Lerp(0, 1, _timer / _holdTime);
        }*/
        
        private void Update() {
            if (!_isButtonHeld) {
                return;
            }

            _filledImage.fillAmount = _actionReference.action.GetTimeoutCompletionPercentage();
        }
    }
}