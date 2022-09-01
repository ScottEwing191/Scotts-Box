using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScottEwing.Triggers{
    public class RayInteractor : MonoBehaviour{
        [SerializeField] private float _castRadius = 0.1f;
        [SerializeField] private float _lookDistance = 5;
        [SerializeField] private LayerMask _castHitLayers;
        
        [SerializeField] private Camera _camera;
        
        private RaycastHit[] _hits = new RaycastHit[10];
        private ILookInteractable _previousLookInteractable;

        private void FixedUpdate() {
            SingleHit();
        }

        private void SingleHit() {
            if (Physics.SphereCast(transform.position, _castRadius, transform.forward, out RaycastHit hit, _lookDistance, _castHitLayers.value, QueryTriggerInteraction.Collide)) {
                Debug.DrawLine(transform.position, hit.point);


                if (hit.collider.TryGetComponent<ILookInteractable>(out ILookInteractable interactable)) {
                    /*if (_previousLookInteractable != null && interactable != _previousLookInteractable) {
                        _previousLookInteractable = interactable;
                        interactable.LookExit();
                        return;
                    }*/

                    interactable.Look(_camera.transform.position);
                    _previousLookInteractable = interactable;
                }
                else CheckLookExit();
            }
            else CheckLookExit();

        }

        public void CheckLookExit() {
            if (_previousLookInteractable != null) {
                //_previousLookInteractable.LookExit();
                _previousLookInteractable = null;
            }
        }

        public bool CheckLookExit(ILookInteractable interactable) {
            if (_previousLookInteractable == null) return false;
                
            
            if (interactable != _previousLookInteractable) {
                _previousLookInteractable = interactable;
                interactable.LookExit();
                return true;
            }

            return false;
        }

        /*private void MultiHit() {
            var hits = Physics.SphereCastNonAlloc(transform.position, _castRadius, transform.forward, this._hits, _lookDistance, _castHitLayers.value, QueryTriggerInteraction.Collide);
            if (hits == 0) return;

        }*/
    }
}
