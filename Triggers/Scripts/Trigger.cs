using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif


namespace ScottEwing.Triggers{
    public abstract class Trigger : MonoBehaviour{
        private enum TriggeredType{
            DestroyOnTriggered,
            DisableOnTriggered,
            CooldownOnTriggered,
            RemainActive
        }

        /// <summary>
        /// States are anly valid for the frame they are received
        /// </summary>
        public enum TriggerState{
            Enter,
            Stay,
            Exit,
            None
        }

        [SerializeField] protected string _triggeredByTag = "Player";
        [SerializeField] private TriggeredType _triggeredType = TriggeredType.DestroyOnTriggered;
#if ODIN_INSPECTOR
        [ShowIf("_triggeredType", TriggeredType.CooldownOnTriggered)]
#endif
        [SerializeField] private float _cooldownTime = 2.0f;

        [field: SerializeField] protected bool IsActivatable { get; set; } = true;

        [SerializeField] private bool _invokeOnTriggerExitWhenTriggered = false;
        [SerializeField] protected UnityEvent _onTriggered;
        [SerializeField] protected UnityEvent _onTriggerEnter;
        [SerializeField] protected UnityEvent _onTriggerStay;
        [SerializeField] protected UnityEvent _onTriggerExit;

        private Coroutine _cooldownRoutine;

        protected virtual void OnTriggerEnter(Collider other) => InvokeOnTriggerEnter();
        protected virtual void OnTriggerStay(Collider other) => InvokeOnTriggerStay();
        protected virtual void OnTriggerExit(Collider other) => InvokeOnTriggerExit();


        protected virtual TriggerState InvokeOnTriggerEnter() {
            _onTriggerEnter?.Invoke();
            return TriggerState.Enter;
        }

        protected virtual TriggerState InvokeOnTriggerStay() {
            _onTriggerStay?.Invoke();
            return TriggerState.Stay;
        }

        protected virtual TriggerState InvokeOnTriggerExit() {
            _onTriggerExit?.Invoke();
            return TriggerState.Exit;
        }


        protected void Triggered() {
            if (!gameObject.activeSelf || !gameObject.activeInHierarchy) return;
            if (!IsActivatable) return;

            _onTriggered.Invoke();
            switch (_triggeredType) {
                case TriggeredType.DestroyOnTriggered:
                    Destroy(gameObject);
                    break;
                case TriggeredType.DisableOnTriggered:
                    DisableTriggerObject();
                    break;
                case TriggeredType.CooldownOnTriggered:
                    StartCooldown();
                    break;
            }

            if (_invokeOnTriggerExitWhenTriggered) {
                InvokeOnTriggerExit();
            }
        }

        public void StartCooldown() {
            _cooldownRoutine = StartCoroutine(Cooldown());

            IEnumerator Cooldown() {
                IsActivatable = false;
                yield return new WaitForSeconds(_cooldownTime);
                _cooldownRoutine = null;
                IsActivatable = true;
            }
        }

        public void CancelCooldown() {
            if (_cooldownRoutine == null) return;
            StopCoroutine(_cooldownRoutine);
            IsActivatable = true;
        }


        public void DisableTriggerObject() {
            gameObject.SetActive(false);
        }
    }
}