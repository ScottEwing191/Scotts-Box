using System;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using NaughtyAttributes;
#endif
using UnityEngine;

namespace ScottEwing.TriggersV2{
    public class CombinedLookTriggerBehaviour : BaseTrigger, ILookInteractable{
        private enum TriggerType{
            Look,
            LookInteract,
        }

        [SerializeField] private TriggerType _triggerType = TriggerType.Look;

        [ShowIf("_triggerType", TriggerType.Look)]
#if ODIN_INSPECTOR
        [PropertyOrder(-1)]
#endif
        [SerializeField] private LookTriggerData _lookTriggerData;
#if ODIN_INSPECTOR
        [PropertyOrder(-1)]
#endif
        [ShowIf("_triggerType", TriggerType.LookInteract)]
        [SerializeField] private LookInteractTriggerData _lookInteractTriggerData;

        protected override void InitialiseTrigger() {
            _trigger = _triggerType switch {
                TriggerType.Look => new LookTrigger(this, _lookTriggerData),
                TriggerType.LookInteract => new LookInteractTrigger(this, _lookInteractTriggerData),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected override void Awake() {
            InitialiseTrigger();
            base.Awake();
        }

        public TriggerState Look(Collider other, bool localCast = false) {
            return ((LookTrigger)_trigger).Look(other);
        }
        
        private void OnValidate() {
            if (Application.isEditor && Application.isPlaying && gameObject.activeInHierarchy) {
                print("OnValidate");
                _trigger = null;
                InitialiseTrigger();
            }
        }
    }
}