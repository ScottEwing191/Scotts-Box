using ScottEwing.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ScottEwing.Honours
{
    [AddComponentMenu("ScottEwing/Triggers/LookInteractTrigger(deprecated)")]
    public class LookInteractTrigger : LookTrigger, ITakesInput
    {
        [field:SerializeField] public InputActionProperty InputActionReference { get; set; }
        public bool ShouldCheckInput { get; set; }  // this is not required since bool LookingAtTrigger exists

        private void Update() => GetInput();

        public void GetInput() {
            if (!IsActivatable) return;
            if (!LookingAtTrigger) return;
            if (InputActionReference.action == null) return;
            if (InputActionReference.action.triggered) {
                Triggered();
            }
        }
    }
}
