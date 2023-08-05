using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScottEwing.Triggers {
    [AddComponentMenu("ScottEwing/Triggers/TouchTrigger")]
    public class TouchTrigger : Trigger{
        protected override void TriggerEntered(Collider other) {
            base.TriggerEntered(other);
            Triggered();
        }
    } 
}
