using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScottEwing.Triggers {
    [AddComponentMenu("ScottEwing/Triggers/TouchTrigger(deprecated)")]
    public class TouchTrigger : Trigger{
        [Tooltip("The time between the trigger being entered and the OnTriggered event being invoked")]
        //[SerializeField] private float _triggerDelay = 0.0f;
        
        protected override void OnTriggerEnter(Collider other) {
            if (!IsColliderValid(other)) return;
            InvokeOnTriggerEnter(other);
            Triggered();
            /*if (_triggerDelay == 0.0f) {
            }
            else {
                StartCoroutine(TriggeredDelayRoutine());
            }*/
        }

        /*private IEnumerator TriggeredDelayRoutine() {
            yield return new WaitForSeconds(_triggerDelay);
            Triggered();
        }*/
    } 
}
