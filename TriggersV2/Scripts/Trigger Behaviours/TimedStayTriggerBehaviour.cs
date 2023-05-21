using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
namespace ScottEwing.TriggersV2{
    [AddComponentMenu("ScottEwing/Triggers/TimedStayTrigger")]
    public class TimedStayTriggerBehaviour : BaseTrigger{
#if ODIN_INSPECTOR
        [PropertyOrder(-1)]
#endif
        [SerializeField] private TimedStayTriggerData _timedStayTriggerData;

        protected override void Awake() {
            _trigger = new TimedStayTrigger(this, _timedStayTriggerData);
            base.Awake();
        }
    }
}