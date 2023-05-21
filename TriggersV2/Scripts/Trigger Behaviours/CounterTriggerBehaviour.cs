using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
namespace ScottEwing.TriggersV2{
    [AddComponentMenu("ScottEwing/Triggers/CounterTrigger")]
    public class CounterTriggerBehaviour : BaseTrigger{
#if ODIN_INSPECTOR
        [PropertyOrder(-1)]
#endif
        [SerializeField] private CounterTriggerData _counterTriggerData;

        protected override void Awake() {
            _trigger = new CounterTrigger(this, _counterTriggerData);
            base.Awake();
        }
    }
}