using System;
using System.Collections;
using System.Runtime.InteropServices;
using ScottEwing.ExtensionMethods;
using UnityEngine;
using UnityEngine.Events;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif


// The Trigger Type only works if triggered is called. Triggered is not called from this class
// It must be called from a derived class. This is why this was an abstract class.
// Will still keep non abstract but should be aware that trigger type will have no effect 
namespace ScottEwing.TriggersV2{
        /// <summary>
        /// States are anly valid for the frame they are received
        /// </summary>
        public enum TriggerState{
            Enter,
            Stay,
            Exit,
            None
        }
    public class TriggerV2 : MonoBehaviour{
        private enum TriggeredBehaviour{
            DestroyOnTriggered,
            DisableOnTriggered,
            CooldownOnTriggered,
            RemainActive,
            DisableTriggerComponent
        }
        
        private enum TriggeredBy{Tag,LayerMask}

        private enum TriggerType{
            Base,Touch, Interact, ToggleInteract, Checkpoint, Look, LookInteract, TimedStay, Audio, Counter 
        }

        [SerializeField]private TriggerType _triggerType = TriggerType.Base;
        #if ODIN_INSPECTOR
        [ShowIf("_triggerType", TriggerType.Interact)]
        #endif
        
        [SerializeField] private InteractTriggerData _interactTriggerData;
        [ShowIf("_triggerType", TriggerType.ToggleInteract)]
        [SerializeField] private ToggleInteractTriggerData _toggleInteractTriggerData;
        [ShowIf("_triggerType", TriggerType.Checkpoint)]
        [SerializeField] private CheckpointTouchTriggerData _checkpointTouchTriggerData;
        [ShowIf("_triggerType", TriggerType.Look)]
        [SerializeField] private LookTriggerData _lookTriggerData;
        [ShowIf("_triggerType", TriggerType.LookInteract)]
        [SerializeField] private LookInteractTriggerData _lookInteractTriggerData;
        [ShowIf("_triggerType", TriggerType.TimedStay)]
        [SerializeField] private TimedStayTriggerData _timedStayTriggerData;
        [ShowIf("_triggerType", TriggerType.Counter)]
        [SerializeField] private CounterTriggerData _counterTriggerData;
        

        


        private BaseTrigger _trigger;
        
        [SerializeField] private TriggeredBy _triggeredBy = TriggeredBy.Tag;
#if ODIN_INSPECTOR
        [ShowIf("_triggeredBy", TriggeredBy.LayerMask)]
#endif
        [SerializeField] private LayerMask _triggeredByMask;

        [ShowIf("_triggeredBy", TriggeredBy.Tag)]
        [SerializeField] protected string _triggeredByTag = "Player";
        [SerializeField] private TriggeredBehaviour _triggeredBehaviour = TriggeredBehaviour.DestroyOnTriggered;
#if ODIN_INSPECTOR
        [ShowIf("_triggeredBehaviour", TriggeredBehaviour.CooldownOnTriggered)]
#endif
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
#if ODIN_INSPECTOR
        [ShowIf("_useTriggerEvents")]
#endif
        [SerializeField] protected UnityEvent<Collider> _onTriggerEnter;
#if ODIN_INSPECTOR
        [ShowIf("_useTriggerEvents")]
#endif
        [SerializeField] protected UnityEvent<Collider> _onTriggerStay;
#if ODIN_INSPECTOR
        [ShowIf("_useTriggerEvents")]
#endif
        [SerializeField] protected UnityEvent<Collider> _onTriggerExit;

