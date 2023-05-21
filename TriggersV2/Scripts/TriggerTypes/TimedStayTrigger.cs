using System.Collections;
using UnityEngine;

namespace ScottEwing.TriggersV2{
    /// <summary>
    /// This trigger will activate once a valid collider has been inside the trigger for a given period of timew
    /// </summary>
    public class TimedStayTrigger : BaseTriggerType{
        [Tooltip("The duration an object must be inside the trigger before it is triggered")]
        //[SerializeField] private float _durationRequiredForTrigger = 1;

        //[SerializeField] private bool _cancelOnTriggerExit = true;

        private Coroutine timerRoutine;

        private TimedStayTriggerData _data;
        public TimedStayTrigger(BaseTrigger trigger, ITriggerData data = null) : base(trigger, data) {
            _data = (TimedStayTriggerData)data;
        }

        IEnumerator TimerRoutine() {
            yield return new WaitForSeconds(_data._durationRequiredForTrigger);
            timerRoutine = null;
            Trigger.Triggered();
        }

        public override bool OnTriggerEnter(Collider other) {
            if (timerRoutine != null) {
                Trigger.StopCoroutine(timerRoutine);
            }
            timerRoutine = Trigger.StartCoroutine(TimerRoutine());
            return true;
        }

        public override bool OnTriggerExit(Collider other) {
            if (_data._cancelOnTriggerExit) {
                if (timerRoutine != null) {
                    Trigger.StopCoroutine(timerRoutine);
                }
            }
            return true;
        }
    }
}