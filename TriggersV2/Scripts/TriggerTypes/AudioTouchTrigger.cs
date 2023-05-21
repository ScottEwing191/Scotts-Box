using UnityEngine;

namespace ScottEwing.TriggersV2{
    public class AudioTouchTrigger : TouchTrigger{
        private AudioSource _audioSource;
        public AudioTouchTrigger(BaseTrigger trigger, ITriggerData data = null) : base(trigger, data) {
        }

        public override void Start() {
            _audioSource = Trigger.GetComponent<AudioSource>();
            if (!_audioSource) {
                Debug.LogError("No AudioSource Component", Trigger);
            }
        }

        public override bool OnTriggerEnter(Collider other) {
            if (!base.OnTriggerEnter(other)) {
                return false;
            }
            _audioSource.Play();
            return true;
        }

    }
}