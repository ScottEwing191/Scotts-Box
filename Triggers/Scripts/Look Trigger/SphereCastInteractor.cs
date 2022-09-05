using System;
using UnityEngine;

namespace ScottEwing.Triggers{
    public class SphereCastInteractor : MonoBehaviour, ISphereCastInteractor{
        [SerializeField] private float _lookDistance = 5;

        [field: SerializeField] public float SphereCastRadius { get; set; } = 0.1f;
        [field: SerializeField] public LayerMask CollisionLayers { get; set; } = ~0;
        [field: SerializeField] public QueryTriggerInteraction TriggerInteraction { get; set; } = QueryTriggerInteraction.Ignore;
       
        [SerializeField] private LookSource _source = new LookSource();

        
        private void FixedUpdate() {
            DoRaycast();
        }

        private void DoRaycast() {
            if (!Physics.SphereCast(_source.CurrentSource.position, SphereCastRadius, _source.CurrentSource.forward, out RaycastHit hit, _lookDistance, CollisionLayers.value, TriggerInteraction)) return;
            if (!hit.collider.TryGetComponent(out ILookInteractable interactable)) return;
            interactable.Look(_source.CurrentSource.position);
        }

        
    }
}