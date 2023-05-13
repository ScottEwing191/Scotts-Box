using UnityEngine;

namespace ScottEwing.TriggersV2{
    public class AudioTouchTrigger : TouchTrigger{
        private AudioSource _audioSource;
        public AudioTouchTrigger(TriggerV2 triggerV2, ITriggerData data = null) : base(triggerV2, data) {
        }

        public override void Start() {
            _audioSource = TriggerV2.GetComponent<AudioSource>();
            if (!_audioSource) {
                Debug.LogError("No AudioSource Component", TriggerV2);
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