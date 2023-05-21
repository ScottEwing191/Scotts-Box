using UnityEngine;
using UnityEngine.InputSystem;

namespace ScottEwing.TriggersV2{
    public class LookInteractTrigger : LookTrigger, ITakesInput{
        //[field:SerializeField] public InputActionProperty InputActionReference { get; set; }
        public bool ShouldCheckInput { get; set; } // this is not required since bool LookingAtTrigger exists
        private LookInteractTriggerData _data;

        public LookInteractTrigger(BaseTrigger trigger, ITriggerData data = null) : base(trigger, data) {
            _data = (LookInteractTriggerData)data;
        }

        public override void Update() => GetInput();

        public void GetInput() {
            if (!Trigger.IsActivatable) return;
            if (!LookingAtTrigger) return;
            if (_data.InputActionReference.action == null) return;
            if (_data.InputActionReference.action.triggered) {
                Triggered();
            }
        }
    }
}