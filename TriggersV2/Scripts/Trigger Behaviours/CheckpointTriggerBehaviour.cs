using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
namespace ScottEwing.TriggersV2{
    [AddComponentMenu("ScottEwing/Triggers/CheckpointTrigger")]
    public class CheckpointTriggerBehaviour : BaseTrigger{
#if ODIN_INSPECTOR
        [PropertyOrder(-1)]
        [SerializeField] private CheckpointTouchTriggerData _checkpointTouchTriggerData;
#endif

        protected override void Awake() {
            _trigger = new CheckpointTouchTrigger(this, _checkpointTouchTriggerData);
            base.Awake();
        }
    }
}