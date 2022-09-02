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
            CooldownOnTriggered
        }

        [SerializeField] protected string _triggeredByTag = "Player";
        [SerializeField] private TriggeredType _triggeredType = TriggeredType.DestroyOnTriggered;
#if ODIN_INSPECTOR
        [ShowIf("_triggeredType", TriggeredType.CooldownOnTriggered)]
#endif
        [SerializeField] private float _cooldownTime = 2.0f;

        [SerializeField] protected bool _activateOnce = true;
        [field: SerializeField] protected bool IsActivatable { get; set; } = true;

        [SerializeField] protected UnityEvent _onTriggered;
        [SerializeField] protected UnityEvent _onTriggerEnter;
        [SerializeField] protected UnityEvent _onTriggerStay;
        [SerializeField] protected UnityEvent _onTriggerExit;

        private Coroutine _cooldownRoutine;

        //private void OnEnable() => _onTriggered.AddListener(DestroyOnTriggered);
        //private void OnDisable() => _onTriggered.AddListener(DestroyOnTriggered);
        protected virtual void OnTriggerEnter(Collider other) => _onTriggerEnter?.Invoke();
        protected virtual void OnTriggerStay(Collider other) => _onTriggerStay?.Invoke();
        protected virtual void OnTriggerExit(Collider other) => _onTriggerExit?.Invoke();

        /*private void DestroyOnTriggered() {
            if (_activateOnce) {
                Destroy(this.gameObject);
            }
        }*/


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