        //--Collision Unity Events
        [Header("Collision Events")]
        [SerializeField] private bool _useCollisionEvents = false;
#if ODIN_INSPECTOR
        [ShowIf("_useCollisionEvents")]
#endif
        [SerializeField] protected UnityEvent<Collision> _onCollisionEnter;

#if ODIN_INSPECTOR
        [ShowIf("_useCollisionEvents")]
#endif
        [SerializeField] protected UnityEvent<Collision> _onCollisionStay;

#if ODIN_INSPECTOR
        [ShowIf("_useCollisionEvents")]
#endif
        [SerializeField] protected UnityEvent<Collision> _onCollisionExit;

        
        private void InitialiseTrigger() {
            _trigger = _triggerType switch {
                TriggerType.Base => new BaseTrigger(this),
                TriggerType.Touch => new TouchTrigger(this),
                TriggerType.Interact => new InteractTrigger(this, _interactTriggerData),
                TriggerType.ToggleInteract => new ToggleInteractTrigger(this, _toggleInteractTriggerData),
                TriggerType.Checkpoint => new CheckpointTouchTrigger(this, _checkpointTouchTriggerData),
                TriggerType.Look => new LookTrigger(this, _lookTriggerData),
                TriggerType.LookInteract => new LookInteractTrigger(this, _lookInteractTriggerData),
                TriggerType.TimedStay => new TimedStayTrigger(this, _timedStayTriggerData),
                TriggerType.Audio => new AudioTouchTrigger(this),
                TriggerType.Counter => new CounterTrigger(this, _counterTriggerData),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void Awake() {
            InitialiseTrigger();
            _trigger.Awake();
        }
        private void Start() => _trigger.Start();
        private void Update() => _trigger.Update();
        private void FixedUpdate() => _trigger.FixedUpdate();
        private void LateUpdate() => _trigger.LateUpdate();


        #region On Trigger Events

        protected virtual void OnTriggerEnter(Collider other) {
            if (IsColliderValid(other) && _trigger.OnTriggerEnter(other)) {
                InvokeOnTriggerEnter(other);
            }
        }

        protected virtual void OnTriggerStay(Collider other) {
            if (IsColliderValid(other)&& _trigger.OnTriggerStay(other)) {
                InvokeOnTriggerStay(other);
            }
        }

        protected virtual void OnTriggerExit(Collider other) {
            if (IsColliderValid(other)&& _trigger.OnTriggerExit(other)) {
                InvokeOnTriggerExit(other);
            }
        }

        #endregion


        #region On Collision Events

        protected virtual void OnCollisionEnter(Collision other) {
            if (IsCollisionValid(other) && _trigger.OnCollisionEnter(other)) {
                InvokeOnCollisionEnter(other);
            }
        }

        protected virtual void OnCollisionStay(Collision other) {
            if (IsCollisionValid(other) && _trigger.OnCollisionStay(other)) {
                InvokeOnCollisionStay(other);
            }
        }

        protected virtual void OnCollisionExit(Collision other) {
            if (IsCollisionValid(other) && _trigger.OnCollisionExit(other)) {
                InvokeOnCollisionExit(other);
            }
        }

        #endregion


        #region Invoke Trigger Unity EVents

        protected internal virtual TriggerState InvokeOnTriggerEnter(Collider other) {
            if (isDebug) 
                Debug.Log("OnTriggerEnter", this);
            _onTriggerEnter?.Invoke(other);
            currentCollider = other;
            return TriggerState.Enter;
        }

        protected internal virtual TriggerState InvokeOnTriggerStay(Collider other = null) {
            if (isDebug)
                Debug.Log("OnTriggerStay", this);       
            
            _onTriggerStay?.Invoke(other);
            return TriggerState.Stay;
        }

        // The collider passed will be null on the following occasions. The trigger has been triggered while invokeOnTriggerExitOnTrigger
        // is true. It will also be null when it is called after Look Interact Trigger Looks away from it collider
        protected internal virtual TriggerState InvokeOnTriggerExit(Collider other) {
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

        protected virtual TriggerState InvokeOnCollisionEnter(Collision other) {
            if (isDebug) 
                Debug.Log("OnCollisionEnter", this);
            _onCollisionEnter?.Invoke(other);
            return TriggerState.Enter;
        }

        protected virtual TriggerState InvokeOnCollisionStay(Collision other) {
            if (isDebug)
                Debug.Log("OnCollisionStay", this);       
            _onCollisionStay?.Invoke(other);
            return TriggerState.Stay;
        }

        protected virtual TriggerState InvokeOnCollisionExit(Collision other) {
            if (isDebug)
                Debug.Log("OnCollisionExit", this);
            _onCollisionExit?.Invoke(other);
            return TriggerState.Exit;
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

        /// Also checks if trigger is activatable
        protected bool IsColliderValid(Collider other) {
            if (!IsActivatable) {
                return false;
            }
            switch (_triggeredBy) {
                case TriggeredBy.Tag:
                    return other.CompareTag(_triggeredByTag);
                    
                case TriggeredBy.LayerMask:
                    return _triggeredByMask.IsLayerInLayerMask(other.gameObject.layer);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        //--is invalid if collision events should not be used or if not colliding with object in layer mask or with target tag
        protected bool IsCollisionValid(Collision other) => _useCollisionEvents && IsColliderValid(other.collider);

        private void OnValidate() {
            if (Application.isEditor && Application.isPlaying) {
                print("OnValidate");
                InitialiseTrigger();
            }
        }
    }
}