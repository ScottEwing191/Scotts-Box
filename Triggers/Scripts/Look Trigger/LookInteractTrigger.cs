using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ScottEwing.Triggers{
    public class LookInteractTrigger : Trigger, ILookInteractable, ITakesInput{
        [field:SerializeField] public InputActionProperty InputActionReference { get; set; }
        [SerializeField] protected float _maxInteractDistance = 1;

        public bool ShouldCheckInput { get; set; }

        private bool _isLooking;
        [Description("Used to detect the first frame that this trigger was not looked at")]
        private bool _lookedThisFixedUpdate;

        #region Unity Methods

        private void Update() => GetInput();
        protected virtual void FixedUpdate() => HandleFirstUpdateNotLookedAt();
        //-- Dont want base behaviour
        protected override void OnTriggerEnter(Collider other) { }
        protected override void OnTriggerStay(Collider other){ }
        protected override void OnTriggerExit(Collider other){ }

        #endregion
        
        public void GetInput() {
            if (!IsActivatable) return;
            if (!ShouldCheckInput) return;
            if (InputActionReference.action == null) return;
            if (InputActionReference.action.ReadValue<float>() > 0) {
                Triggered();
            }
        }

        /// <summary>
        /// Determines if this fixed update was the first time the trigger was not looked and calls look exit if it was
        /// </summary>
        private void HandleFirstUpdateNotLookedAt() {
            if (!_isLooking && _lookedThisFixedUpdate) {
                _isLooking = true;
            }
            if (_isLooking && !_lookedThisFixedUpdate) {
                _isLooking = false;
                LookExit();
            }
            _lookedThisFixedUpdate = false;     // reset this for next fram
        }

        public void Look(Vector3 cameraPosition) {
            if (!IsActivatable) return;
            _lookedThisFixedUpdate = true;
            if (!ShouldCheckInput && CanCameraActivateTrigger(cameraPosition)) {
                LookEnter();
            }
            else if (ShouldCheckInput && CanCameraActivateTrigger(cameraPosition)) {
                LookStay();
            }
            else if (ShouldCheckInput && !CanCameraActivateTrigger(cameraPosition)) {
                LookExit();
            }
        }

        public void LookEnter() {
            if (ShouldCheckInput) return;
            ShouldCheckInput = true;
            _onTriggerEnter?.Invoke();
        }

        public void LookStay() {
            _onTriggerStay?.Invoke();
        }

        public void LookExit() {
            if (!ShouldCheckInput) return;
            ShouldCheckInput = false;
            _onTriggerExit?.Invoke();
        }

        private bool CanCameraActivateTrigger(Vector3 cameraPosition) {
            if (Vector3.Distance(cameraPosition, transform.position) > _maxInteractDistance)  return false;
            return true;
        }


        
        
        
    }
}
