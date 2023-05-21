using UnityEngine;

namespace ScottEwing.TriggersV2 {
    public class InteractTrigger : BaseTriggerType , ITakesInput{

        public bool ShouldCheckInput { get; set; }
        private readonly InteractTriggerData _data;
        public InteractTrigger(BaseTrigger trigger, ITriggerData data = null) : base(trigger, data) {
            _data = (InteractTriggerData)data;
        }

        public override void Update() => GetInput();

        public void GetInput() {
            if (!Trigger.IsActivatable) return;
            if (!ShouldCheckInput) return;
            if (_data.InputActionReference.action == null) return;
            if (_data.InputActionReference.action.triggered) {
                Triggered();
            }
        }

        public override void Triggered(GameObject other = null) {
            base.Triggered(other);
        }

        public override bool OnTriggerEnter(Collider other) {
            ShouldCheckInput = true;
            return true;
        }

        public override bool OnTriggerExit(Collider other) {
            ShouldCheckInput = false;
            return true;
        }

    } 
}
