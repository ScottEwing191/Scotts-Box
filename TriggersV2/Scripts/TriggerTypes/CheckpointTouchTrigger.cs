
//using ScottEwing.Checkpoints;
using UnityEngine;

namespace ScottEwing.TriggersV2{
    public class CheckpointTouchTrigger : TouchTrigger{
        //[SerializeField] private CheckpointReachedReloadTrigger _checkpointReachedReload;
        private CheckpointTouchTriggerData _data;
        public CheckpointTouchTrigger(TriggerV2 triggerV2, ITriggerData data = null) : base(triggerV2, data) {
            _data = (CheckpointTouchTriggerData)data;
        }

        public override void Awake() {
            _data._checkpointReachedReload.Init(TriggerV2.transform);
        }

        public override bool OnTriggerEnter(Collider other) {

            if (!base.OnTriggerEnter(other)) {
                return false;
            }
            _data._checkpointReachedReload.Triggered(other);
            return true;
        }

    }
}
