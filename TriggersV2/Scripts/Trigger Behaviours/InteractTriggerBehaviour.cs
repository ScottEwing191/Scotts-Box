
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace ScottEwing.TriggersV2{
[AddComponentMenu("ScottEwing/Triggers/InteractTrigger")]
    public class InteractTriggerBehaviour : BaseTrigger
    {
#if ODIN_INSPECTOR
        [PropertyOrder(-1)]
#endif
        [SerializeField] private InteractTriggerData _interactTriggerData;

        protected override void Awake() {
            _trigger = new InteractTrigger(this, _interactTriggerData);
            base.Awake();
        }
    }
}
