using UnityEngine;

namespace ScottEwing.Triggers{
    [AddComponentMenu("ScottEwing/Triggers/CounterTrigger")]
    public class CounterTrigger : Trigger{
        [SerializeField] private int _requiredCount = 1;
        [SerializeField] private bool _decreaseCountOnExit;
        private int _currentCount;

        protected override void TriggerEntered(Collider other) {
            base.TriggerEntered(other);
            _currentCount++;
            if (_currentCount >= _requiredCount) {
                Triggered(other);
            }
        }

        protected override void TriggerExited(Collider other) {
            base.TriggerExited(other);
            if (_decreaseCountOnExit) {
                _currentCount--;
            }
        }
    }
}