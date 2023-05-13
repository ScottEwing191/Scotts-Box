using UnityEngine;
using UnityEngine.Events;


namespace ScottEwing.TriggersV2{
    public class ToggleInteractTrigger : InteractTrigger{
        /*[Tooltip("The Trigger has been toggled off")]
        [SerializeField] protected UnityEvent _onTriggeredOff;

        [SerializeField] private bool _turnOnOnFirstEnter;
        [Tooltip("Turns off the trigger on trigger exit only if trigger is currently on")]
        [SerializeField] private bool _turnOffOnTriggerExit = true;*/
        private bool _triggerOn;
        private bool _firstEnterComplete = false;


        private ToggleInteractTriggerData _data;

        public ToggleInteractTrigger(TriggerV2 triggerV2, ITriggerData data = null) : base(triggerV2, data) {
            _data = (ToggleInteractTriggerData)data;
        }

        public override bool OnTriggerEnter(Collider other) {
            if (!base.OnTriggerEnter(other)) {
                return false;
            }

            if (_data._turnOnOnFirstEnter && !_firstEnterComplete) {
                _firstEnterComplete = true;
                Triggered();
            }

            return true;
        }

        public override bool OnTriggerExit(Collider other) {
            if (!base.OnTriggerExit(other)) {
                return false;
            }

            if (_data._turnOffOnTriggerExit && _triggerOn) {
                TriggerOff();
            }

            return true;
        }

        public override void Triggered(GameObject other = null) {
            _triggerOn = !_triggerOn;
            if (_triggerOn)
                base.Triggered();
            else
                TriggerOff();
        }

        private bool TriggerOff() {
            if (!TriggerV2.gameObject.activeSelf || !TriggerV2.gameObject.activeInHierarchy) return false;
            if (!TriggerV2.IsActivatable) return false;
            if (TriggerV2.isDebug)
                Debug.Log("Trigger Off", TriggerV2);
            _triggerOn = false;
            _data._onTriggeredOff.Invoke();
            return true;
        }
    }
}