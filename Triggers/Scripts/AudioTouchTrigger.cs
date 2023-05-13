using UnityEngine;

namespace ScottEwing.Triggers{
    public class AudioTouchTrigger : TouchTrigger{
        private AudioSource _audioSource;
        [SerializeField] private bool playOnTouch = true;
        private void Start() {
            _audioSource = GetComponent<AudioSource>();
        }

        protected override void OnTriggerEnter(Collider other) {
            if (!IsColliderValid(other)) return;
            base.OnTriggerEnter(other);
            if (playOnTouch) {
                _audioSource.Play();
            }
        }
    }
}
