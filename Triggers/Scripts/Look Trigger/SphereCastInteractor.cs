using System;
using UnityEngine;

namespace ScottEwing.Triggers{
    public class SphereCastInteractor : MonoBehaviour, ISphereCastInteractor{
        [SerializeField] private float _lookDistance = 5;

        [field: SerializeField] public float SphereCastRadius { get; set; } = 0.1f;
        [field: SerializeField] public LayerMask CollisionLayers { get; set; } = ~0;
        [field: SerializeField] public QueryTriggerInteraction TriggerInteraction { get; set; } = QueryTriggerInteraction.Ignore;
        [field: SerializeField] public CastSourceType CastSourceType { get; set; } = CastSourceType.UseMainCameraTransform;
        [field: SerializeField] public Transform AssignedSource { get; set; }
        public Transform CurrentSource { get; set; }

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
        
        
        private void FixedUpdate() {
            DoRaycast();
        }

        private void DoRaycast() {
            if (!Physics.SphereCast(CurrentSource.position, SphereCastRadius, CurrentSource.forward, out RaycastHit hit, _lookDistance, CollisionLayers.value, TriggerInteraction)) return;
            if (!hit.collider.TryGetComponent(out ILookInteractable interactable)) return;
            interactable.Look(CurrentSource.position);
        }

        
    }
}