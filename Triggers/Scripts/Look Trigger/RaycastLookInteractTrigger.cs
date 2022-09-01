using System;
using System.Collections;
using System.Collections.Generic;
using ScottEwing.Triggers;
using Unity.VisualScripting;
using UnityEngine;

namespace ScottEwing.Triggers{
    public enum CastSourceType{
        UseMainCameraTransform,
        UseAssignedTransform
    }
    
    public class RaycastLookInteractTrigger : LookInteractTrigger, ISphereCastInteractor{
        [Tooltip("The game object that need to be looked at. Default is the game object this is attached to. If different, default and _lookTarget should be under same parent (untested)")]
        [SerializeField] private Transform _lookTarget;

        [field: SerializeField] public float SphereCastRadius { get; set; } = 0.1f;
        [field: SerializeField] public LayerMask CollisionLayers { get; set; } = ~0;
        [field: SerializeField] public QueryTriggerInteraction TriggerInteraction { get; set; } = QueryTriggerInteraction.Ignore;
        [field: SerializeField] public CastSourceType CastSourceType { get; set; } = CastSourceType.UseMainCameraTransform;
        [field: SerializeField] public Transform AssignedSource { get; set; }
        public Transform CurrentSource { get; set; }

        private bool _performSphereCast;

        private void Awake() {
            _lookTarget ??= gameObject.transform;
        }

        private void Start() {
            switch (CastSourceType) {
                case CastSourceType.UseMainCameraTransform:
                    if (Camera.main) {
                        CurrentSource = Camera.main.transform;
                    }
                    break;
                case CastSourceType.UseAssignedTransform:
                    CurrentSource = AssignedSource;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void FixedUpdate() {
            DoRaycast();
            base.FixedUpdate();
        }

        private void DoRaycast() {
            if (!_performSphereCast) return;
            if (!Physics.SphereCast(CurrentSource.position, SphereCastRadius, CurrentSource.forward, out RaycastHit hit, _maxInteractDistance, CollisionLayers.value, TriggerInteraction)) return;
            if (!hit.transform.IsChildOf(_lookTarget)) return;
            Look(CurrentSource.position);
        }

        protected override void OnTriggerEnter(Collider other) {
            if (other.CompareTag(_triggeredByTag)) {
                _performSphereCast = true;
            }
        }

        protected override void OnTriggerStay(Collider other) {
        }

        protected override void OnTriggerExit(Collider other) {
            if (other.CompareTag(_triggeredByTag)) {
                _performSphereCast = false;
            }
        }

        
    }
}