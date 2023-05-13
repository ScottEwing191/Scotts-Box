using UnityEngine;
using UnityEngine.InputSystem;


namespace ScottEwing.TriggersV2 {
    public class InteractTrigger : BaseTrigger , ITakesInput{

        //[field: SerializeField] public InputActionProperty InputActionReference { get; set; }
        //public InputActionProperty InputActionReference { get; set; }
        public bool ShouldCheckInput { get; set; }
        //private InteractTriggerData _data;
        private readonly InteractTriggerData _data;
        public InteractTrigger(TriggerV2 triggerV2, ITriggerData data = null) : base(triggerV2, data) {
            //InputActionReference = ((InteractTriggerData)data).InputActionReference;
            _data = (InteractTriggerData)data;

        }
        
        //Convert to Routine
        public override void Update() => GetInput();


        public void GetInput() {
            if (!TriggerV2.IsActivatable) return;
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
