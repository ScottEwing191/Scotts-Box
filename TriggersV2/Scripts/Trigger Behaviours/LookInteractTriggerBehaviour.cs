#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace ScottEwing.TriggersV2{
    [AddComponentMenu("ScottEwing/Triggers/LookInteractTrigger")]
    public class LookInteractTriggerBehaviour : BaseTrigger{
#if ODIN_INSPECTOR
        [PropertyOrder(-1)]
        [SerializeField] private LookInteractTriggerData _lookInteractTriggerData;
#endif
 
        protected override void Awake() {
            _trigger = new LookInteractTrigger(this, _lookInteractTriggerData);
            base.Awake();
        }
    }
}