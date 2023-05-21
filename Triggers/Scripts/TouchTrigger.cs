using UnityEngine;

namespace ScottEwing.Triggers {
    [AddComponentMenu("ScottEwing/Triggers/TouchTrigger(deprecated)")]
    public class TouchTrigger : Trigger {
        protected override void OnTriggerEnter(Collider other) {
            if (!IsColliderValid(other)) return;
            InvokeOnTriggerEnter(other);
            Triggered();
        }
    } 
}
