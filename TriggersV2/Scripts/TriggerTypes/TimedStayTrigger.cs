using System.Collections;
using UnityEngine;

namespace ScottEwing.TriggersV2{
    /// <summary>
    /// This trigger will activate once a valid collider has been inside the trigger for a given period of timew
    /// </summary>
    public class TimedStayTrigger : BaseTrigger{
        [Tooltip("The duration an object must be inside the trigger before it is triggered")]
        //[SerializeField] private float _durationRequiredForTrigger = 1;

        //[SerializeField] private bool _cancelOnTriggerExit = true;

        private Coroutine timerRoutine;

        private TimedStayTriggerData _data;
        public TimedStayTrigger(TriggerV2 triggerV2, ITriggerData data = null) : base(triggerV2, data) {
            _data = (TimedStayTriggerData)data;
        }

        IEnumerator TimerRoutine() {
            yield return new WaitForSeconds(_data._durationRequiredForTrigger);
            timerRoutine = null;
            TriggerV2.Triggered();
        }

        public override bool OnTriggerEnter(Collider other) {
            if (timerRoutine != null) {
                TriggerV2.StopCoroutine(timerRoutine);
            }
            timerRoutine = TriggerV2.StartCoroutine(TimerRoutine());
            return true;
        }

        public override bool OnTriggerExit(Collider other) {
            if (_data._cancelOnTriggerExit) {
                if (timerRoutine != null) {
                    TriggerV2.StopCoroutine(timerRoutine);
                }
            }
            return true;
        }
    }
}