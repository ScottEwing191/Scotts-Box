using UnityEngine;

namespace ScottEwing.Triggers{
    public class CounterTrigger : Trigger{
        [SerializeField] private int _requiredCount = 1;
        [SerializeField] private bool _decreaseCountOnExit;
        private int _currentCount;

        protected override void OnTriggerEnter(Collider other) {
            if (IsColliderValid(other)) {
                base.OnTriggerEnter(other);
                _currentCount++;
                if (_currentCount >= _requiredCount) {
                    Triggered();
                }
            }
        }

        protected override void OnTriggerExit(Collider other) {
            if (_decreaseCountOnExit && IsColliderValid(other)) {
                _currentCount--;
            }

            base.OnTriggerExit(other);

        }
    }
}
