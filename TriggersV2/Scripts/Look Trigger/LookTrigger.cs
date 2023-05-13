using System;
using System.ComponentModel;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;


namespace ScottEwing.TriggersV2{
    /// <summary>
    /// This class works in tandem with the CastInteractor. each frame cast interactor looks for ILookInteractable's (e.g., this class). If this class is found the Look method
    /// is called. The usual tag or layermask check is not performed when using this trigger
    /// </summary>
    public class LookTrigger : BaseTrigger, ILookInteractable{
        [Description("")]
        public enum CastType{
            GlobalCastInteractor,
            LocalRaycastHelper,
        }
        /*[Serializable]
        private struct LocalRaycastData{
            [SerializeField] public RayCastHelper _castHelper;

            [Tooltip("The game object that need to be looked at. Default is the game object this is attached to. If different, default and _lookTarget should be under same parent")]
            [SerializeField] public Transform _lookTargetRoot;

            [Description("Is true while inside collider with defined tag or layer mask")]
            [HideInInspector] public bool _castForTrigger;
        }



        [SerializeField] private CastType _castType = CastType.GlobalCastInteractor;

#if ODIN_INSPECTOR
        [ShowIf("_castType", CastType.LocalRaycastHelper)]
#endif
        [SerializeField]
        private LocalRaycastData _castData;*/

        protected bool LookingAtTrigger { get; set; }
        private bool _isLooking;

        [Description("Used to detect the first frame that this trigger was not looked at")]
        private bool _lookedThisFixedUpdate;

        private LookTriggerData _data; 

        public LookTrigger(TriggerV2 triggerV2, ITriggerData data = null) : base(triggerV2, data) {
            _data = (LookTriggerData)data;
        }
        #region Unity Methods

        public override void Start() {
            if (_data._castType == CastType.LocalRaycastHelper && _data._castData._lookTargetRoot == null) {
                _data._castData._lookTargetRoot = TriggerV2.gameObject.transform;
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
            if (!TriggerV2.IsActivatable) return TriggerState.None;
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
            return TriggerV2.InvokeOnTriggerEnter(other);
        }

        protected virtual TriggerState LookStay(Collider other) {
            return TriggerV2.InvokeOnTriggerStay(other);
        }

        protected virtual TriggerState LookExit(Collider other) {
            if (!LookingAtTrigger) {
                Debug.Log("Need to confirm that the return is correct");
                return TriggerState.None;
            }

            LookingAtTrigger = false;
            return TriggerV2.InvokeOnTriggerExit(other);
        }
    }
}