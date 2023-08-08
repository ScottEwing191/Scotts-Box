using UnityEngine;
using UnityEngine.InputSystem;


namespace ScottEwing.Triggers {
    [AddComponentMenu("ScottEwing/Triggers/InteractTrigger")]
    public class InteractTrigger : Trigger , ITakesInput{
        [field: SerializeField] public InputActionProperty InputActionReference { get; set; }
        public bool ShouldCheckInput { get; set; }

        private void Update() => GetInput();


        public void GetInput() {
            if (!IsActivatable) return;
            if (!ShouldCheckInput) return;
            if (InputActionReference.action == null) return;
            if (InputActionReference.action.triggered) {
                Triggered(null);
            }
        }
        protected override void TriggerEntered(Collider other) {
            base.TriggerEntered(other);
            ShouldCheckInput = true;
        }

        protected override void TriggerExited(Collider other) {
            base.TriggerExited(other);
            ShouldCheckInput = false;
        }
    } 
}
