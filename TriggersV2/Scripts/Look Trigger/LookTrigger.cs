using System.ComponentModel;
using UnityEngine;


namespace ScottEwing.TriggersV2{
    /// <summary>
    /// This class works in tandem with the CastInteractor. each frame cast interactor looks for ILookInteractable's (e.g., this class). If this class is found the Look method
    /// is called. The usual tag or layermask check is not performed when using this trigger
    /// </summary>
    public class LookTrigger : BaseTriggerType, ILookInteractable{
        [Description("")]
        public enum CastType{
            GlobalCastInteractor,
            LocalRaycastHelper,
        }
        
        protected bool LookingAtTrigger { get; set; }
        private bool _isLooking;

        [Description("Used to detect the first frame that this trigger was not looked at")]
        private bool _lookedThisFixedUpdate;

        private LookTriggerData _data; 

        public LookTrigger(BaseTrigger trigger, ITriggerData data = null) : base(trigger, data) {
            _data = (LookTriggerData)data;
        }
        #region Unity Methods

        public override void Start() {
            if (_data._castType == CastType.LocalRaycastHelper && _data._castData._lookTargetRoot == null) {
                _data._castData._lookTargetRoot = Trigger.gameObject.transform;
            }
        }

        public override void FixedUpdate() {
            if (_data._castType == CastType.LocalRaycastHelper) {
                DoLocalCast();
            }
            HandleFirstUpdateNotLookedAt();
        }

        //-- Dont want base behaviour
        public override bool OnTriggerEnter(Collider other) {
            _data._castData._castForTrigger = true;
            return false;
        }

        public override bool OnTriggerStay(Collider other) {
            return false;
        }

        public override bool OnTriggerExit(Collider other) {
            _data._castData._castForTrigger = false;
            return false;
        }

        #endregion


        /// <summary>
        /// Determines if this fixed update was the first time the trigger was not looked and calls look exit if it was
        /// </summary>
        private void HandleFirstUpdateNotLookedAt() {
            if (!_isLooking && _lookedThisFixedUpdate) {
                _isLooking = true;
            }

            if (_isLooking && !_lookedThisFixedUpdate) {
                _isLooking = false;
                LookExit(null);
            }

            _lookedThisFixedUpdate = false; // reset this for next frame
        }

        private void DoLocalCast() {
            if (!_data._castData._castForTrigger) return;
            if (!_data._castData._castHelper.Cast(out var hit)) return;
            if (!hit.transform.IsChildOf(_data._castData._lookTargetRoot)) return;
            Look(hit.collider, true);
        }

        public TriggerState Look(Collider other, bool localCast = false) {
            //-- Return if not called locally while in local cast mode
            if (!localCast && _data._castType == CastType.LocalRaycastHelper)
                return TriggerState.None;

            _lookedThisFixedUpdate = true;
            if (!Trigger.IsActivatable) return TriggerState.None;
            if (!LookingAtTrigger) {
                return LookEnter(other);
            }

            if (LookingAtTrigger) {
                return LookStay(other);
            }

            return TriggerState.None;
        }

        protected virtual TriggerState LookEnter(Collider other) {
            if (LookingAtTrigger) {
                Debug.Log("Need to confirm that the return is correct");
                return TriggerState.None;
            }

            LookingAtTrigger = true;
            return Trigger.InvokeOnTriggerEnter(other);
        }

        protected virtual TriggerState LookStay(Collider other) {
            return Trigger.InvokeOnTriggerStay(other);
        }

        protected virtual TriggerState LookExit(Collider other) {
            if (!LookingAtTrigger) {
                Debug.Log("Need to confirm that the return is correct");
                return TriggerState.None;
            }

            LookingAtTrigger = false;
            return Trigger.InvokeOnTriggerExit(other);
        }
    }
}