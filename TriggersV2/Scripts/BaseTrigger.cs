using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using NaughtyAttributes;
#endif

// The Trigger Type only works if triggered is called. Triggered is not called from this class
// It must be called from a derived class. This is why this was an abstract class.
// Will still keep non abstract but should be aware that trigger type will have no effect 
namespace ScottEwing.TriggersV2{
    public enum TriggerState{
        Enter,
        Stay,
        Exit,
        None
    }
    [AddComponentMenu("ScottEwing/Triggers/Basic Trigger")]
    public  class BaseTrigger : MonoBehaviour{
        protected enum TriggeredBehaviour{
            DestroyOnTriggered,
            DisableOnTriggered,
            CooldownOnTriggered,
            RemainActive,
            DisableTriggerComponent
        }
        protected BaseTriggerType _trigger;

        [SerializeField] private TriggerBy _triggerBy = new TriggerBy();

        [SerializeField] private TriggeredBehaviour _triggeredBehaviour = TriggeredBehaviour.DestroyOnTriggered;

        [ShowIf("_triggeredBehaviour", TriggeredBehaviour.CooldownOnTriggered)]
        [SerializeField] private float _cooldownTime = 2.0f;

        [field: SerializeField] public bool IsActivatable { get; set; } = true;
        [SerializeField] public bool isDebug;
        [SerializeField] private bool _invokeOnTriggerExitWhenTriggered = false;
        private Coroutine _cooldownRoutine;
        protected Collider currentCollider;

        [SerializeField] protected UnityEvent _onTriggered;

        //--Trigger Unity Events
        [Header("Trigger Events")]
        [SerializeField] private bool _useTriggerEvents = true;

        [ShowIf("_useTriggerEvents")]
        [SerializeField] protected UnityEvent<Collider> _onTriggerEnter;

        [ShowIf("_useTriggerEvents")]
        [SerializeField] protected UnityEvent<Collider> _onTriggerStay;

        [ShowIf("_useTriggerEvents")]
        [SerializeField] protected UnityEvent<Collider> _onTriggerExit;

        //--Collision Unity Events
        [Header("Collision Events")]
        [SerializeField] private bool _useCollisionEvents = false;

        [ShowIf("_useCollisionEvents")]
        [SerializeField] protected UnityEvent<Collision> _onCollisionEnter;
        
        [ShowIf("_useCollisionEvents")]
        [SerializeField] protected UnityEvent<Collision> _onCollisionStay;
        
        [ShowIf("_useCollisionEvents")]
        [SerializeField] protected UnityEvent<Collision> _onCollisionExit;

        protected virtual void Awake() {
            if (_trigger == null) {
                _trigger = new BaseTriggerType(this);
            }
            _trigger?.Awake();
        }

        protected virtual void Start() => _trigger.Start();
        protected virtual void Update() => _trigger.Update();
        protected virtual void FixedUpdate() => _trigger.FixedUpdate();
        protected virtual void LateUpdate() => _trigger.LateUpdate();

        protected virtual void InitialiseTrigger() { }

        #region On Trigger Events

        protected virtual void OnTriggerEnter(Collider other) {
            if (_triggerBy.IsColliderValid(other, IsActivatable) && _trigger.OnTriggerEnter(other)) {
                if (_useTriggerEvents) {
                    InvokeOnTriggerEnter(other);
                }
            }
        }

        protected virtual void OnTriggerStay(Collider other) {
            if (_triggerBy.IsColliderValid(other, IsActivatable) && _trigger.OnTriggerStay(other)) {
                if (_useTriggerEvents) {
                    InvokeOnTriggerStay(other);
                }
            }
        }

        protected virtual void OnTriggerExit(Collider other) {
            if (_triggerBy.IsColliderValid(other, IsActivatable) && _trigger.OnTriggerExit(other)) {
                if (_useTriggerEvents) {
                    InvokeOnTriggerExit(other);
                }
            }
        }

        #endregion


        #region On Collision Events
        protected virtual void OnCollisionEnter(Collision other) {
            if (_triggerBy.IsCollisionValid(other, IsActivatable) && _trigger.OnCollisionEnter(other)) {
                if (_useCollisionEvents) {
                    InvokeOnCollisionEnter(other);
                }
            }
        }

