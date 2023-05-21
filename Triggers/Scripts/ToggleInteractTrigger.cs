using UnityEngine;
using UnityEngine.Events;


namespace ScottEwing.Triggers {
    [AddComponentMenu("ScottEwing/Triggers/ToggleInteractTrigger(deprecated)")]
    public class ToggleInteractTrigger : InteractTrigger{
        [Tooltip("The Trigger has been toggled off")]
        [SerializeField] protected UnityEvent _onTriggeredOff;

        private bool _triggerOn;
        [SerializeField] private bool _turnOnOnFirstEnter;
        [Tooltip("Turns off the trigger on trigger exit only if trigger is currently on")]
        [SerializeField] private bool _turnOffOnTriggerExit = true;
        private bool _firstEnterComplete = false;

        protected override void OnTriggerEnter(Collider other) {
            if (!IsColliderValid(other)) return;
            if (_turnOnOnFirstEnter && !_firstEnterComplete) {
                _firstEnterComplete = true;
                Triggered();
            }
            base.OnTriggerEnter(other);
        }

        protected override void OnTriggerExit(Collider other) {
            if (!IsColliderValid(other)) return;
            if (_turnOffOnTriggerExit && _triggerOn) {
                TriggerOff();
            }
            base.OnTriggerExit(other);
        }

        protected override bool Triggered() {
            _triggerOn = !_triggerOn;
            return _triggerOn ? base.Triggered() : TriggerOff();
        }

        private bool TriggerOff() {
            if (!gameObject.activeSelf || !gameObject.activeInHierarchy) return false;
            if (!IsActivatable) return false;
            if (isDebug)
                Debug.Log("Trigger Off", this);
            _triggerOn = false;
            _onTriggeredOff.Invoke();
            return true;
        }
    } 
}
