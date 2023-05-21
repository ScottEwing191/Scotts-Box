using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
namespace ScottEwing.TriggersV2{
    [AddComponentMenu("ScottEwing/Triggers/ToggleInteractTrigger")]
    public class ToggleInteractTriggerBehaviour : BaseTrigger{
#if ODIN_INSPECTOR
        [PropertyOrder(-1)]
#endif
        [SerializeField] private ToggleInteractTriggerData _toggleInteractTriggerData;

        protected override void Awake() {
            _trigger = new ToggleInteractTrigger(this, _toggleInteractTriggerData);
            base.Awake();
        }
    }
}