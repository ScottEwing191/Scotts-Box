using UnityEngine;

namespace ScottEwing.TriggersV2{
    public class CastInteractor : MonoBehaviour{
        [field: SerializeField] public RayCastHelper Caster { get; set; } = new RayCastHelper();

        [SerializeField] private bool _isDebug;

        // It would be nice to use update options but would need to make sure that the same option is picked in LookInteractTrigger
        // When checking calling HandleFirstUpdateNotLookedAt
        [Tooltip("If using to look at Look Interact Triggers then Fixed Update must be used")]
        [SerializeField] private UpdateOptions _updateOptions = UpdateOptions.FixedUpdate;

        private void Update() {
            if (_updateOptions == UpdateOptions.Update) {
                LookForInteractable();
            }
        }

        private void LateUpdate() {
            if (_updateOptions == UpdateOptions.LateUpdate) {
                LookForInteractable();
            }
        }

        private void FixedUpdate() {
            if (_updateOptions == UpdateOptions.FixedUpdate) {
                LookForInteractable();
            }
        }
        
        public TriggerState? LookForInteractable(out ILookInteractable interactable) {
            interactable = null;
            if (!Caster.ConditionalSpecifyTargetsCast(out var castHit))
                return null;
            var gotComp = castHit.collider.TryGetComponent(out interactable);
            if (!gotComp) {
                if (_isDebug) 
                    Debug.Log("Couldn't find Look Interactable. Hit: " + castHit.collider.name, this);
                return null;
            }
            if (_isDebug) 
                Debug.Log("Looking at" + interactable, this);
            return interactable.Look(castHit.collider);
        }

        public TriggerState? LookForInteractable() => LookForInteractable(out ILookInteractable interactable);
    }
}