        protected virtual void OnCollisionStay(Collision other) {
            if (_triggerBy.IsCollisionValid(other, IsActivatable) && _trigger.OnCollisionStay(other)) {
                if (_useCollisionEvents) {
                    InvokeOnCollisionStay(other);
                }
            }
        }

        protected virtual void OnCollisionExit(Collision other) {
            if (_triggerBy.IsCollisionValid(other, IsActivatable) && _trigger.OnCollisionExit(other)) {
                if (_useCollisionEvents) {
                    InvokeOnCollisionExit(other);
                }
            }
        }

        #endregion


        #region Invoke Trigger Unity EVents

        protected internal TriggerState InvokeOnTriggerEnter(Collider other) {
            if (isDebug)
                Debug.Log("OnTriggerEnter", this);
            _onTriggerEnter?.Invoke(other);
            currentCollider = other;
            return TriggerState.Enter;
        }

        protected internal TriggerState InvokeOnTriggerStay(Collider other = null) {
            if (isDebug)
                Debug.Log("OnTriggerStay", this);

            _onTriggerStay?.Invoke(other);
            return TriggerState.Stay;
        }

        // The collider passed will be null on the following occasions. The trigger has been triggered while invokeOnTriggerExitOnTrigger
        // is true. It will also be null when it is called after Look Interact Trigger Looks away from it collider
        protected internal TriggerState InvokeOnTriggerExit(Collider other) {
            if (isDebug)
                Debug.Log("OnTriggerExit", this);
            var collider = (other != null) ? other : currentCollider;
            if (other == null) {
                Debug.LogWarning("Trigger Exit: Other collider is null. Using collider saved from OnTriggerEnter" +
                                 "This may cause issues if interacting with multiple triggers at once");
            }

            _onTriggerExit?.Invoke(collider);
            currentCollider = null;
            return TriggerState.Exit;
        }

        #endregion

        #region Invoke Collision Unity Events

        protected void InvokeOnCollisionEnter(Collision other) {
            if (isDebug)
                Debug.Log("OnCollisionEnter", this);
            _onCollisionEnter?.Invoke(other);
        }

        protected void InvokeOnCollisionStay(Collision other) {
            if (isDebug)
                Debug.Log("OnCollisionStay", this);
            _onCollisionStay?.Invoke(other);
        }

        protected void InvokeOnCollisionExit(Collision other) {
            if (isDebug)
                Debug.Log("OnCollisionExit", this);
            _onCollisionExit?.Invoke(other);
        }

        #endregion

        public virtual bool Triggered() {
            if (!gameObject.activeSelf || !gameObject.activeInHierarchy) return false;
            if (!IsActivatable) return false;
            if (isDebug)
                Debug.Log("Triggered", this);
            _onTriggered.Invoke();
            switch (_triggeredBehaviour) {
                case TriggeredBehaviour.DestroyOnTriggered:
                    Destroy(gameObject);
                    break;
                case TriggeredBehaviour.DisableOnTriggered:
                    DisableTriggerObject();
                    break;
                case TriggeredBehaviour.CooldownOnTriggered:
                    StartCooldown();
                    break;
                case TriggeredBehaviour.DisableTriggerComponent:
                    DisableTriggerComponent();
                    break;
            }

            if (_invokeOnTriggerExitWhenTriggered) {
                InvokeOnTriggerExit(null);
            }

            return true;
        }

        private void StartCooldown() {
            _cooldownRoutine = StartCoroutine(Cooldown());

            IEnumerator Cooldown() {
                IsActivatable = false;
                yield return new WaitForSeconds(_cooldownTime);
                _cooldownRoutine = null;
                IsActivatable = true;
            }
        }

        private void CancelCooldown() {
            if (_cooldownRoutine == null) return;
            StopCoroutine(_cooldownRoutine);
            IsActivatable = true;
        }


        public void DisableTriggerObject() => gameObject.SetActive(false);

        private void DisableTriggerComponent() => this.enabled = false;
    }
}