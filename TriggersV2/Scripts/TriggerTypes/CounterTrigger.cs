using UnityEngine;

namespace ScottEwing.TriggersV2{
    public class CounterTrigger : BaseTrigger{
        private int _currentCount;
        private CounterTriggerData _data;
        public CounterTrigger(TriggerV2 triggerV2, ITriggerData data = null) : base(triggerV2, data) {
            _data = (CounterTriggerData)data;
        }

        public override bool OnTriggerEnter(Collider other) {
            _currentCount++;
            if (_currentCount >= _data._requiredCount) {
                TriggerV2.Triggered();
            }
            return true;
        }

        public override bool OnTriggerExit(Collider other) {
            if (_data._decreaseCountOnExit && _currentCount>0) {
                _currentCount--;
            }
            return true;
        }
    }
}