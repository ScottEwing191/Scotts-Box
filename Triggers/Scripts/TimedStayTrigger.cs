using System.Collections;
using UnityEngine;

namespace ScottEwing.Triggers
{
    /// <summary>
    /// This trigger will activate once a valid collider has been inside the trigger for a given period of timew
    /// </summary>
    public class TimedStayTrigger : Trigger
    {
        [Tooltip("The duration an object must be inside the trigger before it is triggered")]
        [SerializeField] private float _durationRequiredForTrigger = 1;
        [SerializeField] private bool _cancelOnTriggerExit = true;

        private Coroutine timerRoutine;

        IEnumerator TimerRoutine() {
            yield return new WaitForSeconds(_durationRequiredForTrigger);
            timerRoutine = null;
            Triggered();
        }

        protected override void OnTriggerEnter(Collider other) {
            if (IsColliderValid(other)) {
                InvokeOnTriggerEnter(other);
                if (timerRoutine != null) {
                    StopCoroutine(timerRoutine);
                }
                timerRoutine = StartCoroutine(TimerRoutine());
            }
        }
        
        protected override void OnTriggerExit(Collider other) {
            if (IsColliderValid(other) && _cancelOnTriggerExit) {
                InvokeOnTriggerExit(other);
                if (timerRoutine != null) {
                    StopCoroutine(timerRoutine);
                }
            }
        }
    }
}
