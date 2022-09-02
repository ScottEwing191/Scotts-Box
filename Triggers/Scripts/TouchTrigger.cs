using UnityEngine;

namespace ScottEwing.Triggers {
    public class TouchTrigger : Trigger {
        protected override void OnTriggerEnter(Collider other) {
            if (other.CompareTag(_triggeredByTag) && IsActivatable) {
                Triggered();
            }
        }
    } 
}
