using UnityEngine;

namespace ScottEwing.TriggersV2{
    [AddComponentMenu("ScottEwing/Triggers/AudioTouchTrigger")]
    public class AudioTouchTriggerBehaviour : BaseTrigger{
        protected override void Awake() {
            _trigger = new AudioTouchTrigger(this);
            base.Awake();
        }
    }
}