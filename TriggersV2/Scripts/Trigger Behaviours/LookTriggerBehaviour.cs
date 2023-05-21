using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace ScottEwing.TriggersV2{
    [AddComponentMenu("ScottEwing/Triggers/LookTrigger")]
    public class LookTriggerBehaviour : BaseTrigger, ILookInteractable{
#if ODIN_INSPECTOR
        [PropertyOrder(-1)]
        [SerializeField] private LookTriggerData _lookTriggerData;
#endif
        protected override void Awake() {
            _trigger = new LookTrigger(this, _lookTriggerData);
            base.Awake();
        }
        
        public TriggerState Look(Collider other, bool localCast = false) {
            return ((LookTrigger)_trigger).Look(other);
        }
    }
}