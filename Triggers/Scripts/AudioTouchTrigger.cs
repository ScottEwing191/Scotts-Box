using UnityEngine;

namespace ScottEwing.Triggers{
    [AddComponentMenu("ScottEwing/Triggers/AudioTouchTrigger")]
    public class AudioTouchTrigger : TouchTrigger{
        private AudioSource _audioSource;
        [SerializeField] private bool playOnTouch = true;
        private void Start() {
            _audioSource = GetComponent<AudioSource>();
        }

        protected override void TriggerEntered(Collider other) {
            //if (!IsColliderValid(other)) return;
            base.TriggerEntered(other);
            if (playOnTouch && _audioSource != null) {
                _audioSource.Play();
            }
        }
    }
}
