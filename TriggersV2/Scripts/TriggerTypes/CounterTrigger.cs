using UnityEngine;

namespace ScottEwing.TriggersV2{
    public class CounterTrigger : BaseTriggerType{
        private int _currentCount;
        private CounterTriggerData _data;
        public CounterTrigger(BaseTrigger trigger, ITriggerData data = null) : base(trigger, data) {
            _data = (CounterTriggerData)data;
        }

        public override bool OnTriggerEnter(Collider other) {
            _currentCount++;
            if (_currentCount >= _data._requiredCount) {
                Trigger.Triggered();
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