using System;
using System.Collections;
using ScottEwing.ExtensionMethods;
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
namespace ScottEwing.Triggers{
    /// <summary>
    /// States are anly valid for the frame they are received
    /// </summary>
    public enum TriggerState{
        Enter,
        Stay,
        Exit,
        None
    }

    [AddComponentMenu("ScottEwing/Triggers/Trigger")]
    public class Trigger : MonoBehaviour{
        private enum TriggeredType{
            DestroyOnTriggered,
            DisableOnTriggered,
            CooldownOnTriggered,
            RemainActive,
            DisableTriggerComponent
        }

        /*private enum TriggeredBy{
            Tag,
            LayerMask,
            Either
        }

        [SerializeField] private TriggeredBy _triggeredBy = TriggeredBy.Tag;
#if ODIN_INSPECTOR
        [ShowIf("_triggeredBy", TriggeredBy.LayerMask)]
#endif
        [SerializeField] private LayerMask _triggeredByMask;
#if ODIN_INSPECTOR
        [ShowIf("_triggeredBy", TriggeredBy.Tag)]
#endif
        [SerializeField] protected string _triggeredByTag = "Player";
        */


        [SerializeField] private TriggerBy _triggerBy;

        [SerializeField] private TriggeredType _triggeredType = TriggeredType.DestroyOnTriggered;
        [ShowIf("_triggeredType", TriggeredType.CooldownOnTriggered)]
        [SerializeField] private float _cooldownTime = 2.0f;

        [field: SerializeField] protected bool IsActivatable { get; set; } = true;
        [SerializeField] protected bool isDebug;
        [SerializeField] private bool _invokeOnTriggerExitWhenTriggered = false;
        private Coroutine _cooldownRoutine;
        protected Collider currentCollider;

        [SerializeField] protected UnityEvent<Collider> _onTriggered;

        //--Trigger Unity Events
        [Header("Trigger Events")]
        [SerializeField] private bool _useTriggerEvents = true;

        [ShowIf("_useTriggerEvents")] [SerializeField]
        protected UnityEvent<Collider> _onTriggerEnter;

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


        #region On Trigger Events

        //--Trigger Enter
        private void OnTriggerEnter(Collider other) {
            if (_useTriggerEvents && _triggerBy.IsColliderValid(other) && CanEnterTrigger(other)) {
                TriggerEntered(other);
            }
        }
        protected virtual void TriggerEntered(Collider other) => InvokeOnTriggerEnter(other);
        protected virtual bool CanEnterTrigger(Collider other) => true;

        //--Trigger Stay
        private void OnTriggerStay(Collider other) {
            if (_useTriggerEvents && _triggerBy.IsColliderValid(other) && CanStayTrigger(other)) {
                TriggerStay(other);
            }
        }
        protected virtual void TriggerStay(Collider other) => InvokeOnTriggerStay(other);
        protected virtual bool CanStayTrigger(Collider other) => true;
        
        //--Trigger Exit
        private void OnTriggerExit(Collider other) {
            if (_useTriggerEvents && _triggerBy.IsColliderValid(other) && CanExitTrigger(other)) {
                TriggerExited(other);
            }
        }
        protected virtual void TriggerExited(Collider other) => InvokeOnTriggerExit(other);
        protected virtual bool CanExitTrigger(Collider other) => true;

        #endregion


        #region On Collision Events

        //--Collision Enter
        protected virtual void OnCollisionEnter(Collision collision) {
            if (_useCollisionEvents && _triggerBy.IsCollisionValid(collision) && CanCollisionEnter(collision)) {
                CollisionEntered(collision);
            }
        }
        protected virtual void CollisionEntered(Collision other) => InvokeOnCollisionEnter(other);
        protected virtual bool CanCollisionEnter(Collision other) => true;

        
        //--Collision Stay
        protected virtual void OnCollisionStay(Collision collision) {
            if (_useCollisionEvents && _triggerBy.IsCollisionValid(collision) && CanCollisionStay(collision)) {
                CollisionStay(collision);
            }
        }
        protected virtual void CollisionStay(Collision collision) => InvokeOnCollisionStay(collision);
        protected virtual bool CanCollisionStay(Collision collision) => true;


        //--Collision Exit
        protected virtual void OnCollisionExit(Collision collision) {
            if (_useCollisionEvents && _triggerBy.IsCollisionValid(collision) && CanCollisionExit(collision)) {
                CollisionExited(collision);
            }
        }
        protected virtual void CollisionExited(Collision other) => InvokeOnCollisionExit(other);
        protected virtual bool CanCollisionExit(Collision other) => true;

        #endregion


        #region Invoke Trigger Unity EVents

        protected TriggerState InvokeOnTriggerEnter(Collider other) {
            if (isDebug)
                Debug.Log("OnTriggerEnter", this);
            _onTriggerEnter?.Invoke(other);
            currentCollider = other;
            return TriggerState.Enter;
        }

        protected TriggerState InvokeOnTriggerStay(Collider other = null) {
            if (isDebug)
                Debug.Log("OnTriggerStay", this);

            _onTriggerStay?.Invoke(other);
            return TriggerState.Stay;
        }

        // The collider passed will be null on the following occasions. The trigger has been triggered while invokeOnTriggerExitOnTrigger
        // is true. It will also be null when it is called after Look Interact Trigger Looks away from it collider
        protected TriggerState InvokeOnTriggerExit(Collider other) {
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

        protected TriggerState InvokeOnCollisionEnter(Collision other) {
            if (isDebug)
                Debug.Log("OnCollisionEnter", this);
            _onCollisionEnter?.Invoke(other);
            return TriggerState.Enter;
        }

        protected TriggerState InvokeOnCollisionStay(Collision other) {
            if (isDebug)
                Debug.Log("OnCollisionStay", this);
            _onCollisionStay?.Invoke(other);
            return TriggerState.Stay;
        }

        protected TriggerState InvokeOnCollisionExit(Collision other) {
            if (isDebug)
                Debug.Log("OnCollisionExit", this);
            _onCollisionExit?.Invoke(other);
            return TriggerState.Exit;
        }

        #endregion

        protected virtual bool Triggered(Collider other) {
            if (!gameObject.activeSelf || !gameObject.activeInHierarchy) return false;
            if (!IsActivatable) return false;
            if (isDebug)
                Debug.Log("Triggered", this);
            _onTriggered.Invoke(other);
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
                case TriggeredType.DisableTriggerComponent:
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

        /*public bool IsColliderValid(Collider other, bool isActivatable = true) {
            if (!isActivatable) {
                return false;
            }

            return _triggeredBy switch {
                TriggeredBy.Tag => other.CompareTag(_triggeredByTag),
                TriggeredBy.LayerMask => _triggeredByMask.IsLayerInLayerMask(other.gameObject.layer),
                TriggeredBy.Either => other.CompareTag(_triggeredByTag) || _triggeredByMask.IsLayerInLayerMask(other.gameObject.layer),
                _ => false
            };
        }
        
        //--is invalid if collision events should not be used or if not colliding with object in layer mask or with target tag
        public bool IsCollisionValid(Collision other, bool isActivatable = true) => IsColliderValid(other.collider, isActivatable);
        */


        /*/// Also checks if trigger is activatable
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
        protected bool IsCollisionValid(Collision other) => _useCollisionEvents && IsColliderValid(other.collider);*/
    }
}