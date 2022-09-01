using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ScottEwing.Triggers{
    public class LookInteractTrigger : Trigger, ILookInteractable{
        [SerializeField] private InputActionProperty _inputActionReference;
        [SerializeField] protected float _maxInteractDistance = 1;

        private bool _isLookedAt;
        private bool _isLooking;
        [Description("Used to detect the first frame that this trigger was not looked at")]
        private bool _lookedThisFixedUpdate;

        

        private void Update() {
            if (!_isLookedAt) return;
            if (_inputActionReference.action == null) return;
            if (_inputActionReference.action.ReadValue<float>() > 0) {
                onTriggered.Invoke();
            }
        }

        protected virtual void FixedUpdate() {
            HandleFirstUpdateNotLookedAt();
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
            _lookedThisFixedUpdate = false;
        }

        public void Look(Vector3 cameraPosition) {
            if (!IsActivatable) return;

            _lookedThisFixedUpdate = true;
            if (!_isLookedAt && CanCameraActivateTrigger(cameraPosition)) {
                LookEnter();
            }
            else if (_isLookedAt && CanCameraActivateTrigger(cameraPosition)) {
                LookStay();
            }
            else if (_isLookedAt && !CanCameraActivateTrigger(cameraPosition)) {
                LookExit();
            }
        }

        public void LookEnter() {
            if (_isLookedAt) return;
            _isLookedAt = true;
            onTriggerEnter?.Invoke();
        }

        public void LookStay() {
            onTriggerStay?.Invoke();
        }

        public void LookExit() {
            if (!_isLookedAt) return;
            _isLookedAt = false;
            onTriggerExit?.Invoke();
        }

        private bool CanCameraActivateTrigger(Vector3 cameraPosition) {
            if (Vector3.Distance(cameraPosition, transform.position) > _maxInteractDistance)  return false;
            return true;
        }


        //-- Dont want base behaviour
        protected override void OnTriggerEnter(Collider other) { }
        protected override void OnTriggerStay(Collider other){ }
        protected override void OnTriggerExit(Collider other){ }
    }
}
