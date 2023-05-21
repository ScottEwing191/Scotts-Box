using UnityEngine;

namespace ScottEwing.TriggersV2{
    [AddComponentMenu("ScottEwing/Triggers/TouchTrigger")]
    public class TouchTriggerBehaviour : BaseTrigger{
        protected override void Awake() {
            _trigger = new TouchTrigger(this);
            base.Awake();
        }
    }
}