using UnityEngine;

namespace ScottEwing.TriggersV2{
    public class TouchTrigger : BaseTrigger{
        
        public TouchTrigger(TriggerV2 triggerV2, ITriggerData data = null) : base(triggerV2, data) {
        }
        
        public override bool OnTriggerEnter(Collider other) {
            Triggered();
            return true;
        }

    }
}