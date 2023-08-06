using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScottEwing.Triggers {
    [AddComponentMenu("ScottEwing/Triggers/TouchTrigger")]
    public class TouchTrigger : Trigger{
        [Tooltip("The delay before the trigger is activated")]
        [SerializeField] private float _triggerDelay = 0;
        
        //TriggerEnter method that will wait for a delay before triggering if the delay is greater than 0
        protected override void TriggerEntered(Collider other) {
            base.TriggerEntered(other);
            if (_triggerDelay > 0)
                StartCoroutine(DelayedTrigger(other));
            else
                Triggered(other);
        }

        private IEnumerator DelayedTrigger(Collider other ) {
            yield return new WaitForSeconds(_triggerDelay);
            Triggered(other);
        }
    } 
}
