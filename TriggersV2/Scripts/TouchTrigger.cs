using UnityEngine;

namespace ScottEwing.TriggersV2{
    public class TouchTrigger : BaseTriggerType{
        
        public TouchTrigger(BaseTrigger trigger, ITriggerData data = null) : base(trigger, data) {
        }
        
        public override bool OnTriggerEnter(Collider other) {
            Triggered(other.gameObject);
            return true;
        }

    }
}